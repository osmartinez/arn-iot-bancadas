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
    
    public partial class SP_BancadaBuscarPorId_Result
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public Nullable<bool> Activa { get; set; }
        public Nullable<int> IdHermano { get; set; }
        public bool EsMaster { get; set; }
        public bool EsManual { get; set; }
        public Nullable<decimal> TiempoDesplazamiento { get; set; }
        public Nullable<decimal> TiempoObjetivo { get; set; }
        public Nullable<decimal> PorcentajeDesviacion { get; set; }
        public string CICLO { get; set; }
        public Nullable<decimal> CorrectorBancada { get; set; }
        public Nullable<decimal> TiempoMaquina { get; set; }
        public Nullable<decimal> TiempoOperario { get; set; }
        public Nullable<double> Ritmo { get; set; }
        public Nullable<double> CicloSegundos { get; set; }
        public string Observaciones { get; set; }
        public string CodigoEtiqueta { get; set; }
        public Nullable<int> IdBancada { get; set; }
        public string PinBuzzer { get; set; }
        public string PinLed { get; set; }
        public Nullable<double> ContadorPaquetes { get; set; }
        public Nullable<bool> EsContadorPaquetesAutomatico { get; set; }
        public Nullable<bool> AvisarFinPaquete { get; set; }
        public Nullable<bool> AvisarFinTarea { get; set; }
    }
}
