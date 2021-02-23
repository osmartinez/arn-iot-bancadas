using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF.Select
{
    public class SelectBancada
    {
        public Bancadas BuscarPorId(int id)
        {
            using (SistemaGlobalPREEntities db = new SistemaGlobalPREEntities())
            {
                return db.Bancadas
                    .Include("Maquinas")
                    .FirstOrDefault(x=>x.ID == id);
            }
        }
    }
}
