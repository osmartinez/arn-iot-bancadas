using Entidades;
using Entidades.Eventos;
using SesionManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turnos;

namespace CalculoActividad
{
    public static class Calculo
    {
        private const double COEF_FATIGA = 0.08;
        private const double VUELTA_BANCADA = 3.5;
        private const double COEF_SALTAR_MOLDE_VACIO = 2.3;
        private const double COEF_NECESIDAD = 0.06;
        private const double LIMPIAR_MOLDE = 0.063;
        private const double CAMBIAR_MOLDE = 0.063;
        private const double CAMBIAR_TOPE = 0.0938;
        private const double CAMBIAR_NUMERO = 0.13;

        private const int idMaquinaDirectora = 0;

        public static double CalcularPrimaDirecto(List<Maquinas> maquinas, List<EventoFichajeAsociacion> eventosFichaje, out double limitacionBancada)
        {
            limitacionBancada = 0;
            int vueltas = maquinas.Max(x => x.Pulsos.Where(y => y.IdOperario == Sesion.Operario.Id).Count());
            if (vueltas == 0)
            {
                return 0;
            }

            double horasJornada = Horario.CalcularHorasJornada(DateTime.Now);
            double vueltasHora = vueltas / horasJornada;
            double prima = 0;
            double CICLO_TEORICO = 0;
            double TIEMPO_MAQUINA_100 = 0;
            double TIEMPO_MAQUINA_140 = 0;
            double TIEMPO_CAMBIO_TEORICO = 0;
            double TIEMPO_CAMBIO_MAX = 0;

            double TM_100 = 0;
            double TM_140 = 0;
            double MM_100 = 0;
            double MM_140 = 0;
            double MP_100 = 0;
            double MP_140 = 0;
            double CICLO_100 = 0;
            double CICLO_140 = 0;
            double ACTIV_LIMIT = 0;
            double tiempo_vuelta_real = 0;
            double SALTAR_MOLDE_VACIO_100 = 0;
            double SALTAR_MOLDE_VACIO_140 = 0;
            int saltos = 0;

            maquinas = maquinas.OrderBy(x => x.PosicionGlobal ?? 0).ToList();

            List<PulsoMaquina> pulsos = maquinas.SelectMany(x => x.Pulsos).OrderByDescending(x => x.Fecha).ToList();

            if (pulsos.Count < maquinas.Count)
            {
                // todavía no hay pulsos suficientes para calcular
                return prima;
            }

            // tomo los ultimos N pulsos
            List<PulsoMaquina> ultimosPulsos = pulsos.Take(maquinas.Count).ToList();

            // busco el ciclo teorico de las maquinas
            CICLO_TEORICO = CicloTeorico(ultimosPulsos.Select(x => x.Ciclo));

            // calculo la suma total del tiempo de cambio teorico
            TIEMPO_CAMBIO_TEORICO = ultimosPulsos.Sum(x => x.Control.TiempoBaseEjecucion + (x.Control.TiempoUtillajeEjecucion * x.Pares));
            // busco el maximo tiempo de cambio teorico
            TIEMPO_CAMBIO_MAX = (ultimosPulsos.Select(x => x.Control.TiempoBaseEjecucion + (x.Control.TiempoUtillajeEjecucion * x.Pares)).Max());

            // calculo el tiempo maquina al 100
            TIEMPO_MAQUINA_100 = CICLO_TEORICO + TIEMPO_CAMBIO_MAX;
            // calculo el tiempo maquina 40% mas rapido
            TIEMPO_MAQUINA_140 = TIEMPO_MAQUINA_100 / 140 * 100;

            // cuento los saltos que ha habido en la última vuelta
            saltos = ContarSaltos(ultimosPulsos);

            // calculo el tiempo de salto de molde al 100
            SALTAR_MOLDE_VACIO_100 = COEF_SALTAR_MOLDE_VACIO * (saltos / ultimosPulsos.Count) * (1 /* FATIGA? */) * ultimosPulsos.Count;
            // calculo el tiempo de salto de molde 40% más rapido
            SALTAR_MOLDE_VACIO_140 = SALTAR_MOLDE_VACIO_100 / 140 * 100;

            // el tiempo maquina al 100 y 140 se quedan como están
            TM_100 = TIEMPO_MAQUINA_100;
            TM_140 = TIEMPO_MAQUINA_140;

            // obtengo los cambios de barquilla que ha habido
            var fichajesCajas = ObtenerEventosFichajeUltimaVuelta(pulsos, eventosFichaje);
            // calculo el tiempo operario como la suma de los tiempos de cambio + los saltos de molde
            MM_100 = TIEMPO_CAMBIO_TEORICO + SALTAR_MOLDE_VACIO_100 + fichajesCajas.Sum(x => x.Control.TiempoCambioBarquilla);
            //  calculo el tiempo operario 40% mas rapido
            MM_140 = MM_100 / 140 * 100;

            // me quedo con el mayor del tiempo maquina o tiempo operario al 100
            CICLO_100 = (TM_100 > MM_100) ? TM_100 : MM_100;
            // me quedo con el mayor del tiempo maquina o tiempo operario al 140
            CICLO_140 = (TM_140 > MM_140) ? TM_140 + MP_140 : MM_140 + MP_140;

            // calculo la limitación de la bancada
            double limite_informativo = 1 / (CICLO_140 / MM_100);
            limitacionBancada = limite_informativo;

            // calculo el tiempo real que le ha costado dar la vuelta
            tiempo_vuelta_real = TiempoVuelta(ultimosPulsos);

            // si tiempo_vuelta es menor que CICLO_140 -> 1.4
            //si no tengo que calcular 1/(tiempo_vuelta/ciclo_100)
            prima = 1 / (tiempo_vuelta_real / (CICLO_100 * (1 + COEF_FATIGA)));


            return Math.Round(prima, 2);
        }

