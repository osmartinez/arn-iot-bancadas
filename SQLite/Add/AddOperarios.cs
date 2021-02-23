using Entidades;
using IRepositorios.IAdd;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLite.Add
{
    public class AddOperarios : IAddOperarios<LocalOperario>
    {
        public LocalOperario Insertar(LocalOperario t)
        {
            var cfg = Fichero.LeerConfiguracion();
            cfg.Operarios.Add(t);
            Fichero.EscribirConfiguracion(cfg);
            return t;
        }
    }
}
