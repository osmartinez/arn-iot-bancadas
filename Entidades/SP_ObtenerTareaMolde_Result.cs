//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Entidades
{
    using System;
    
    public partial class SP_ObtenerTareaMolde_Result
    {
        public int ID { get; set; }
        public string OrdenFabricacion { get; set; }
        public Nullable<int> Prioridad { get; set; }
        public string Molde { get; set; }
        public string Seccion { get; set; }
        public string Talla { get; set; }
        public string Proceso { get; set; }
        public Nullable<double> Cantidad { get; set; }
        public Nullable<int> Inicio { get; set; }
        public Nullable<decimal> TiempoEjecucion { get; set; }
        public int MaquinaID { get; set; }
        public Nullable<decimal> TiempoPreparacion { get; set; }
        public Nullable<decimal> TiempoOperario { get; set; }
        public Nullable<decimal> TiempoDesplazamientoBancada { get; set; }
        public string TiempoBase { get; set; }
    }
}
