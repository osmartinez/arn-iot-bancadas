using ArnMonitorBancadaWPF.Util;
using ArnMonitorBancadaWPF.Ventanas;
using CalculoActividad;
using ConfiguracionLocal;
using EF.Select;
using Entidades;
using Entidades.Enum;
using Entidades.Eventos;
using Logs;
using MQTT;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Turnos;

namespace ArnMonitorBancadaWPF
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private EF.SelectProcedimientos.SelectBarquillas selectBarquillas = new EF.SelectProcedimientos.SelectBarquillas();
        private EF.AddProcedimientos.AddColaTrabajo addColaTrabajo = new EF.AddProcedimientos.AddColaTrabajo();
        private EF.Select.SelectMaquinasRegistroDatos selectRegistroDatos = new EF.Select.SelectMaquinasRegistroDatos();
        private EF.Select.SelectControl selectControl = new EF.Select.SelectControl();
        private FicheroConfiguracion ficheroConfig = new FicheroConfiguracion();
        private EF.Select.SelectBancada selectBancadas = new EF.Select.SelectBancada();

        private string etiqueta = "";
        private Fichajes fichajes = new Fichajes();
        private Queue<EventoFichajeAsociacion> colaEventosFichaje = new Queue<EventoFichajeAsociacion>();
        private DispatcherTimer timerEventoFichaje = new DispatcherTimer();
        private DispatcherTimer timerNumVueltas = new DispatcherTimer();
        private DispatcherTimer timerInactividad = new DispatcherTimer();
        public Operarios Operario { get; set; } = new Operarios { Nombre = "<SIN OPERARIO>" };

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            if (!System.Security.Principal.WindowsIdentity.GetCurrent().Name.Contains("omartinez"))
            {
                KillExplorer();
                this.Topmost = true;
                this.WindowStyle = WindowStyle.None;
            }

            if (SesionManager.Sesion.Operario == null)
            {
                while (SesionManager.Sesion.Operario == null)
                {
                    Ventanas.Login login = new Ventanas.Login();
                    login.ShowDialog();
                }


                this.Operario = SesionManager.Sesion.Operario;

                Bienvenida welcome = new Bienvenida();
                welcome.Show();

                Notificar("Operario");
            }



            CargarDatos();



            this.Closing += (s, e) =>
            {
                Logs.Log.Write(new Exception(" Cerrando aplicacion..."));
                ClienteMQTT.Cerrar();
            };


        }

        private void CargarDatos()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (s, e) =>
            {
                Configuracion cfg = ficheroConfig.LeerConfiguracion();
                Store.Bancada = selectBancadas.BuscarPorId(cfg.Bancada.Id);
                CargarPaquetesPrevios();
                ClienteMQTT.Iniciar();

            };

            bw.RunWorkerCompleted += (s, e) =>
            {

                this.fichajes.OnFichajeAsociacion += Fichajes_OnFichajeAsociacion;

                this.timerEventoFichaje.Interval = new TimeSpan(0, 0, 5);
                this.timerEventoFichaje.Tick += TimerEventoFichaje_Tick;
                this.timerEventoFichaje.Start();

                this.timerNumVueltas.Interval = new TimeSpan(0, 0, 10);
                this.timerNumVueltas.Tick += TimerNumVueltas_Tick; ;
                this.timerNumVueltas.Start();

                double minSgCiclo = 360;
                this.timerInactividad.Interval = new TimeSpan(0, 0, (int)minSgCiclo);
                this.timerInactividad.Tick += TimerInactividad_Tick;
                this.timerInactividad.Start();

                this.PreviewKeyUp += MainWindow_PreviewKeyUp;

                ClienteMQTT.Topics[1].Callbacks.Add(Normal);
                ClienteMQTT.Topics[2].Callbacks.Add(Calentar);

                Store.StoreIniciada();

            };

            bw.RunWorkerAsync();
        }


        private void TimerInactividad_Tick(object sender, EventArgs e)
        {
            try
            {
                if (Visibility.Hidden == this.primaActual.Visibility)
                {
                    return;
                }

                this.temporizador.Reset();
                Dispatcher.Invoke(() =>
                {
                    this.primaActual.Visibility = Visibility.Hidden;
                    this.temporizador.Visibility = Visibility.Visible;
                });
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }

        }

        private void CargarPaquetesPrevios()
        {
            try
            {
                DateTime ahora = DateTime.Now;
                Operarios op = SesionManager.Sesion.Operario;
                Turno turno = Horario.CalcularTurnoAFecha(ahora);
                DateTime fechaInicio;
                DateTime fechaFin;
                Horario.CalcularHorarioTurno(turno, ahora, out fechaInicio, out fechaFin);

                var paquetes = selectRegistroDatos.HistoricoPaquetesOperario(op.Id, fechaInicio, fechaFin);

                foreach (var paquete in paquetes.Where(x => x.PiezaIntroducida))
                {
                    var maq = Store.Bancada.Maquinas.FirstOrDefault(x => x.IpAutomata == paquete.IpAutomata && x.Posicion == paquete.PosicionMaquina);
                    if (maq != null)
                    {
                        maq.Pulsos.Add(new PulsoMaquina
                        {
                            CodigoEtiqueta = paquete.CodigoEtiqueta,
                            Control = BuscarControl(paquete.IdOperacion, maq),
                            Ciclo = paquete.Ciclo,
                            Fecha = paquete.FechaCreacion,
                            Pares = paquete.Pares,
                            PosicionGlobal = maq.PosicionGlobal ?? 0,
                            IdOperario = paquete.IdOperario,
                        }); ;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }

        }

        private void TimerNumVueltas_Tick(object sender, EventArgs e)
        {
            try
            {
                if (SesionManager.Sesion.Operario != null)
                {
                    int maxVueltas = Store.Bancada.Maquinas.Max(x => x.Pulsos.Where(y => y.IdOperario == SesionManager.Sesion.Operario.Id).Count());
                    int pares = Store.Bancada.Maquinas.Sum(x => x.Pulsos.Where(y => y.IdOperario == SesionManager.Sesion.Operario.Id).Sum(y => y.Pares));
                    double primaAcumulada = Calculo.CalcularPrima(Store.Bancada.Maquinas.ToList());
                    double limiteBancada = 0;
                    double primaUltimaVuelta = Calculo.CalcularPrimaDirecto(Store.Bancada.Maquinas.ToList(), Store.EventosFichajes, out limiteBancada);

                    this.contadorVueltas.Vueltas = maxVueltas;
                    this.limitacionBancada.Limite = limiteBancada;
                    this.primaAcumulada.Prima = primaAcumulada;
                    this.primaActual.Prima = primaUltimaVuelta;
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }


        }

        private void Calentar(string msg, string topicRecibido, Topic topic)
        {
            try
            {
                ConsumoPrensa consumo = JsonConvert.DeserializeObject<ConsumoPrensa>(msg);

                if (consumo == null)
                {
                    Log.Write(new Exception("Consumo recibido nulo " + msg));
                }

                string tipoBancada = topicRecibido.Split('/')[topic.IndiceTipoBancada].Trim();
                string ipPlc = topicRecibido.Split('/')[topic.IndiceIdTopic].Trim();
                consumo.IpPlc = ipPlc;

                Maquinas maquina = Store.Bancada.Maquinas.FirstOrDefault(x => x.IpAutomata == ipPlc && x.Posicion == consumo.Prensa);

                if (maquina != null)
                {
                    maquina.CargarInformacion(consumo);
                    if (maquina.Modo != ModoMaquina.Calentamiento)
                    {
                        maquina.CambiarModo(ModoMaquina.Calentamiento);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }

        }

        private void Normal(string msg, string topicRecibido, Topic topic)
        {
            try
            {
                // recibo consumo
                ConsumoPrensa consumo = JsonConvert.DeserializeObject<ConsumoPrensa>(msg);

                if (consumo == null)
                {
                    Log.Write(new Exception("Consumo recibido nulo " + msg));
                    return;
                }

                string tipoBancada = topicRecibido.Split('/')[topic.IndiceTipoBancada].Trim();
                string ipPlc = topicRecibido.Split('/')[topic.IndiceIdTopic].Trim();
                consumo.IpPlc = ipPlc;


                // busco la maquina
                Maquinas maquina = Store.Bancada.Maquinas.FirstOrDefault(x => x.IpAutomata == ipPlc && x.Posicion == consumo.Prensa);

                // si encuentro la maquina
                if (maquina != null)
                {
                    // si la maquina no esta en modo normal 
                    if (maquina.Modo != ModoMaquina.Normal)
                    {
                        // la pongo en modo normal
                        maquina.CambiarModo(ModoMaquina.Normal);
                    }

                    // si hay operario loggeado
                    if (SesionManager.Sesion.Operario != null)
                    {
                        // pongo en marcha si no estaba la visibilidad del cuadrante de prima
                        Dispatcher.Invoke(() =>
                        {
                            this.primaActual.Visibility = Visibility.Visible;
                            this.temporizador.Visibility = Visibility.Hidden;
                        });
                        // reseteo el timer de actividad
                        this.timerInactividad.Stop();
                        this.timerInactividad.Start();

                        // cargo informacion del consumo en la maquina por si acaso no estaba
                        maquina.CargarInformacion(consumo);
                        // añado el pulso a la maquina
                        maquina.Pulsos.Add(new PulsoMaquina
                        {
                            CodigoEtiqueta = maquina.TrabajoEjecucion.CodigoEtiquetaFichada,
                            // previamente busco el control en la tienda
                            Control = BuscarControl(maquina.IdOperacion, maquina),
                            Ciclo = consumo.SgCiclo,
                            Fecha = consumo.HoraLocal,
                            Pares = consumo.ParesUtillaje * consumo.NumMoldes,
                            PosicionGlobal = maquina.PosicionGlobal ?? 0,
                            IdOperario = SesionManager.Sesion.Operario.Id,
                        });



                        // si lo hemos encontrado y ademas se introdujo una pieza
                        if (maquina.TrabajoEjecucion != null)
                        {
                            if (consumo.PiezaIntroducida == 1)
                            {
                                // insertamos pieza
                                bool insertados = maquina.InsertarPares(maquina.TrabajoEjecucion, consumo.NumMoldes * consumo.ParesUtillaje);
                            }
                        }
                        else
                        {

                            //(background) recuperar cola y publicarla
                            BackgroundWorker bw = new BackgroundWorker();
                            List<MaquinasColasTrabajo> colaTrabajo = new List<MaquinasColasTrabajo>();
                            bw.DoWork += (se, ev) =>
                            {
                                colaTrabajo = SelectColaTrabajo.ObtenerColaTrabajoMaquinaPorId(maquina.ID);
                            };
                            bw.RunWorkerCompleted += (se, ev) =>
                            {
                                if(colaTrabajo.Count == 0)
                                {
                                    ClienteMQTT.Publicar(string.Format("/moldeado/fallo/{0}", maquina.IpAutomata.PadLeft(3)), 
                                        string.Format("El automata {0} con maquina {1} no tiene cola de trabajo",maquina.IpAutomata,maquina.Posicion ), 1);
                                }
                                maquina.AsignarColaTrabajo(colaTrabajo);
                                
                            };
                            bw.RunWorkerAsync();


                            maquina.ErrorTareaSinEjecutar();
                            maquina.ParesConsumidos();
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }

        private OperacionesControles BuscarControl(int idOperacion, Maquinas maq)
        {
            OperacionesControles ctrl = Store.Controles.FirstOrDefault(x => x.idOfos.Contains(idOperacion));

            if (ctrl == null)
            {
                ctrl = selectControl.BuscarControlOperacion(idOperacion, maq.IdTipo ?? 0);
                if (ctrl != null)
                {
                    Store.Controles.Add(ctrl);
                }
            }

            return ctrl;

        }

        private void TimerEventoFichaje_Tick(object sender, EventArgs e)
        {
            TbCodigo.Focus();
            EventoFichajeAsociacion evento = null;

            try
            {
                if (colaEventosFichaje.Count == 0 || SesionManager.Sesion.Operario == null) return;
                evento = colaEventosFichaje.Dequeue();
                if (evento != null)
                {

                    //asociar
                    var maquina = Store.Bancada.Maquinas.FirstOrDefault(x => x.CodigoEtiqueta == evento.CodigoMaquina);
                    if (maquina != null)
                    {
                        var infoBarquillaSeccion = selectBarquillas.BuscarInformacionBarquilla(evento.CodigoBarquilla, maquina.CodSeccion);
                        if (infoBarquillaSeccion.Any())
                        {
                            var idsOrden = infoBarquillaSeccion.Select(x => x.IdOrden);
                            var idsOrdenDistinto = idsOrden.Distinct();
                            if (idsOrden.Count() != idsOrdenDistinto.Count())
                            {
                                // multiOperacion
                            }
                            else
                            {
                                var idsTareas = infoBarquillaSeccion.Select(x => x.IdTarea.Value).Distinct().ToList();

                                evento.Control = BuscarControl(infoBarquillaSeccion.First().IdOperacion, maquina);

                                // bd
                                BackgroundWorker bwActualizarCola = new BackgroundWorker();
                                List<MaquinasColasTrabajo> cola = new List<MaquinasColasTrabajo>();
                                bwActualizarCola.DoWork += (se, ev) =>
                                {
                                    cola = addColaTrabajo.ActualizarColaTrabajo(evento.CodigoBarquilla, idsTareas, infoBarquillaSeccion.First().Agrupacion ?? 0, maquina.ID, SesionManager.Sesion.Operario.Id, infoBarquillaSeccion.Sum(x => x.Cantidad));
                                };
                                bwActualizarCola.RunWorkerCompleted += (se, ev) =>
                                {
                                    maquina.AsignarColaTrabajo(cola);
                                };
                                bwActualizarCola.RunWorkerAsync();

                                // mqtt
                                MqttAsociarBarquilla(infoBarquillaSeccion, maquina);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                colaEventosFichaje.Enqueue(evento);
                Log.Write(ex);
            }
        }

        private void MqttAsociarBarquilla(List<SP_BarquillaBuscarInformacionEnSeccion_Result> prepaquete, Maquinas maquina, bool asociacion = true)
        {
            try
            {
                string nombreCliente = prepaquete.First().NOMBRECLI ?? "ARNEPLANT S.L.";
                nombreCliente = new Regex("[^A-Za-z0-9 ]").Replace(nombreCliente, " ");
                if (nombreCliente.Length > 25)
                {
                    nombreCliente = nombreCliente.Substring(0, 24);
                }

                string mensaje = string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};",
                    maquina.Posicion
                    , prepaquete.First().IdTarea.ToString().PadLeft(10, '0')
                    , prepaquete.First().CantidadFabricar.ToString().PadLeft(10, '0')
                    , prepaquete.First().Codigo.PadLeft(13)
                    , prepaquete.First().CodUtillaje.PadLeft(25)
                    , prepaquete.First().IdUtillajeTalla.PadLeft(10)
                    , prepaquete.First().Talla.PadLeft(10)
                    , prepaquete.First().CodigoEtiqueta.PadLeft(13)
                    , prepaquete.First().IdOrden.ToString().PadLeft(10, '0')
                    , prepaquete.First().IdOperacion.ToString().PadLeft(10, '0')
                    , nombreCliente.PadLeft(25)
                    , prepaquete.First().CodigoArticulo.PadLeft(20)
                    , prepaquete.First().Productividad.ToString().PadLeft(3)
                    , SesionManager.Sesion.Operario.Id.ToString().PadLeft(5));

                ClienteMQTT.Publicar(string.Format("/moldeado/plc/{0}/asociarTarea", maquina.IpAutomata.PadLeft(3)), mensaje, 1);
                ClienteMQTT.Publicar(string.Format("/moldeado/plc/{0}/asociarTarea", maquina.IpAutomata.PadLeft(3)), mensaje, 1);
                ClienteMQTT.Publicar(string.Format("/moldeado/plc/{0}/asociarTarea", maquina.IpAutomata.PadLeft(3)), mensaje, 1);
                ClienteMQTT.Publicar(string.Format("/moldeado/plc/{0}/asociarTarea", maquina.IpAutomata.PadLeft(3)), mensaje, 1);


                maquina.CargarInformacion(new AsociacionTarea
                {
                    Cliente = prepaquete.First().NOMBRECLI,
                    CodigoArticulo = prepaquete.First().CodigoArticulo,
                    CodigoOrden = prepaquete.First().Codigo,
                    IdOperacion = prepaquete.First().IdOperacion,
                    IdOrden = prepaquete.First().IdOrden,
                    Utillaje = prepaquete.First().CodUtillaje,
                    TallaUtillaje = prepaquete.First().IdUtillajeTalla,
                    TallasArticulo = prepaquete.First().Tallas,
                    IdTarea = prepaquete.First().IdTarea ?? 0,
                    Pares = prepaquete.First().CantidadFabricar.HasValue ? (int)prepaquete.First().CantidadFabricar : 0,
                    CodigoEtiqueta = prepaquete.First().CodigoEtiqueta,
                    IdOperario = SesionManager.Sesion.Operario.Id,
                    ParesUtillaje = (int)prepaquete.First().Productividad,
                    Prensa = maquina.Posicion,
                    CantidadCaja = prepaquete.Sum(x => x.Cantidad),
                });


            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }


        private void MainWindow_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.Down || e.Key == Key.Up || e.Key == Key.F1)
                {
                    if (e.Key == Key.F1)
                    {
                        Configurar c = new Configurar();
                        c.ShowDialog();
                    }
                    else
                    {
                        this.LayoutPrensas.MoverPrensa(e.Key);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }

        private void Fichajes_OnFichajeAsociacion(object sender, EventoFichajeAsociacion e)
        {
            try
            {
                Store.EventosFichajes.Add(e);
                this.colaEventosFichaje.Enqueue(e);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void Notificar(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void BtLogout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (InputManager.Current.MostRecentInputDevice is KeyboardDevice)
                {
                    e.Handled = true;
                    return;
                }

                Store.Reset();

                ImprimirAuditoria impAud = new ImprimirAuditoria();
                impAud.ShowDialog();

                ClienteMQTT.Cerrar();
                SesionManager.Sesion.Operario = null;
                while (SesionManager.Sesion.Operario == null)
                {
                    Ventanas.Login login = new Ventanas.Login();
                    login.ShowDialog();
                }

                this.Operario = SesionManager.Sesion.Operario;

                ClienteMQTT.Iniciar();

                Bienvenida welcome = new Bienvenida();
                welcome.Show();
                this.CargarPaquetesPrevios();

                Notificar("Operario");
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }

        private void KillExplorer()
        {
            try
            {
                // Create a ProcessStartInfo, otherwise Explorer comes back to haunt you.
                ProcessStartInfo TaskKillPSI = new ProcessStartInfo("taskkill", "/F /IM explorer.exe");
                // Don't show a window
                TaskKillPSI.WindowStyle = ProcessWindowStyle.Hidden;
                // Create and start the process, then wait for it to exit.
                Process process = new Process();
                process.StartInfo = TaskKillPSI;
                process.Start();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }

        }

        private void TbCodigo_TextChanged(object sender, TextChangedEventArgs e)
        {
            string codigo = TbCodigo.Text;
            if (codigo.Length == 12)
            {
                TbCodigo.Clear();
                this.fichajes.EtiquetaFichada(codigo);
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.TbCodigo.Focus();
        }
    }
}
