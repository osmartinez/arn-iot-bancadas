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

namespace ArnMonitorBancadaWPF.Controles
{
    /// <summary>
    /// Lógica de interacción para PrensaLayout.xaml
    /// </summary>
    public partial class PrensaLayout : UserControl,INotifyPropertyChanged
    {
        public Maquinas Maquina { get; set; }
        private LocalPrensa prensa;

        public event PropertyChangedEventHandler PropertyChanged;

        public string NombrePrensa
        {
            get
            {
                return (prensa == null) ? "" : (
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
            this.prensa = prensa;
            Grid.SetRow(this, (int)prensa.Top);
            Grid.SetColumn(this, (int)prensa.Left);
            Maquina.OnInfoEjecucionActualizada += Maquina_OnInfoEjecucionActualizada;

           

            /*this.MouseLeftButtonDown += PrensaLayout_MouseLeftButtonDown;
            this.MouseLeftButtonUp += PrensaLayout_MouseLeftButtonUp;
            this.MouseMove += PrensaLayout_MouseMove;*/
        }

        public void Notifica(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Maquina_OnInfoEjecucionActualizada(object sender, EventArgs e)
        {
            this.Notifica();
        }
    }
}
