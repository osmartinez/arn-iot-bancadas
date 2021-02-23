using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF.Select
{
    public class SelectMaquinasRegistroDatos
    {
        public  List<MaquinasRegistrosDatos> HistoricoPaquetesOperario(int idOperario, DateTime fechaInicio, DateTime fechaFin)
        {
            fechaInicio = fechaInicio.ToUniversalTime();
            fechaFin = fechaFin.ToUniversalTime();
            using (SistemaGlobalPREEntities db = new SistemaGlobalPREEntities())
            {
                var registros =
                db.MaquinasRegistrosDatos
                    .Where(x => x.IdOperario == idOperario &&
                (fechaInicio <= x.Fecha && x.Fecha <= fechaFin)).ToList();

                foreach (var registro in registros)
                {
                    if (registro.IdTarea != 0)
                    {
                        registro.OrdenesFabricacionOperacionesTallasCantidad = db.OrdenesFabricacionOperacionesTallasCantidad
                            .Include("OrdenesFabricacionOperacionesTallas.OrdenesFabricacionOperaciones.OrdenesFabricacion")
                            .FirstOrDefault(x => x.ID == registro.IdTarea);
                    }
                }

                return registros;
            }
        }
    }
}
