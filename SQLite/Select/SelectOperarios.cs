using Entidades;
using IRepositorios.ISelect;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLite.Select
{
    public class SelectOperarios : ISelectOperarios<LocalOperario>
    {

        public LocalOperario BuscarPorCodigo(string cod)
        {
            LocalConfiguracion cfg = Fichero.LeerConfiguracion();
            return cfg.Operarios.FirstOrDefault(x => x.Codigo.Contains(cod));
        }
    }
}
