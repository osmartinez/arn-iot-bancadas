using Entidades;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLite.Add
{
    public class AddPrensa
    {
        public LocalPrensa Insertar(LocalPrensa t)
        {
            var cfg = Fichero.LeerConfiguracion();
            cfg.Prensas.Add(t);
            Fichero.EscribirConfiguracion(cfg);
            return t;
        }
    }
}
