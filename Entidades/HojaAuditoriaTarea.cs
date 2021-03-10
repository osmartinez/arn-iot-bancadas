using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class HojaAuditoriaTarea
    {
        public string Descripcion { get; set; }
        public int NumMoldes { get; set; }
        public int Pares { get; set; }
        public int ParesExceso { get; set; }

        public HojaAuditoriaTarea(List<OrdenesFabricacionProductos> paquetes)
        {
            var primero = paquetes.FirstOrDefault();
            NumMoldes = (int)primero.Cantidad;

            Descripcion = string.Format("{2}   {3} X {1} <{0}>",
                primero
                .OrdenesFabricacionOperacionesTallasCantidad
                .OrdenesFabricacionOperacionesTallas.Tallas,

                primero
                .OrdenesFabricacionOperacionesTallasCantidad
                .OrdenesFabricacionOperacionesTallas
                .OrdenesFabricacionOperaciones.CodUtillaje,

                primero
                .OrdenesFabricacionOperacionesTallasCantidad
                .OrdenesFabricacionOperacionesTallas
                .OrdenesFabricacionOperaciones
                .OrdenesFabricacion.Codigo,

                NumMoldes
                );
            Pares = (int)paquetes.Where(x => x.Tipo == "PLC").Sum(x => x.Cantidad);
            ParesExceso = (int)paquetes.Where(x => x.Tipo == "PLC-EXCESO").Sum(x => x.Cantidad);
        }
    }
}
