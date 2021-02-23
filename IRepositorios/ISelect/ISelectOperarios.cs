using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRepositorios.ISelect
{
    public interface ISelectOperarios<T>
    {
        T BuscarPorCodigo(string cod);
    }
}
