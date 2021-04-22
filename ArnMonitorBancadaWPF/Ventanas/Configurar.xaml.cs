using ConfiguracionLocal;
using Entidades;
using Newtonsoft.Json;
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

namespace ArnMonitorBancadaWPF.Ventanas
{
    /// <summary>
    /// Lógica de interacción para Configurar.xaml
    /// </summary>
    public partial class Configurar : Window
    {
        private FicheroConfiguracion fc = new FicheroConfiguracion();
        public string Contenido { get; set; } = "";
        public Configurar()
        {
            InitializeComponent();
            this.DataContext = this;
            try
            {
                var config = fc.LeerConfiguracion();
                Contenido = JsonConvert.SerializeObject(config);
                this.Closing += Configurar_Closing;
            }
            catch (Exception ex)
            {
                Logs.Log.Write(ex);
            }

        }

        private void Configurar_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                Configuracion config = JsonConvert.DeserializeObject<Configuracion>(Contenido);
                fc.GuardarConfiguracion(config);
                System.Windows.Forms.Application.Restart();
                System.Windows.Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                Logs.Log.Write(ex);
            }
        }
    }
}
