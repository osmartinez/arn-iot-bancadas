using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class ConsumoPrensa
    {
        public int Prensa { get; set; }
        public int Tipo { get; set; }
        public int IdTarea { get; set; }
        public int IdOF { get; set; }
        public int IdOperacion { get; set; }
        public string CodigoOF { get; set; }
        public string CodigoBarras { get; set; }
        public int SgCiclo { get; set; }
        public int ParesUtillaje { get; set; }
        public int NumMoldes { get; set; }
        public int PiezaIntroducida { get; set; }
        public string Utillaje { get; set; }
        public string TallaUtillaje { get; set; }
        public string TallaArticulo { get; set; }
        public string NombreCliente { get; set; }
        public string CodigoArticulo { get; set; }
        public int ParesTarea { get; set; }

        public string IpPlc { get; set; }
        public string Hora { get; set; }
        public DateTime HoraLocal
        {
            get
            {
                if (this.Hora == null)
                {
                    return DateTime.MinValue;
                }

                DateTime dt;
                if (DateTime.TryParseExact(this.Hora
                    .Trim()
                    .Replace("-  ", "-")
                    .Replace("- ", "-")
                    .Replace(":  ", ":")
                    .Replace(": ", ":")
                    .Replace("  ", " "), "yyyy-M-d H:m:s", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out dt))
                {
                    return dt.ToLocalTime();
                }
                else
                {
                    return DateTime.MinValue;
                }
            }
        }

    }
}
