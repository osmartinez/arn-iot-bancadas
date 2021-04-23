using Entidades;
using Entidades.Eventos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArnMonitorBancadaWPF.Util
{
    public static class Store
    {
        public static event EventHandler OnStoreIniciada;

        public static List<EventoFichajeAsociacion> EventosFichajes { get; set; } = new List<EventoFichajeAsociacion>();
        public static Bancadas Bancada { get; set; }
        public static List<OperacionesControles> Controles { get; set; } = new List<OperacionesControles>();

        public static void Reset()
        {
            foreach(var maquina in Bancada.Maquinas)
            {
                maquina.Pulsos.Clear();
            }
            EventosFichajes.Clear();
        }

        public static void StoreIniciada()
        {
            if (OnStoreIniciada != null)
            {
                OnStoreIniciada(null, new EventArgs());
            }
        }

    }
}
