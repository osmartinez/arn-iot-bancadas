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
    
    public partial class Utillajes
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Utillajes()
        {
            this.OrdenesFabricacionOperaciones = new HashSet<OrdenesFabricacionOperaciones>();
            this.UtillajesTallas = new HashSet<UtillajesTallas>();
        }
    
        public string CodUtillaje { get; set; }
        public string Descripcion { get; set; }
        public string Observaciones { get; set; }
        public string Modelo { get; set; }
        public string Cliente { get; set; }
        public string CodSeccion { get; set; }
        public Nullable<int> Estado { get; set; }
        public bool Habilitado { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrdenesFabricacionOperaciones> OrdenesFabricacionOperaciones { get; set; }
        public virtual Secciones Secciones { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UtillajesTallas> UtillajesTallas { get; set; }
    }
}
