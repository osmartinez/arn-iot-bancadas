using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class HojaProduccion
    {
        public Operarios Operario { get; set; }
        public ObservableCollection<ClaveValores> Datos { get; set; } = new ObservableCollection<ClaveValores>();
        public DateTime FechaGeneracion { get; set; } = DateTime.Now;

    }
}
