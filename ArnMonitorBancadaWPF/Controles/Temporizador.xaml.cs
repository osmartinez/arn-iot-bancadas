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
    /// Lógica de interacción para Temporizador.xaml
    /// </summary>
    public partial class Temporizador : UserControl, INotifyPropertyChanged
    {
        private string tiempo = "";
        private int segundos = 0;
        public string Tiempo
        {
            get { return tiempo; }
            set { this.tiempo = value; Notifica("Tiempo"); }
        }
        private DispatcherTimer timer = new DispatcherTimer();
        public Temporizador()
        {
            InitializeComponent();
            this.DataContext = this;
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += Timer_Tick;
        }

        public void Reset()
        {
            this.timer.Stop();
            this.segundos = 0;
            this.timer.Start();
           
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            this.segundos++;
            int minutos = segundos / 60;
            int segundosSobrantes = segundos % 60;
            Tiempo = string.Format("{0}:{1}", minutos.ToString().PadLeft(2, '0'), segundosSobrantes.ToString().PadLeft(2, '0'));
            Notifica("Tiempo");
        }

        public event PropertyChangedEventHandler PropertyChanged;


        private void Notifica(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
