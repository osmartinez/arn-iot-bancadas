using Entidades.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades.Eventos
{
    public class ModoMaquinaCambioEventArgs:EventArgs
    {
        public ModoMaquina Modo { get; private set; }

        public ModoMaquinaCambioEventArgs(ModoMaquina modo)
        {
            Modo = modo;
        }
    }
}
