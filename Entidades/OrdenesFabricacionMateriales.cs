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
    using System.Collections.Generic;
    
    public partial class OrdenesFabricacionMateriales
    {
        public int ID { get; set; }
        public int IdOrdenFabricacion { get; set; }
        public int IdArticulo { get; set; }
        public string CodMateria { get; set; }
        public string ReferenciaERP { get; set; }
        public Nullable<bool> ArticuloInterno { get; set; }
        public string Descripcion { get; set; }
        public string Observaciones { get; set; }
        public string UnidadMedida { get; set; }
        public Nullable<double> Cantidad { get; set; }
        public Nullable<double> Precio { get; set; }
        public Nullable<double> CosteIndirecto { get; set; }
        public string CodConexion { get; set; }
        public Nullable<bool> MostrarEnOperaciones { get; set; }
        public Nullable<bool> FabricarStock { get; set; }
    
        public virtual Articulos Articulos { get; set; }
        public virtual OrdenesFabricacion OrdenesFabricacion { get; set; }
    }
}
