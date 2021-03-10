using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class HojaAuditoria
    {
        public Operarios Operario { get; set; }
        public List<HojaAuditoriaPrensa> Prensas { get; set; } = new List<HojaAuditoriaPrensa>();

        public HojaAuditoria(Operarios operario,List<OrdenesFabricacionProductos> paquetes, List<Maquinas> maquinas)
        {
            Operario = operario;

            var agrupadosPrensa = paquetes.GroupBy(x => x.IdMaquina);

            foreach(var grupo in agrupadosPrensa)
            {
                Maquinas prensa = maquinas.FirstOrDefault(x => x.ID == grupo.Key);

                HojaAuditoriaPrensa hojaPrensa = new HojaAuditoriaPrensa(prensa, grupo.ToList());
                Prensas.Add(hojaPrensa);
            }
        }
    }
}
