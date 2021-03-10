using Entidades.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turnos
{
    public static class Horario
    {
        private static int comienzoTurnoMañana = 6;
        private static int comienzoTurnoTarde = 14;
        private static int comienzoTurnoNoche = 22;
        public static void CalcularHorarioTurno(Turno turno, DateTime fecha, out DateTime fechaInicio, out DateTime fechaFin)
        {

            switch (turno)
            {
                case Turno.Mañana:
                    fechaInicio = new DateTime(fecha.Year, fecha.Month, fecha.Day, comienzoTurnoMañana, 0, 0);
                    fechaFin = new DateTime(fecha.Year, fecha.Month, fecha.Day, comienzoTurnoTarde, 0, 0);
                    break;
                case Turno.Tarde:
                    fechaInicio = new DateTime(fecha.Year, fecha.Month, fecha.Day, comienzoTurnoTarde, 0, 0);
                    fechaFin = new DateTime(fecha.Year, fecha.Month, fecha.Day, comienzoTurnoNoche, 0, 0);
                    break;
                case Turno.Noche:
                    fechaInicio = new DateTime(fecha.Year, fecha.Month, fecha.Day, comienzoTurnoNoche, 0, 0);
                    fecha = fecha.AddDays(1);
                    fechaFin = new DateTime(fecha.Year, fecha.Month, fecha.Day, comienzoTurnoMañana, 0, 0);
                    break;
                default:
                    fechaInicio = fecha;
                    fechaFin = fecha;
                    break;
            }
        }

        public static double CalcularHorasJornada(DateTime ahora)
        {
            Turno turno = CalcularTurnoAFecha(ahora);
            DateTime comienzo;
            DateTime fin;
            CalcularHorarioTurno(turno, ahora, out comienzo, out fin);
            return ahora.Subtract(comienzo).TotalHours;
        }

        public static Turno CalcularTurnoAFecha(DateTime fecha)
        {

            Turno turno = Turno.Mañana;
            // 6 - 14    14 - 22    22 - 6
            if (fecha.Hour >= comienzoTurnoMañana && fecha.Hour < comienzoTurnoTarde)
            {
                turno = Turno.Mañana;
            }
            else if (fecha.Hour >= comienzoTurnoTarde && fecha.Hour < comienzoTurnoNoche)
            {
                turno = Turno.Tarde;
            }
            else if (fecha.Hour >= comienzoTurnoNoche && fecha.Hour < comienzoTurnoMañana)
            {
                turno = Turno.Noche;
            }
            return turno;
        }
    }
}
