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
    
    public partial class SP_BuscarTodasMaquinasEnSeccion_Result
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string CodigoAgrupacion { get; set; }
        public string CodSeccion { get; set; }
        public Nullable<int> IDPlantillaCalendario { get; set; }
        public Nullable<System.DateTime> PlantillaCalendarioDesde { get; set; }
        public Nullable<bool> Averia { get; set; }
        public Nullable<int> IdIncidencia { get; set; }
        public Nullable<bool> IncluirTiempoPreparacion { get; set; }
        public Nullable<int> IdTaller { get; set; }
        public Nullable<int> TallerHorasCapacidadDiaria { get; set; }
        public Nullable<System.DateTime> FechaBorrado { get; set; }
        public string UsuarioBorrado { get; set; }
        public Nullable<double> CorrectorCapacidad { get; set; }
        public string CodUbicacion { get; set; }
        public string CodigoEtiqueta { get; set; }
        public Nullable<int> IdBancada { get; set; }
        public string IpAutomata { get; set; }
        public int Posicion { get; set; }
        public double UITopMargin { get; set; }
        public double UILeftMargin { get; set; }
        public Nullable<int> PosicionGlobal { get; set; }
        public Nullable<int> IdTipo { get; set; }
    }
}
