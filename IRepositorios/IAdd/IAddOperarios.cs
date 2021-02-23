using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRepositorios.IAdd
{
    public interface IAddOperarios<T>
    {
        T Insertar(T t);
    }
}