        public static double CalcularPrima(List<Maquinas> maquinas)
        {
            int vueltas = maquinas.Max(x => x.Pulsos.Where(y => y.IdOperario == Sesion.Operario.Id).Count());
            if (vueltas == 0)
            {
                return 0;
            }

            double horasJornada = Horario.CalcularHorasJornada(DateTime.Now);
            double vueltasHora = vueltas / horasJornada;
            double prima = 0;
            double cicloMedio = 0;
            double tiempoCambioMax = 0;
            double TIEMPO_MAQUINA = 0;
            double TIEMPO_MAQUINA_100 = 0;
            double TIEMPO_MAQUINA_140 = 0;
            double FATIGA_MAQUINA_REGALADO = 0;
            double FATIGA_MAQUINA_REGALADO_100 = 0;
            double FATIGA_MAQUINA_REGALADO_140 = 0;
            double TM_100 = 0;
            double TM_140 = 0;
            double MM_100 = 0;
            double MM_140 = 0;
            double MP_100 = 0;
            double MP_140 = 0;
            double CICLO_100 = 0;
            double CICLO_140 = 0;
            double ACTIV_LIMIT = 0;
            int limpiarMoldes = 0;
            int cambiosMolde = 0;
            int cambiosTope = 0;
            int cambiosNumero = 0;
            double actividadAñadida = 0;

            maquinas = maquinas.OrderBy(x => x.PosicionGlobal ?? 0).ToList();

            cicloMedio = CicloTeorico(maquinas.SelectMany(x => x.Pulsos.Select(y => y.Ciclo)));
            tiempoCambioMax = TiempoCambioMedio(maquinas.SelectMany(x => x.Pulsos).ToList());
            TIEMPO_MAQUINA = cicloMedio + tiempoCambioMax;
            FATIGA_MAQUINA_REGALADO = COEF_FATIGA * TIEMPO_MAQUINA;

            TIEMPO_MAQUINA_100 = TIEMPO_MAQUINA * (1 + 0.00 + COEF_NECESIDAD);
            TIEMPO_MAQUINA_140 = TIEMPO_MAQUINA_100;

            FATIGA_MAQUINA_REGALADO_100 = FATIGA_MAQUINA_REGALADO * (1 + COEF_NECESIDAD);
            FATIGA_MAQUINA_REGALADO_140 = FATIGA_MAQUINA_REGALADO_100 / 140 * 100;

            TM_100 = TIEMPO_MAQUINA_100 + FATIGA_MAQUINA_REGALADO_100;
            TM_140 = TIEMPO_MAQUINA_140 + FATIGA_MAQUINA_REGALADO_140;
            MM_100 = tiempoCambioMax * maquinas.Count;
            MM_140 = MM_100 / 140 * 100;

            CICLO_100 = (TM_100 > MM_100) ? TM_100 + MP_100 : MM_100 + MP_100;
            CICLO_140 = (TM_140 > MM_140) ? TM_140 + MP_140 : MM_140 + MP_140;

            ACTIV_LIMIT = CICLO_100 / CICLO_140 * 100;


            actividadAñadida =
                LIMPIAR_MOLDE * limpiarMoldes
                + CAMBIAR_MOLDE * cambiosMolde
                + CAMBIAR_TOPE * cambiosTope
                + CAMBIAR_NUMERO * cambiosNumero;

            prima = ((((vueltas * 1.4) / (horasJornada * vueltasHora)) * horasJornada) + actividadAñadida) / horasJornada;

            return prima;
        }

