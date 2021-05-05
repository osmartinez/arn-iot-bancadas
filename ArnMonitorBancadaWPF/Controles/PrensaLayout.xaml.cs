using Entidades;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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

namespace ArnMonitorBancadaWPF.Controles
{
    /// <summary>
    /// Lógica de interacción para PrensaLayout.xaml
    /// </summary>
    public partial class PrensaLayout : UserControl, INotifyPropertyChanged
    {
        public Maquinas Maquina { get; set; }
        public LocalPrensa Prensa { get; private set; }
        private DispatcherTimer timerCalentamiento;
        private DispatcherTimer timerInactividad;
        private DispatcherTimer timerLimiteCaja;
        private bool inactiva = false;


        public event PropertyChangedEventHandler PropertyChanged;

        public string NombrePrensa
        {
            get
            {
                return (Prensa == null) ? "" : (
                    string.Format("PRENSA {0} - {1}<{2}>",
                    Maquina.Nombre
                    .Replace("MOLDE ESPUMA ", "")
                    .Replace("MOLDE PEGADO ", ""), Maquina.Utillaje, Maquina.TallaUtillaje)
                    ); ;
            }
        }
        public PrensaLayout(LocalPrensa prensa, Maquinas maquina)
        {
            InitializeComponent();
            this.DataContext = this;
            this.Maquina = maquina;
            this.Prensa = prensa;
            this.MinHeight = 30;
            Grid.SetRow(this, (int)prensa.Top);
            Grid.SetColumn(this, (int)prensa.Left);
            Maquina.OnParesConsumidos += Maquina_OnParesConsumidos;
            Maquina.OnInfoEjecucionActualizada += Maquina_OnInfoEjecucionActualizada;
            Maquina.OnModoCambiado += Maquina_OnModoCambiado;
            this.PreviewMouseUp += PrensaLayout_PreviewMouseUp;

            this.timerCalentamiento = new DispatcherTimer();
            this.timerCalentamiento.Interval = new TimeSpan(0, 0, 0, 0, 400);
            this.timerCalentamiento.Tick += TimerCalentamiento_Tick; ;
            this.timerInactividad = new DispatcherTimer();
            this.timerInactividad.Interval = new TimeSpan(0, 10, 0);
            this.timerInactividad.Tick += TimerInactividad_Tick;
            this.timerLimiteCaja = new DispatcherTimer();
            this.timerLimiteCaja.Interval = new TimeSpan(0, 0, 10);
            this.timerLimiteCaja.Tick += TimerLimiteCaja_Tick;

            this.timerLimiteCaja.Start();
            this.timerInactividad.Start();
        }

        private void Maquina_OnParesConsumidos(object sender, EventArgs e)
        {
            this.Notifica();
            this.inactiva = false;
            this.timerInactividad.Stop(); this.timerInactividad.Start();
            if (this.Maquina.Modo != Entidades.Enum.ModoMaquina.Calentamiento)
            {
                PonerColorFrio();
            }
        }

        private void TimerLimiteCaja_Tick(object sender, EventArgs e)
        {
            if(this.Maquina.Modo == Entidades.Enum.ModoMaquina.Normal &&
                this.Maquina.CantidadCaja <= this.Maquina.CantidadCajaRealizada && !inactiva)
            {
                PonerColorError();
            }
        }

        private void TimerInactividad_Tick(object sender, EventArgs e)
        {
            if (this.Maquina.Modo != Entidades.Enum.ModoMaquina.Calentamiento)
            {
                PonerColorInactivo();
                inactiva = true;
            }
        }

        private void PrensaLayout_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            (((this.Parent as Grid).Parent as ScrollViewer).Parent as LayoutPrensas).PrensaSeleccionada = this;
        }

        private void TimerCalentamiento_Tick(object sender, EventArgs e)
        {
            /**
             * la idea es que si esta calentando:
             *  - si no ha alcanzado la tepmeratura -> naranja fijo
             *  - si ha alcanzado la temperatura -> naranja parpadeo
             */
            try
            {
                this.Dispatcher.BeginInvoke((Action)(() =>
                {
                    if (this.Maquina.TemperaturaOK)
                    {
                        if (this.Border.Background == Brushes.White)
                        {
                            PonerColorCaliente();
                        }
                        else
                        {
                            PonerColorFrio();
                        }
                    }
                    else
                    {
                        PonerColorCaliente();
                    }

                }));
            }
            catch (Exception ex)
            {
                Logs.Log.Write(ex);
            }
        }

        private void PonerColorCaliente()
        {
            try
            {
                this.Dispatcher.BeginInvoke((Action)(() =>
                {
                    this.Border.Background = Brushes.Orange;

                }));
            }
            catch (Exception ex)
            {
                Logs.Log.Write(ex);
            }

        }

        private void PonerColorFrio()
        {
            try
            {
                this.Dispatcher.BeginInvoke((Action)(() =>
                {
                    this.Border.Background = Brushes.White;
                }));
            }
            catch (Exception ex)
            {
                Logs.Log.Write(ex);
            }

        }

        private void PonerColorInactivo()
        {
            try
            {
                this.Dispatcher.BeginInvoke((Action)(() =>
                {
                    this.Border.Background = Brushes.Gray;
                }));
            }
            catch (Exception ex)
            {
                Logs.Log.Write(ex);
            }
        }

        private void PonerColorError()
        {
            try
            {
                this.Dispatcher.BeginInvoke((Action)(() =>
                {
                    this.Border.Background = Brushes.Red;
                }));
            }
            catch (Exception ex)
            {
                Logs.Log.Write(ex);
            }
        }



        private void Maquina_OnModoCambiado(object sender, Entidades.Eventos.ModoMaquinaCambioEventArgs e)
        {
            if (e.Modo == Entidades.Enum.ModoMaquina.Calentamiento)
            {
                timerCalentamiento.Start();
            }
            else
            {
                timerCalentamiento.Stop();
                PonerColorFrio();
            }
        }

        public void Notifica(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Maquina_OnInfoEjecucionActualizada(object sender, EventArgs e)
        {
            this.Notifica();
            this.inactiva = false; ;
            this.timerInactividad.Stop(); this.timerInactividad.Start();
            if (this.Maquina.Modo != Entidades.Enum.ModoMaquina.Calentamiento)
            {
                PonerColorFrio();
            }
        }
    }
}
