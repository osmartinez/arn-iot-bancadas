using Entidades.Eventos;
using Entidades.Extensiones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public partial class Maquinas :Notificable
    {
        public event EventHandler OnErrorTareaSinEjecutar;
        public event EventHandler OnColaTrabajoActualizada;
        public event EventHandler OnParesConsumidos;
        public event EventHandler OnInfoEjecucionActualizada;
        public event EventHandler<ColorearEventArgs> OnPeticionColorear;


        public string Cliente { get; set; } = "- SIN CLIENTE -";
        public string Utillaje { get; set; }
        public string Modelo { get; set; }
        public int ParesFabricando { get; set; }
        public string TallaUtillaje { get; set; }
        public string CodigoOrden { get; set; } = "- SIN OF -";
        public int IdTarea { get; set; }
        public string CodigoArticulo { get; set; }
        public double SgCiclo { get; set; }
        public double ParesCiclo { get; set; } = 1;
        public int NumMoldes { get; set; }

        public List<PulsoMaquina> Pulsos { get; private set; } = new List<PulsoMaquina>();

        public void ColaTrabajoActualizada()
        {
            if (OnColaTrabajoActualizada != null)
            {
                OnColaTrabajoActualizada(this, new EventArgs());
            }
        }

        public void PeticionColorear(string color)
        {
            if (OnPeticionColorear
                != null)
            {
                OnPeticionColorear(this, new ColorearEventArgs(color));
            }
        }

        public void InfoEjecucionActualizada()
        {
            if (OnInfoEjecucionActualizada != null)
            {
                OnInfoEjecucionActualizada(this, new EventArgs());
            }
        }


        public void ErrorTareaSinEjecutar()
        {
            if (OnErrorTareaSinEjecutar != null)
            {
                OnErrorTareaSinEjecutar(this, new EventArgs());
            }
        }

        public void ParesConsumidos()
        {
            if (OnParesConsumidos != null)
            {
                OnParesConsumidos(this, new EventArgs());
            }
        }

        public void AsignarColaTrabajo(List<MaquinasColasTrabajo> cola)
        {
            this.MaquinasColasTrabajo = cola.OrderBy(x => x.Posicion).ToList();
            ColaTrabajoActualizada();
        }

        public void DesasignarTrabajo(MaquinasColasTrabajo trabajo)
        {
            var lista = this.MaquinasColasTrabajo.ToList();
            lista.RemoveAll(x => x.IdMaquina == trabajo.IdMaquina && x.Posicion == trabajo.Posicion);
            this.AsignarColaTrabajo(lista);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            Maquinas o = (obj as Maquinas);
            return o.Posicion == this.Posicion && o.IpAutomata == this.IpAutomata;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            // TODO: write your implementation of GetHashCode() here
            return base.GetHashCode();
        }

        public void CargarInformacion(ConsumoPrensa consumo)
        {
            bool infoActualizada = false;
            if (consumo.NombreCliente != this.Cliente
                || consumo.Utillaje != this.Utillaje
                || consumo.IdTarea != this.IdTarea)
            {
                infoActualizada = true;
            }
            this.Cliente = consumo.NombreCliente.Trim();
            this.Utillaje = consumo.Utillaje.Trim();
            this.CodigoArticulo = consumo.CodigoArticulo.Trim();
            this.ParesFabricando = consumo.ParesTarea;
            this.TallaUtillaje = consumo.TallaUtillaje.Trim();
            this.CodigoOrden = consumo.CodigoOF.Trim();
            this.IdTarea = consumo.IdTarea;
            this.SgCiclo = consumo.SgCiclo;
            this.NumMoldes = consumo.NumMoldes;
            int nuevosParesCiclo = consumo.NumMoldes * consumo.ParesUtillaje;
            if (this.ParesCiclo != nuevosParesCiclo)
            {
                this.ParesCiclo = nuevosParesCiclo;
                this.ColaTrabajoActualizada();
            }
            else
            {
                this.ParesCiclo = nuevosParesCiclo;
            }

            if (infoActualizada)
            {
                this.InfoEjecucionActualizada();
                Notifica();
            }
        }

        public void CargarInformacion(AsociacionTarea asociacion)
        {
            this.Cliente = asociacion.Cliente.Trim();
            this.Utillaje = asociacion.Utillaje.Trim();
            this.CodigoArticulo = asociacion.CodigoArticulo.Trim();
            this.ParesFabricando = asociacion.Pares;
            this.TallaUtillaje = asociacion.TallaUtillaje.Trim() ;
            this.CodigoOrden = asociacion.CodigoOrden.Trim();
            this.IdTarea = asociacion.IdTarea;
            this.InfoEjecucionActualizada();
            Notifica();

        }

        public bool InsertarPares(MaquinasColasTrabajo trabajo, double pares)
        {
            MaquinasColasTrabajo t = this.MaquinasColasTrabajo.FirstOrDefault(x => x.Id == trabajo.Id && x.Ejecucion);
            if (t != null)
            {
               
                t.OrdenesFabricacionOperacionesTallasCantidad.OrdenesFabricacionProductos.Add(new OrdenesFabricacionProductos
                {
                    Cantidad = pares,
                    FechaCreacion = DateTime.Now,
                    IdMaquina = this.ID,

                });
                this.Notifica();
                t.Notifica();
                this.ColaTrabajoActualizada();
                this.ParesConsumidos();
                return true;
            }
            else
            {
                this.ErrorTareaSinEjecutar();
                return false;
            }


        }
    }
}
