using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turnos
{
    public static class CalcularHoja
    {
        public static HojaProduccion CalcularHojaProduccion(Operarios operario, List<MaquinasRegistrosDatos> paquetes, List<Maquinas> maquinas, DateTime fechaInicio, DateTime fechaFin)
        {

            HojaProduccion hoja = new HojaProduccion();
            hoja.Operario = operario;

            if (paquetes.Count == 0)
            {
                return hoja;
            }

            foreach (var paquete in paquetes)
            {
                paquete.IdMaquina = maquinas.FirstOrDefault(x => x.IpAutomata == paquete.IpAutomata && x.Posicion == paquete.PosicionMaquina).ID;
            }

            int maxVueltas = 0;
            int cambiosUtillaje = 0;
            int cambiosTope = 0;
            int idMaquinaMasVueltas = 0;
            double mediaVuelta = 0;

            maxVueltas = CalculoVueltasYCambios(paquetes, out idMaquinaMasVueltas, out mediaVuelta, out cambiosUtillaje, out cambiosTope);
            CalculoPrima(paquetes, maquinas, idMaquinaMasVueltas);

            hoja.Datos.Add(new ClaveValores
            {
                Clave = "Horas totales",
                Valor1 = Convert.ToString(fechaFin.Subtract(fechaInicio).TotalHours),
            });

            hoja.Datos.Add(new ClaveValores
            {
                Clave = "Horas efectivas",
                Valor1 = Convert.ToString(Math.Round(paquetes.Last().FechaCreacion.Subtract(paquetes.First().FechaCreacion).TotalHours, 2)),
            });

            hoja.Datos.Add(new ClaveValores
            {
                Clave = "Pares fabricados",
                Valor1 = Convert.ToString(paquetes.Sum(x => x.Pares)),
            });

            hoja.Datos.Add(new ClaveValores
            {
                Clave = "Vueltas",
                Valor1 = Convert.ToString(maxVueltas),
            });

            hoja.Datos.Add(new ClaveValores
            {
                Clave = "Tiempo medio vuelta",
                Valor1 = Convert.ToString(Math.Round(mediaVuelta, 2)),
            });

            hoja.Datos.Add(new ClaveValores
            {
                Clave = "Cambios utillaje",
                Valor1 = Convert.ToString(cambiosUtillaje),
            });

            hoja.Datos.Add(new ClaveValores
            {
                Clave = "Cambios tope",
                Valor1 = Convert.ToString(cambiosTope),
            });

            return hoja;
        }

        private static void CalculoPrima(List<MaquinasRegistrosDatos> paquetes, List<Maquinas> maquinas, int idMaquinaPrincipal)
        {
            List<MaquinasRegistrosDatos> paquetesOrdenados = paquetes.OrderBy(x => x.FechaCreacion).ToList();
            List<List<MaquinasRegistrosDatos>> paquetesVueltas = new List<List<MaquinasRegistrosDatos>>();

            int idPrimeraMaquina = paquetes.First().IdMaquina;

            List<MaquinasRegistrosDatos> vueltaTemp = new List<MaquinasRegistrosDatos>();
            foreach (var paquete in paquetesOrdenados)
            {
                if (paquete.IdMaquina == idPrimeraMaquina)
                {
                    if (vueltaTemp.Count > 0)
                    {
                        paquetesVueltas.Add(vueltaTemp.ToList());
                        vueltaTemp.Clear();
                        vueltaTemp.Add(paquete);
                    }
                    else
                    {
                        vueltaTemp.Add(paquete);
                    }
                }
                else
                {
                    vueltaTemp.Add(paquete);
                }
            }
        }


        private static int CalculoVueltasYCambios(List<MaquinasRegistrosDatos> paquetes, out int idMaquinaMax, out double mediaVuelta, out int cambiosUtillaje, out int cambiosTope)
        {
            int maxVueltas = 0;
            idMaquinaMax = 0;
            mediaVuelta = 0;
            cambiosUtillaje = 0;
            cambiosTope = 0;
            var grupos = paquetes.GroupBy(x => x.IdMaquina);
            foreach (var grupo in grupos)
            {
                if (grupo.Count() > maxVueltas)
                {
                    maxVueltas = grupo.Count();
                    idMaquinaMax = grupo.Key;
                }

                CalcularCambiosUtillajeYTope(grupo.ToList(), ref cambiosUtillaje, ref cambiosTope);
            }

            int idMaquinaMasVueltas = idMaquinaMax;

            if (idMaquinaMasVueltas != 0)
            {
                var paquetesMaquina = paquetes.Where(x => x.IdMaquina == idMaquinaMasVueltas).ToList();

                if (paquetesMaquina.Count > 0)
                {
                    DateTime fechaPaqueteAnterior = paquetesMaquina.First().FechaCreacion;
                    foreach (var paquete in paquetesMaquina)
                    {
                        mediaVuelta += paquete.FechaCreacion.Subtract(fechaPaqueteAnterior).TotalSeconds;
                        fechaPaqueteAnterior = paquete.FechaCreacion;
                    }
                }
                mediaVuelta = mediaVuelta / maxVueltas;
            }

            return maxVueltas;
        }

        private static void CalcularCambiosUtillajeYTope(List<MaquinasRegistrosDatos> grupo, ref int cambiosUtillaje, ref int cambiosTope)
        {

            List<OrdenesFabricacionOperacionesTallas> operacionesTallas = grupo.Select(x => x.OrdenesFabricacionOperacionesTallasCantidad.OrdenesFabricacionOperacionesTallas).ToList();
            string codUtillaje = operacionesTallas.First().OrdenesFabricacionOperaciones.CodUtillaje;
            string talla = operacionesTallas.First().IdUtillajeTalla;

            string punta = "";
            string talon = "";
            string puntaActual = "";
            string talonActual = "";

            foreach (var operacionTalla in operacionesTallas)
            {
                if (operacionTalla.IdUtillajeTalla != talla || operacionTalla.OrdenesFabricacionOperaciones.CodUtillaje != codUtillaje)
                {
                    cambiosUtillaje++;
                    codUtillaje = operacionTalla.OrdenesFabricacionOperaciones.CodUtillaje;
                    talla = operacionTalla.IdUtillajeTalla;
                }

                puntaActual = "";
                talonActual = "";
                var partes = operacionTalla.OrdenesFabricacionOperaciones.Descripcion.Split(new string[] { "MM" }, StringSplitOptions.None);
                if (partes.Length == 3)
                {
                    var partes2 = partes[0].Split(new string[] { " A " }, StringSplitOptions.None);
                    var partes3 = partes[1].Split('Y');
                    if (partes2.Length == 2 && partes3.Length == 2)
                    {
                        puntaActual = partes2[1].Trim();
                        talonActual = partes3[1].Trim();
                    }
                }

                if ((puntaActual != punta && punta != "") || (talonActual != talon && talon != ""))
                {
                    cambiosTope++;
                    punta = puntaActual;
                    talon = talonActual;
                }

            }


        }
    }

}
