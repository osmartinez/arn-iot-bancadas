using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public partial class MaquinasRegistrosDatos
    {
        public OrdenesFabricacionOperacionesTallasCantidad OrdenesFabricacionOperacionesTallasCantidad { get; set; }
        public int IdMaquina { get; set; }

        public DateTime FechaCreacion
        {
            get
            {
                return this.Fecha.ToLocalTime();
            }
        }
    }
}
