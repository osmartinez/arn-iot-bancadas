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
    
    public partial class SP_ObtenerDatoLineasAlbaranProveedorMateriales_Result
    {
        public string CodigoProveedor { get; set; }
        public string RazonSocial { get; set; }
        public Nullable<System.DateTime> FechaFactura { get; set; }
        public short CodigoEmpresa { get; set; }
        public short EjercicioAlbaran { get; set; }
        public string SerieAlbaran { get; set; }
        public int NumeroAlbaran { get; set; }
        public short Orden { get; set; }
        public System.Guid LineasPosicion { get; set; }
        public System.Guid LineaPedido { get; set; }
        public System.DateTime FechaRegistro { get; set; }
        public System.DateTime FechaAlbaran { get; set; }
        public string CodigoArticulo { get; set; }
        public string DescripcionArticulo { get; set; }
        public string DescripcionLinea { get; set; }
        public string UnidadMedida1_ { get; set; }
        public decimal C_Iva { get; set; }
        public short EjercicioPedido { get; set; }
        public string SeriePedido { get; set; }
        public int NumeroPedido { get; set; }
        public short EjercicioFactura { get; set; }
        public string SerieFactura { get; set; }
        public int NumeroFactura { get; set; }
        public decimal Unidades { get; set; }
        public decimal Precio { get; set; }
        public decimal PrecioRebaje { get; set; }
        public decimal ImporteBruto { get; set; }
        public decimal ImporteNeto { get; set; }
        public decimal ImporteParcial { get; set; }
        public decimal BaseImponible { get; set; }
        public decimal BaseIva { get; set; }
        public decimal CuotaIva { get; set; }
        public decimal TotalIva { get; set; }
        public decimal ImporteLiquido { get; set; }
        public string TipoArticulo { get; set; }
    }
}
