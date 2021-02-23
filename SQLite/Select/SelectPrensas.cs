using Entidades;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLite.Select
{
    public class SelectPrensas
    {
        public List<LocalPrensa> BuscarTodas()
        {
            LocalConfiguracion cfg = Fichero.LeerConfiguracion();
            return cfg.Prensas;
        }
    }
}
