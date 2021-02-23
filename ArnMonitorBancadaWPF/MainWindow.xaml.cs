using ArnMonitorBancadaWPF.Util;
using Entidades;
using Entidades.Enum;
using MQTT;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        private string etiqueta = "";
        private Fichajes fichajes = new Fichajes();
        private Queue<EventoFichajeAsociacion> colaEventosFichaje = new Queue<EventoFichajeAsociacion>();
        private DispatcherTimer timerEventoFichaje = new DispatcherTimer();
        private DispatcherTimer timerNumVueltas = new DispatcherTimer();
        public Operarios Operario { get; set; } = new Operarios { Nombre = "<SIN OPERARIO>" };

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            if (SesionManager.Sesion.Operario == null)
            {
                Ventanas.Login login = new Ventanas.Login();
                login.ShowDialog();
                this.Operario = SesionManager.Sesion.Operario;
                Notificar("Operario");
            }

            ClienteMQTT.Iniciar();
            this.Closing += (s, e) => { ClienteMQTT.Cerrar(); };

            this.fichajes.OnFichajeAsociacion += Fichajes_OnFichajeAsociacion;

            this.timerEventoFichaje.Interval = new TimeSpan(0, 0, 5);
            this.timerEventoFichaje.Tick += TimerEventoFichaje_Tick;
            this.timerEventoFichaje.Start();

            this.timerNumVueltas.Interval = new TimeSpan(0, 0, 10);
            this.timerNumVueltas.Tick += TimerNumVueltas_Tick; ;
            this.timerNumVueltas.Start();

            this.PreviewKeyUp += MainWindow_PreviewKeyUp;

            //ClienteMQTT.Topics[0].Callbacks.Add(TareaAsociada);
            ClienteMQTT.Topics[1].Callbacks.Add(Normal);

            CargarPaquetesPrevios();
        }

        private void CargarPaquetesPrevios()
        {
            DateTime ahora = DateTime.Now;
            Operarios op = SesionManager.Sesion.Operario;
            Turno turno = Horario.CalcularTurnoAFecha(ahora);
            DateTime fechaInicio;
            DateTime fechaFin;
            Horario.CalcularHorarioTurno(turno, ahora, out fechaInicio, out fechaFin);

            var paquetes = selectRegistroDatos.HistoricoPaquetesOperario(op.Id, fechaInicio, fechaFin);

            foreach(var paquete in paquetes.Where(x=>x.PiezaIntroducida))
            {
                var maq = Store.Bancada.Maquinas.FirstOrDefault(x => x.IpAutomata == paquete.IpAutomata && x.Posicion == paquete.PosicionMaquina);
                if(maq != null)
                {
                    maq.Pulsos.Add(new PulsoMaquina
                    {
                        Ciclo = paquete.Ciclo,
                        Fecha = paquete.FechaCreacion,
                        Pares = paquete.Pares,
                    });
                }
            }
        }

        private void TimerNumVueltas_Tick(object sender, EventArgs e)
        {
            int maxVueltas= Store.Bancada.Maquinas.Max(x => x.Pulsos.Count);
            int pares = Store.Bancada.Maquinas.Sum(x => x.Pulsos.Sum(y => y.Pares));
            this.contadorVueltas.Vueltas = maxVueltas;
            this.contadorPares.Pares = pares;
        }

        private void Normal(string msg, string topicRecibido, Topic topic)
        {
            ConsumoPrensa consumo = JsonConvert.DeserializeObject<ConsumoPrensa>(msg);

            if (consumo.HoraLocal != DateTime.MinValue)
            {

            }

            string tipoBancada = topicRecibido.Split('/')[topic.IndiceTipoBancada].Trim();
            string ipPlc = topicRecibido.Split('/')[topic.IndiceIdTopic].Trim();
            consumo.IpPlc = ipPlc;

            Maquinas maquina = Store.Bancada.Maquinas.FirstOrDefault(x => x.IpAutomata == ipPlc && x.Posicion == consumo.Prensa);

            if (maquina != null)
            {
                maquina.CargarInformacion(consumo);
                maquina.Pulsos.Add(new PulsoMaquina { Ciclo = consumo.SgCiclo, Fecha = consumo.HoraLocal, Pares = consumo.ParesUtillaje * consumo.NumMoldes });
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

        private void TimerEventoFichaje_Tick(object sender, EventArgs e)
        {
            if (colaEventosFichaje.Count == 0) return;
            EventoFichajeAsociacion evento = colaEventosFichaje.Dequeue();
            if (evento != null)
            {
                try
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

                                // bd
                                addColaTrabajo.ActualizarColaTrabajo(idsTareasStr, infoBarquillaSeccion.First().Agrupacion??0, maquina.ID, SesionManager.Sesion.Operario.Id);

                                // mqtt
                                MqttAsociarBarquilla(infoBarquillaSeccion.First(), maquina);
                            }
                        }

                    }
                }catch(Exception ex)
                {
                    //MessageBox.Show(ex.Message);
                    colaEventosFichaje.Enqueue(evento);
                }
            }
        }

        private void MqttAsociarBarquilla(SP_BarquillaBuscarInformacionEnSeccion_Result prepaquete, Maquinas maquina)
        {
            string nombreCliente = prepaquete.NOMBRECLI;
            if (nombreCliente.Length > 25)
            {
                nombreCliente = nombreCliente.Substring(0, 24);
            }
            nombreCliente = new Regex("[^A-Za-z0-9 ]").Replace(nombreCliente, " ");
            string mensaje = string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};",
                maquina.Posicion
                , prepaquete.IdTarea.ToString().PadLeft(10,'0')
                , prepaquete.CantidadFabricar.ToString().PadLeft(10,'0')
                , prepaquete.Codigo.PadLeft(13)
                , prepaquete.CodUtillaje.PadLeft(25)
                , prepaquete.IdUtillajeTalla.PadLeft(10)
                , prepaquete.Talla.PadLeft(10)
                , prepaquete.CodigoEtiqueta.PadLeft(13)
                , prepaquete.IdOrden.ToString().PadLeft(10,'0')
                , prepaquete.IdOperacion.ToString().PadLeft(10, '0')
                , nombreCliente.PadLeft(25)
                , prepaquete.CodigoArticulo.PadLeft(20)
                , prepaquete.Productividad.ToString().PadLeft(3)
                , SesionManager.Sesion.Operario.Id.ToString().PadLeft(5));

            ClienteMQTT.Publicar(string.Format("/moldeado/plc/{0}/asociarTarea",maquina.IpAutomata.PadLeft(3)), mensaje);

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
                Pares = prepaquete.CantidadFabricar.HasValue?(int)prepaquete.CantidadFabricar: 0,
                CodigoEtiqueta = prepaquete.CodigoEtiqueta,
                IdOperario = SesionManager.Sesion.Operario.Id,
                ParesUtillaje = (int)prepaquete.Productividad,
                Prensa = maquina.Posicion,
            });
        }


        private void MainWindow_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return)
            {
                etiqueta += e.Key.ToString().Replace("D", "").Replace("NumPad","");
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

        private void Fichajes_OnFichajeAsociacion(object sender, EventoFichajeAsociacion e)
        {
            //MessageBox.Show(e.CodigoBarquilla + " " + e.CodigoMaquina);
            this.colaEventosFichaje.Enqueue(e);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void Notificar(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void BtLogout_Click(object sender, RoutedEventArgs e)
        {
            if (InputManager.Current.MostRecentInputDevice is KeyboardDevice)
            {
                e.Handled = true;
                return;
            }

            ClienteMQTT.Cerrar();
            SesionManager.Sesion.Operario = null;
            Ventanas.Login login = new Ventanas.Login();
            login.ShowDialog();
            this.Operario = SesionManager.Sesion.Operario;
            ClienteMQTT.Iniciar();
            Notificar("Operario");
        }
    }
}
