using Entidades;
using IRepositorios.ISelect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF.Select
{
    public class SelectOperarios: ISelectOperarios<Operarios>
    {
        public Operarios BuscarPorCodigo(string codigo)
        {
            using(SistemaGlobalPREEntities db = new SistemaGlobalPREEntities())
            {
                return db.Operarios.FirstOrDefault(x => x.CodigoObrero.Contains(codigo));
            }
        }
    }
}
