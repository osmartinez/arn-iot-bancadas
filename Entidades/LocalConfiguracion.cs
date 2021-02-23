using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class LocalConfiguracion
    {
        public List<LocalOperario> Operarios { get; set; } = new List<LocalOperario>();
        public List<LocalPrensa> Prensas { get; set; } = new List<LocalPrensa>();

        public static LocalConfiguracion Default
        {
            get
            {
                return new LocalConfiguracion
                {
                    Operarios = new List<LocalOperario>(),
                    Prensas = new List<LocalPrensa>(),
                };
            }
        }
    }
}
