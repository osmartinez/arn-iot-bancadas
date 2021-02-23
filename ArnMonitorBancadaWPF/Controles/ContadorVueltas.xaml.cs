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

namespace ArnMonitorBancadaWPF.Controles
{
    /// <summary>
    /// Lógica de interacción para ContadorVueltas.xaml
    /// </summary>
    public partial class ContadorVueltas : UserControl,INotifyPropertyChanged
    {
        private int vueltas = 0;
        public int Vueltas
        {
            get { return vueltas; }
            set { this.vueltas = value; Notifica("Vueltas"); }
        }
        public ContadorVueltas()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;


        private void Notifica(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