        private static List<EventoFichajeAsociacion> ObtenerEventosFichajeUltimaVuelta(List<PulsoMaquina> pulsos, List<EventoFichajeAsociacion> eventosFichaje)
        {
            // obtengo la fecha inicio y final de la vuelta
            DateTime minFecha = pulsos.Select(x => x.Fecha).Min();
            DateTime maxFecha = pulsos.Select(x => x.Fecha).Max();

            return eventosFichaje.Where(x => minFecha <= x.Fecha && x.Fecha <= maxFecha).ToList();
        }

        private static int ContarSaltos(IEnumerable<PulsoMaquina> pulsos)
        {
            pulsos = pulsos.OrderBy(x => x.Fecha);

            int i = pulsos.First().PosicionGlobal;
            int saltos = 0;
            foreach (PulsoMaquina pulso in pulsos)
            {
                if (pulso.PosicionGlobal != i % pulsos.Count())
                {
                    saltos++;
                }
                i = pulso.PosicionGlobal + 1;
            }

            return saltos;
        }

        private static double CicloTeorico(IEnumerable<double> ciclos)
        {
            Dictionary<double, int> contador = new Dictionary<double, int>();
            foreach (double ciclo in ciclos)
            {
                if (!contador.ContainsKey(ciclo))
                {
                    contador.Add(ciclo, 1);
                }
                else
                {
                    contador[ciclo]++;
                }
            }

            KeyValuePair<double, int> max = contador.First();

            foreach (var par in contador)
            {
                if (par.Value > max.Value)
                {
                    max = par;
                }
            }

            return max.Key;

        }

        private static double TiempoVuelta(List<PulsoMaquina> todosPulsos)
        {
            List<PulsoMaquina> pulsos = todosPulsos.OrderBy(x => x.Fecha).ToList();

            double segundosDiferencia = pulsos.Last().Fecha.Subtract(pulsos.First().Fecha).TotalSeconds;
            return segundosDiferencia;
        }

        private static double TiempoCambioMedio(List<PulsoMaquina> todosPulsos)
        {
            List<PulsoMaquina> pulsos = todosPulsos.OrderBy(x => x.Fecha).ToList();

            double segundosDiferencia = pulsos.Last().Fecha.Subtract(pulsos.First().Fecha).TotalSeconds;
            return segundosDiferencia / pulsos.Count;
        }
    }
}
