using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF.Select
{
    public class SelectControl
    {
        public OperacionesControles BuscarControlOperacion(int idOfo, int idTipoMaquina)
        {
            using (SistemaGlobalPREEntities db = new SistemaGlobalPREEntities())
            {
                var ofo = db.OrdenesFabricacionOperaciones.Find(idOfo);
                if(ofo.IdOperacionMaestra == null)
                {
                    return OperacionesControles.Default;
                }
                var control = db.OperacionesControles.FirstOrDefault(x => x.IdOperacion == ofo.IdOperacionMaestra && x.IdTipoMaquina == idTipoMaquina);
                if (control == null)
                {
                    return OperacionesControles.Default;

                }
                else
                {
                    return control;
                }
            }
        }
    }
}
