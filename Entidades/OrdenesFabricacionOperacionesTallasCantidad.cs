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
    
    public partial class OrdenesFabricacionOperacionesTallasCantidad
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OrdenesFabricacionOperacionesTallasCantidad()
        {
            this.MaquinasColasTrabajo = new HashSet<MaquinasColasTrabajo>();
            this.OrdenesFabricacionProductos = new HashSet<OrdenesFabricacionProductos>();
        }
    
        public int ID { get; set; }
        public int IdOrdenFabricacionOperacionesTallas { get; set; }
        public Nullable<double> CantidadFabricar { get; set; }
        public Nullable<double> CantidadProducida { get; set; }
        public Nullable<double> CantidadSaldos { get; set; }
        public Nullable<double> CantidadDefectuosa { get; set; }
        public Nullable<int> IdMaquina { get; set; }
        public Nullable<int> IdBancada { get; set; }
        public Nullable<int> Posicion { get; set; }
        public Nullable<int> IdEstado { get; set; }
        public Nullable<int> IdIncidencia { get; set; }
        public Nullable<bool> Finalizado { get; set; }
        public Nullable<int> IdEstadoAnterior { get; set; }
    
        public virtual Bancadas Bancadas { get; set; }
        public virtual Maquinas Maquinas { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MaquinasColasTrabajo> MaquinasColasTrabajo { get; set; }
        public virtual OrdenesFabricacionOperacionesTallas OrdenesFabricacionOperacionesTallas { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrdenesFabricacionProductos> OrdenesFabricacionProductos { get; set; }
    }
}
