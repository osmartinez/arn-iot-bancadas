using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class LocalPrensa
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string IpAutomata { get; set; }
        public int Posicion { get; set; }
        public double Left { get; set; }
        public double Top { get; set; }
    }
}
