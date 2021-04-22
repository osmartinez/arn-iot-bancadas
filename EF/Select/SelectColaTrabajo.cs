using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entidades;
namespace EF.Select
{
    public static class SelectColaTrabajo
    {
        public static List<MaquinasColasTrabajo> ObtenerColaTrabajoMaquinaPorId(int idMaquina)
        {
            using (SistemaGlobalPREEntities db = new SistemaGlobalPREEntities())
            {
                var cola = db.MaquinasColasTrabajo
                    .Include("OrdenesFabricacionOperacionesTallasCantidad.OrdenesFabricacionOperacionesTallas.OrdenesFabricacionOperaciones.OrdenesFabricacion.Campos_ERP")
                    .Include("OrdenesFabricacionOperacionesTallasCantidad.OrdenesFabricacionProductos")
                    .Where(m => m.IdMaquina == idMaquina).ToList();
              

                return cola;
            }
        }
    }
}
