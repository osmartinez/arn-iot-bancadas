using Entidades;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLite.Update
{
    public class UpdatePrensas
    {
        public void ActualizarPrensa(LocalPrensa prensa)
        {
            var cfg = Fichero.LeerConfiguracion();
            var prensaEncontrada = cfg.Prensas.FirstOrDefault(x => x.Id == prensa.Id);
            if (prensaEncontrada != null)
            {
                prensaEncontrada.Left = prensa.Left;
                prensaEncontrada.Top = prensa.Top;
            }
            Fichero.EscribirConfiguracion(cfg);
        }
    }
}
