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
    
    public partial class MaquinasColasTrabajo
    {
        public int Id { get; set; }
        public int IdMaquina { get; set; }
        public int IdTarea { get; set; }
        public int Posicion { get; set; }
        public int Agrupacion { get; set; }
        public System.DateTime FechaProgramado { get; set; }
        public string PersonalPrograma { get; set; }
        public bool Ejecucion { get; set; }
        public Nullable<int> IdOperarioEjecuta { get; set; }
        public Nullable<int> IdOperarioPrograma { get; set; }
        public string CodigoEtiquetaFichada { get; set; }
        public double CantidadEtiquetaFichada { get; set; }
    
        public virtual Maquinas Maquinas { get; set; }
        public virtual Operarios Operarios { get; set; }
        public virtual Operarios Operarios1 { get; set; }
        public virtual OrdenesFabricacionOperacionesTallasCantidad OrdenesFabricacionOperacionesTallasCantidad { get; set; }
    }
}
