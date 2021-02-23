using Entidades.Extensiones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public partial class MaquinasColasTrabajo:Notificable
    {
        private int particiones = 1;

        public int Particiones
        {
            get { return particiones; }
            set { particiones = value; }
        }

        public double ParesFabricar
        {
            get
            {
                return this.OrdenesFabricacionOperacionesTallasCantidad.CantidadFabricar.Value +
                this.OrdenesFabricacionOperacionesTallasCantidad.CantidadSaldos.Value/*) / particiones*/;
            }
        }

        public double ParesFabricados
        {
            get
            {
                double paresFabricados = this.OrdenesFabricacionOperacionesTallasCantidad.OrdenesFabricacionProductos.Sum(x => x.Cantidad);
                return paresFabricados;//.Sum(x => x.OrdenesFabricacionPaquetes.Where(p=>p.IdMaquinaConsumo == this.IdMaquina).Sum(y => y.Cantidad));
            }
        }

        public double ParesPendientes
        {
            get
            {
                return ParesFabricar - ParesFabricados;
            }
        }


        // override object.Equals
        public override bool Equals(object obj)
        {
            //       
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237  
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            MaquinasColasTrabajo o = obj as MaquinasColasTrabajo;
            return o.Id == this.Id;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            // TODO: write your implementation of GetHashCode() here
            return base.GetHashCode();
        }
    }
}
