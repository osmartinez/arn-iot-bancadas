using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public partial class OperacionesControles
    {
        public List<int> idOfos = new List<int>();


        public static OperacionesControles Default
        {
            get
            {
                return new OperacionesControles
                {
                    IdOperacion = 0,
                    IdTipoMaquina = 0,
                    TiempoBaseEjecucion = 0,
                    TiempoInicio = 0,
                    TiempoUtillajeEjecucion = 0,
                };
            }
        }
    }
}
