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
    
    public partial class SP_ObtenerOrdenesFabricacionPendientesMolde_Result
    {
        public string OrdenFabricacion { get; set; }
        public string Modelo { get; set; }
        public string CodUtillaje { get; set; }
        public Nullable<double> CantidadFabricar { get; set; }
        public string Descripcion { get; set; }
        public Nullable<double> Duracion { get; set; }
        public string CodigoArticulo { get; set; }
        public string Talla { get; set; }
        public string Tallas { get; set; }
        public string CodSeccion { get; set; }
        public string NombreSeccion { get; set; }
        public Nullable<int> ArticuloID { get; set; }
        public int OrdenFabricacionID { get; set; }
        public Nullable<int> IdPedido { get; set; }
        public Nullable<System.DateTime> FechaServicio { get; set; }
        public string Cliente { get; set; }
        public string NombreCliente { get; set; }
        public string Proceso { get; set; }
        public string NumeroOperacionAnterior { get; set; }
        public Nullable<int> TipoProceso { get; set; }
        public string Ciclo { get; set; }
        public string Operacion { get; set; }
        public Nullable<int> Revision { get; set; }
    }
}