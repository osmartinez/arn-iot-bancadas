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
    
    public partial class SP_ArticulosRutasOperacionesOrden_Result
    {
        public Nullable<int> ID { get; set; }
        public string TallaUtillaje { get; set; }
        public string TallaArticulo { get; set; }
        public string NumeroOperacion { get; set; }
        public string NumeroOperacionAnterior { get; set; }
        public string NumeroOperacionSiguiente { get; set; }
        public string Descripcion { get; set; }
        public string Observaciones { get; set; }
        public string CodSeccion { get; set; }
        public string CodConexion { get; set; }
        public Nullable<bool> MostrarMaterias { get; set; }
        public string CodUtillaje { get; set; }
        public Nullable<decimal> TiempoPreparacion { get; set; }
        public Nullable<decimal> TiempoEjecucion { get; set; }
        public Nullable<decimal> TiempoOperario { get; set; }
        public Nullable<double> Eficiencia { get; set; }
        public Nullable<double> CosteTiempo { get; set; }
        public Nullable<double> CosteIndirectoInterno { get; set; }
        public Nullable<double> CosteOperacion { get; set; }
        public Nullable<double> CosteIndirectoSubcontrata { get; set; }
        public Nullable<bool> PosibilidadSubcontratar { get; set; }
        public Nullable<bool> PosibilidadInterna { get; set; }
        public Nullable<bool> PrioridadSubcontratar { get; set; }
        public Nullable<double> ParesHoraEstimados { get; set; }
        public Nullable<double> CosteHoraSubcontrata { get; set; }
        public Nullable<bool> CosteConfirmado { get; set; }
        public Nullable<int> TipoProceso { get; set; }
        public Nullable<int> IdOperacion { get; set; }
    }
}
