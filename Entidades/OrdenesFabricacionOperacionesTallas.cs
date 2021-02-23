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
    
    public partial class OrdenesFabricacionOperacionesTallas
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OrdenesFabricacionOperacionesTallas()
        {
            this.OrdenesFabricacionOperacionesTallasCantidad = new HashSet<OrdenesFabricacionOperacionesTallasCantidad>();
        }
    
        public int ID { get; set; }
        public string NumeroOperacion { get; set; }
        public int IdOrdenFabricacionOperacion { get; set; }
        public string IdUtillajeTalla { get; set; }
        public string NumeroOperacionAnterior { get; set; }
        public string NumeroOperacionSiguiente { get; set; }
        public string Tallas { get; set; }
        public Nullable<double> ProductividadCalculada { get; set; }
        public Nullable<double> ProductividadTallaUtillaje { get; set; }
    
        public virtual OrdenesFabricacionOperaciones OrdenesFabricacionOperaciones { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrdenesFabricacionOperacionesTallasCantidad> OrdenesFabricacionOperacionesTallasCantidad { get; set; }
    }
}