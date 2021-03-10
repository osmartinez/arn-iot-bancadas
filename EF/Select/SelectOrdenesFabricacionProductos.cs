using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF.Select
{
    public class SelectOrdenesFabricacionProductos
    {
        public  List<OrdenesFabricacionProductos> PaquetesFabricadosOperario(int idOperario, DateTime fechaInicio, DateTime fechaFin)
        {
            using (SistemaGlobalPREEntities db = new SistemaGlobalPREEntities())
            {
                return db.OrdenesFabricacionProductos
                    .Include("OrdenesFabricacionOperacionesTallasCantidad.OrdenesFabricacionOperacionesTallas.OrdenesFabricacionOperaciones.OrdenesFabricacion")
                    .Where(x => x.IdOperario == idOperario &&
                (fechaInicio <= x.FechaCreacion && x.FechaCreacion <= fechaFin)).ToList();
            }
        }
    }
}
