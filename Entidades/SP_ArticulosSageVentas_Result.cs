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
    
    public partial class SP_ArticulosSageVentas_Result
    {
        public int ID { get; set; }
        public string DescripcionArticulo { get; set; }
        public string CodigoArticulo { get; set; }
        public string Modelo { get; set; }
        public string UnidadMedidaFabricacion { get; set; }
        public string CodigoFamilia { get; set; }
        public string TipoArticulo { get; set; }
        public string CodigoSubfamilia { get; set; }
        public double Precio { get; set; }
        public bool Bloqueado { get; set; }
        public string CodigoCliente { get; set; }
        public string RazonSocial { get; set; }
        public string NombreCliente { get; set; }
        public string Observaciones { get; set; }
        public Nullable<bool> Certificado { get; set; }
        public Nullable<bool> ImportadoERP { get; set; }
        public short GrupoTallaActual { get; set; }
        public short GrupoTallaSage { get; set; }
        public short Colores { get; set; }
        public string DescripcionVariante { get; set; }
    }
}
