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
    
    public partial class SP_ObtenerTareasProgramadasMaquina_Result
    {
        public Nullable<int> IdOrden { get; set; }
        public Nullable<int> IdPedido { get; set; }
        public string CodigoOrden { get; set; }
        public string NombreCliente { get; set; }
        public string Modelo { get; set; }
        public string Utillaje { get; set; }
        public string Descripcion { get; set; }
        public string CodigoArticulo { get; set; }
        public Nullable<double> Ciclo { get; set; }
        public string TallaUtillaje { get; set; }
        public string TallasArticulo { get; set; }
        public Nullable<double> ParesTotales { get; set; }
        public Nullable<double> ParesFabricar { get; set; }
        public Nullable<double> ParesFabricarDivididos { get; set; }
        public Nullable<double> ParesFabricados { get; set; }
        public Nullable<double> ParesPendientes { get; set; }
        public Nullable<int> IdOfotc { get; set; }
        public Nullable<int> Posicion { get; set; }
        public Nullable<int> IdMaquina { get; set; }
        public Nullable<bool> Procesado { get; set; }
    }
}