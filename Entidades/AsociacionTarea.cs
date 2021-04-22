using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class AsociacionTarea
    {
        public int Prensa { get; set; }
        public int IdTarea { get; set; }
        public int Pares { get; set; }
        public string CodigoOrden { get; set; }
        public string Utillaje { get; set; }
        public string TallaUtillaje { get; set; }
        public string TallasArticulo { get; set; }
        public string CodigoEtiqueta { get; set; }
        public int IdOrden { get; set; }
        public int IdOperacion { get; set; }
        public string Cliente { get; set; }
        public string CodigoArticulo { get; set; }
        public int ParesUtillaje { get; set; }
        public int IdOperario { get; set; }
        public double CantidadCaja { get; set; }
    }
}
