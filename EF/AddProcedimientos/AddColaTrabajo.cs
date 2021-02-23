using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF.AddProcedimientos
{
    public class AddColaTrabajo
    {
        public void ActualizarColaTrabajo(string idsTareas, int? agrupacion,int idMaquina, int idOperario)
        {
            using (SistemaGlobalPREEntities db = new SistemaGlobalPREEntities())
            {
                db.SP_MaquinaAsignarTarea(idsTareas, idMaquina, agrupacion, idOperario);
            }
        }
    }
}
