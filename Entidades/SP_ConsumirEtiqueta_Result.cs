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
    
    public partial class SP_ConsumirEtiqueta_Result
    {
        public Nullable<int> IdOrden { get; set; }
        public string NombreMaquina { get; set; }
        public string CodigoMaquina { get; set; }
        public Nullable<int> IdMaquina { get; set; }
        public string CodigoOrden { get; set; }
        public string Cliente { get; set; }
        public string Modelo { get; set; }
        public string CodPrepaquete { get; set; }
        public string CodPrepaqueteAgrupacion { get; set; }
        public string Talla { get; set; }
        public string TallaUtillaje { get; set; }
        public Nullable<double> CantidadPaquete { get; set; }
        public Nullable<double> CantidadFabricar { get; set; }
        public Nullable<double> CantidadFabricada { get; set; }
        public Nullable<double> CantidadPendiente { get; set; }
        public Nullable<double> CantidadPendienteAnterior { get; set; }
        public Nullable<int> Estado { get; set; }
        public Nullable<System.DateTime> FechaConsumo { get; set; }
        public Nullable<int> IdOperacion { get; set; }
        public string CodSeccion { get; set; }
    }
}