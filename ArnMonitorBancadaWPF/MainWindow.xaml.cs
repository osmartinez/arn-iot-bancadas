using ArnMonitorBancadaWPF.Util;
using ArnMonitorBancadaWPF.Ventanas;
using CalculoActividad;
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

            CargarPaquetesPrevios();

            ClienteMQTT.Iniciar();
            this.Loaded += (s, e) => { KillExplorer(); };
            this.Closing += (s, e) =>
            {
                Logs.Log.Write(new Exception(" Cerrando aplicacion..."));
                ClienteMQTT.Cerrar();
            };

            this.fichajes.OnFichajeAsociacion += Fichajes_OnFichajeAsociacion;

            this.timerEventoFichaje.Interval = new TimeSpan(0, 0, 5);
            this.timerEventoFichaje.Tick += TimerEventoFichaje_Tick;
            this.timerEventoFichaje.Start();

            this.timerNumVueltas.Interval = new TimeSpan(0, 0, 10);
            this.timerNumVueltas.Tick += TimerNumVueltas_Tick; ;
            this.timerNumVueltas.Start();

            double minSgCiclo = 90;
            this.timerInactividad.Interval = new TimeSpan(0, 0, (int)minSgCiclo);
            this.timerInactividad.Tick += TimerInactividad_Tick;
            this.timerInactividad.Start();

            this.PreviewKeyUp += MainWindow_PreviewKeyUp;

            ClienteMQTT.Topics[1].Callbacks.Add(Normal);
            ClienteMQTT.Topics[2].Callbacks.Add(Calentar);

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
                    if (maquina.Modo != ModoMaquina.Normal)
                    {
                        maquina.CambiarModo(ModoMaquina.Normal);
                    }

                    if (SesionManager.Sesion.Operario != null)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            this.primaActual.Visibility = Visibility.Visible;
                            this.temporizador.Visibility = Visibility.Hidden;
                        });
                        this.timerInactividad.Stop();
                        this.timerInactividad.Start();

                        maquina.CargarInformacion(consumo);
                        maquina.Pulsos.Add(new PulsoMaquina
                        {
                            Control = BuscarControl(consumo.IdOperacion, maquina),
                            Ciclo = consumo.SgCiclo,
                            Fecha = consumo.HoraLocal,
                            Pares = consumo.ParesUtillaje * consumo.NumMoldes,
                            PosicionGlobal = maquina.PosicionGlobal ?? 0,
                            IdOperario = SesionManager.Sesion.Operario.Id,
                        });

                        if (consumo.IdObrero != SesionManager.Sesion.Operario.Id)
                        {
                            MqttAsociarBarquilla(new SP_BarquillaBuscarInformacionEnSeccion_Result
                            {
                                Codigo = consumo.CodigoOF,
                                CodigoEtiqueta = consumo.CodigoBarras,
                                CodUtillaje = consumo.Utillaje,
                                NOMBRECLI = consumo.NombreCliente,
                                IdOrden = consumo.IdOF,
                                IdOperacion = consumo.IdOperacion,
                                IdTarea = consumo.IdTarea,
                                IdUtillajeTalla = consumo.TallaUtillaje,
                                Talla = consumo.TallaArticulo,
                                Tallas = consumo.TallaArticulo,
                                CodigoArticulo = consumo.CodigoArticulo,
                                CantidadFabricar = consumo.ParesTarea,
                                Productividad = consumo.ParesUtillaje,

                            }, maquina);
                        }

                        MaquinasColasTrabajo trabajo = null;// maquina.MaquinasColasTrabajo.FirstOrDefault(x => x.IdTarea == consumo.IdTarea);
                        if (trabajo != null && consumo.PiezaIntroducida == 1)
                        {
                            bool insertados = maquina.InsertarPares(trabajo, consumo.NumMoldes * consumo.ParesUtillaje);
                            if (trabajo.ParesPendientes <= 0)
                            {
                                try
                                {
                                    // maquina.AsignarColaTrabajo(Select.ColaTrabajoMaquinaPorId(maquina.ID));
                                }
                                catch (Exception ex)
                                {

                                }
                            }
                            if (insertados)
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    //this.controlErrores.RemoveError(new ErrorFichajeMaquina { IdMaquina = maquina.ID });

                                });
                            }
                        }
                        else
                        {
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
                                var idsTareas = infoBarquillaSeccion.Select(x => x.IdTarea).Distinct();
                                var idsTareasStr = string.Join(",", idsTareas);


                                evento.Control = BuscarControl(infoBarquillaSeccion.First().IdOperacion, maquina);

                                // bd
                                addColaTrabajo.ActualizarColaTrabajo(idsTareasStr, infoBarquillaSeccion.First().Agrupacion ?? 0, maquina.ID, SesionManager.Sesion.Operario.Id);

                                // mqtt
                                MqttAsociarBarquilla(infoBarquillaSeccion.First(), maquina);
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

        private void MqttAsociarBarquilla(SP_BarquillaBuscarInformacionEnSeccion_Result prepaquete, Maquinas maquina)
        {
            try
            {
                string nombreCliente = prepaquete.NOMBRECLI;
                if (nombreCliente.Length > 25)
                {
                    nombreCliente = nombreCliente.Substring(0, 24);
                }
                nombreCliente = new Regex("[^A-Za-z0-9 ]").Replace(nombreCliente, " ");
                string mensaje = string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};",
                    maquina.Posicion
                    , prepaquete.IdTarea.ToString().PadLeft(10, '0')
                    , prepaquete.CantidadFabricar.ToString().PadLeft(10, '0')
                    , prepaquete.Codigo.PadLeft(13)
                    , prepaquete.CodUtillaje.PadLeft(25)
                    , prepaquete.IdUtillajeTalla.PadLeft(10)
                    , prepaquete.Talla.PadLeft(10)
                    , prepaquete.CodigoEtiqueta.PadLeft(13)
                    , prepaquete.IdOrden.ToString().PadLeft(10, '0')
                    , prepaquete.IdOperacion.ToString().PadLeft(10, '0')
                    , nombreCliente.PadLeft(25)
                    , prepaquete.CodigoArticulo.PadLeft(20)
                    , prepaquete.Productividad.ToString().PadLeft(3)
                    , SesionManager.Sesion.Operario.Id.ToString().PadLeft(5));

                ClienteMQTT.Publicar(string.Format("/moldeado/plc/{0}/asociarTarea", maquina.IpAutomata.PadLeft(3)), mensaje, 1);

                maquina.CargarInformacion(new AsociacionTarea
                {
                    Cliente = prepaquete.NOMBRECLI,
                    CodigoArticulo = prepaquete.CodigoArticulo,
                    CodigoOrden = prepaquete.Codigo,
                    IdOperacion = prepaquete.IdOperacion,
                    IdOrden = prepaquete.IdOrden,
                    Utillaje = prepaquete.CodUtillaje,
                    TallaUtillaje = prepaquete.IdUtillajeTalla,
                    TallasArticulo = prepaquete.Tallas,
                    IdTarea = prepaquete.IdTarea ?? 0,
                    Pares = prepaquete.CantidadFabricar.HasValue ? (int)prepaquete.CantidadFabricar : 0,
                    CodigoEtiqueta = prepaquete.CodigoEtiqueta,
                    IdOperario = SesionManager.Sesion.Operario.Id,
                    ParesUtillaje = (int)prepaquete.Productividad,
                    Prensa = maquina.Posicion,
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
                if (e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.Down || e.Key == Key.Up)
                {

                    this.LayoutPrensas.MoverPrensa(e.Key);

                }
                else
                {
                    if (e.Key != Key.Return)
                    {
                        etiqueta += e.Key.ToString().Replace("D", "").Replace("NumPad", "");
                    }
                    else
                    {
                        //MessageBox.Show(etiqueta);
                        if (etiqueta.Length == 12)
                        {
                            this.fichajes.EtiquetaFichada(etiqueta);
                        }
                        etiqueta = "";
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
            //MessageBox.Show(e.CodigoBarquilla + " " + e.CodigoMaquina);
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
    }
}
