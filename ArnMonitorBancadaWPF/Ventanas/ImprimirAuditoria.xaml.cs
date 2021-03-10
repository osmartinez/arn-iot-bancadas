using EF.Select;
using Entidades;
using Entidades.Enum;
using MQTT;
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
using Turnos;

namespace ArnMonitorBancadaWPF.Ventanas
{
    /// <summary>
    /// Lógica de interacción para ImprimirAuditoria.xaml
    /// </summary>
    public partial class ImprimirAuditoria : Window
    {
        private SelectOrdenesFabricacionProductos selectProductos = new SelectOrdenesFabricacionProductos();
        public Operarios Operario { get; set; }
        public Turno Turno { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public HojaAuditoria HojaAuditoria { get; set; }

        public ImprimirAuditoria()
        {
            InitializeComponent();
            this.DataContext = this;
            Operario = SesionManager.Sesion.Operario;
            Turno = Horario.CalcularTurnoAFecha(DateTime.Now);

        }

        private void btNo_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void GenerarHojaAuditoria()
        {

            if (Operario != null)
            {
                DateTime fechaInicio;
                DateTime fechaFin;
               Horario.CalcularHorarioTurno(Turno, Fecha, out fechaInicio, out fechaFin);

                var paquetes = selectProductos.PaquetesFabricadosOperario(Operario.Id, fechaInicio, fechaFin);

                this.HojaAuditoria = new HojaAuditoria(Operario, paquetes, Util.Store.Bancada.Maquinas.ToList());
            }
        }

        private void btImprimirHoja_Click(object sender, RoutedEventArgs e)
        {
            //GenerarHojaAuditoria();
            ClienteMQTT.Publicar(string.Format("/operario/{0}/imprimir/hojaProduccion", Operario.Id),"",1);
            this.Close();
        }
    }
}
