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
    
    public partial class Operarios
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Operarios()
        {
            this.MaquinasColasTrabajo = new HashSet<MaquinasColasTrabajo>();
            this.MaquinasColasTrabajo1 = new HashSet<MaquinasColasTrabajo>();
            this.OrdenesFabricacionProductos = new HashSet<OrdenesFabricacionProductos>();
        }
    
        public int Id { get; set; }
        public string CodigoObrero { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public bool EsResponsable { get; set; }
        public string CodigoEtiqueta { get; set; }
        public string Clave { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MaquinasColasTrabajo> MaquinasColasTrabajo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MaquinasColasTrabajo> MaquinasColasTrabajo1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrdenesFabricacionProductos> OrdenesFabricacionProductos { get; set; }
    }
}
