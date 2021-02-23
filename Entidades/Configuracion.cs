using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class BancadaConfig
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        
    }
    public class Configuracion
    {
        public BancadaConfig Bancada { get; set; }

        public static Configuracion Default
        {
            get
            {
                return new Configuracion
                {
                    Bancada = new BancadaConfig { Id = 5, Nombre = "MODULO 100A" },
                };
            }
        }
    }
}
