using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArnMonitorBancadaWPF.Util
{
    public class EventoFichajeAsociacion: EventArgs
    {
        public string CodigoMaquina { get; set; }
        public string CodigoBarquilla { get; set; }

        public EventoFichajeAsociacion(string codigoMaquina, string codigoBarquilla)
        {
            CodigoMaquina = codigoMaquina;
            CodigoBarquilla = codigoBarquilla;
        }
    }
}
