using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class HojaAuditoriaPrensa
    {
        public string Nombre { get; set; }
        public int Pares { get; set; }

        public int ParesExceso { get; set; }

        public List<HojaAuditoriaTarea> Tareas = new List<HojaAuditoriaTarea>();
        public HojaAuditoriaPrensa(Maquinas maq, List<OrdenesFabricacionProductos> paquetes)
        {
            Nombre = string.Format("PRENSA {0}", maq.PosicionGlobal.ToString().PadLeft(2, '0'));
            Pares = (int)paquetes.Where(x=>x.Tipo == "PLC").Sum(x => x.Cantidad);
            ParesExceso = (int)paquetes.Where(x => x.Tipo == "PLC-EXCESO").Sum(x => x.Cantidad);

            var agrupados = paquetes.GroupBy(x => new { x.OrdenesFabricacionOperacionesTallasCantidad.ID, x.Cantidad });

            foreach(var grupo in agrupados)
            {
                Tareas.Add(new HojaAuditoriaTarea(grupo.ToList()));
            }
        }
    }
}
