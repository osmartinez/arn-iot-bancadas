using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ArnMonitorBancadaWPF.Ventanas
{
    /// <summary>
    /// Lógica de interacción para Bienvenida.xaml
    /// </summary>
    public partial class Bienvenida : Window
    {
        public Entidades.Operarios Operario { get; set; }
        private DispatcherTimer timer = new DispatcherTimer();
        public Bienvenida()
        {
            InitializeComponent();
            this.DataContext = this;
            Operario = SesionManager.Sesion.Operario;

            timer.Interval = new TimeSpan(0, 0, 4);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
