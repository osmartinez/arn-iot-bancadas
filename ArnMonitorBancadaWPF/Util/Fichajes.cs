using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ArnMonitorBancadaWPF.Util
{
    public class Fichajes
    {
        public string UltimaEtiqueta { get; set; } = "";
        public string CodigoBarquilla { get; set; } = "";
        public string CodigoMaquina { get; set; } = "";

        public event EventHandler<EventoFichajeAsociacion> OnFichajeAsociacion;
        private DispatcherTimer timer = new DispatcherTimer();

        public Fichajes()
        {
            this.timer.Interval = new TimeSpan(0, 0, 10);
            this.timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (EsEtiquetaMaquina(UltimaEtiqueta) && CodigoBarquilla == "")
            {
                Limpiar();
            }
            this.timer.Stop();
        }

        private void Limpiar()
        {
            this.CodigoBarquilla = "";
            this.CodigoMaquina = "";
            this.UltimaEtiqueta = "";
        }

        private void FichajeAsociacion()
        {
            if (OnFichajeAsociacion != null)
            {
                OnFichajeAsociacion(this, new EventoFichajeAsociacion(this.CodigoMaquina, this.CodigoBarquilla));
            }
            this.Limpiar();
        }

        private bool EsEtiquetaMaquina(string cod)
        {
            return cod.StartsWith("02") || cod.StartsWith("2");
        }

        private bool EsEtiquetaBarquilla(string cod)
        {
            return cod.StartsWith("05") || cod.StartsWith("5");
        }

        public void EtiquetaFichada(string cod)
        {
            if (cod[0] == '5')
            {
                // barquilla
                BarquillaFichada("0" + cod);
            }
            else if (cod[0] == '2')
            {
                // maquina
                MaquinaFichada("0" + cod);

            }
        }

        private void BarquillaFichada(string cod)
        {
            // cod es barquilla

            if (EsEtiquetaBarquilla(UltimaEtiqueta))
            {
                CodigoMaquina = "";
                CodigoBarquilla = "";
                UltimaEtiqueta = "";
            }
            else if (EsEtiquetaMaquina(UltimaEtiqueta))
            {
                this.CodigoBarquilla = cod;
                this.UltimaEtiqueta = CodigoBarquilla;
                FichajeAsociacion();
            }
        }

        private void MaquinaFichada(string cod)
        {
            this.CodigoBarquilla = "";
            this.CodigoMaquina = cod;
            this.UltimaEtiqueta = cod;

            timer.Start();
        }
    }
}
