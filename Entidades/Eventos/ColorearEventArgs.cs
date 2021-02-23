using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades.Eventos
{
    public class ColorearEventArgs : EventArgs
    {
        public string Color { get; set; }

        public ColorearEventArgs(string color)
        {
            Color = color;
        }
    }
}
