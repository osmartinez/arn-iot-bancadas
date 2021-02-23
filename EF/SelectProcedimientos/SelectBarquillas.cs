using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF.SelectProcedimientos
{
    public class SelectBarquillas
    {
        public List<SP_BarquillaBuscarInformacionEnSeccion_Result> BuscarInformacionBarquilla(string codEtiqueta, string codSeccion)
        {
            using (SistemaGlobalPREEntities db = new SistemaGlobalPREEntities())
            {
                return db.SP_BarquillaBuscarInformacionEnSeccion(codEtiqueta, codSeccion).ToList();
            }
        }
    }
}
