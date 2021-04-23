using Entidades.Enum;
using Entidades.Eventos;
using Entidades.Extensiones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public partial class Maquinas : Notificable
    {
        public event EventHandler OnErrorTareaSinEjecutar;
        public event EventHandler OnColaTrabajoActualizada;
        public event EventHandler OnParesConsumidos;
        public event EventHandler OnInfoEjecucionActualizada;
        public event EventHandler<ColorearEventArgs> OnPeticionColorear;
        public event EventHandler<ModoMaquinaCambioEventArgs> OnModoCambiado;

        private const double COEF_VARIACION = 0.03;

        public string ModuloViejo
        {
            get
            {
                if (this.Nombre
                    != null)
                {
                    string nombreSust = this.Nombre.Replace("MOLDE ESPUMA ", "");
                    return nombreSust.Substring(0, 2);
                }
                return "";
            }
        }
        public string Cliente
        {
            get
            {
                if(Modo == ModoMaquina.Calentamiento)
                {
                    return "";
                }

                if (TrabajoEjecucion != null)
                {
                    string nombre = TrabajoEjecucion.OrdenesFabricacionOperacionesTallasCantidad.OrdenesFabricacionOperacionesTallas.OrdenesFabricacionOperaciones.OrdenesFabricacion.Campos_ERP.NOMBRECLI;
                    if (nombre == null)
                    {
                        return "ARNEPLANT S.L.";
                    }
                    else
                    {
                        return nombre;
                    }
                }
                else
                {
                    return "";
                }
            }
        }
        public string Utillaje
        {
            get
            {
                if (Modo == ModoMaquina.Calentamiento)
                {
                    return "";
                }
                if (this.Nombre.Contains("102"))
                {

                }
                if (TrabajoEjecucion != null)
                {
                    return TrabajoEjecucion.OrdenesFabricacionOperacionesTallasCantidad.OrdenesFabricacionOperacionesTallas.OrdenesFabricacionOperaciones.CodUtillaje;
                }
                else
                {
                    return "";
                }
            }
        }
        public string Modelo
        {
            get
            {
                if (TrabajoEjecucion != null)
                {
                    return TrabajoEjecucion.OrdenesFabricacionOperacionesTallasCantidad.OrdenesFabricacionOperacionesTallas.OrdenesFabricacionOperaciones.OrdenesFabricacion.Campos_ERP.DESCRIPCIONARTICULO;
                }
                else
                {
                    return "";
                }
            }
        }
        public int ParesFabricando
        {
            get
            {
                if (TrabajoEjecucion != null)
                {
                    return (int)TrabajoEjecucion.OrdenesFabricacionOperacionesTallasCantidad.CantidadFabricar.Value +
                         (int)TrabajoEjecucion.OrdenesFabricacionOperacionesTallasCantidad.CantidadSaldos.Value;
                }
                else
                {
                    return 0;
                }
            }
        }
        public string TallaUtillaje
        {
            get
            {
                if (Modo == ModoMaquina.Calentamiento)
                {
                    return "";
                }
                if (TrabajoEjecucion != null)
                {
                    return TrabajoEjecucion.OrdenesFabricacionOperacionesTallasCantidad.OrdenesFabricacionOperacionesTallas.IdUtillajeTalla;
                }
                else
                {
                    return "";
                }
            }
        }
        public string CodigoOrden
        {
            get
            {
                if (TrabajoEjecucion != null)
                {
                    return TrabajoEjecucion.OrdenesFabricacionOperacionesTallasCantidad.OrdenesFabricacionOperacionesTallas.OrdenesFabricacionOperaciones.OrdenesFabricacion.Codigo;
                }
                else
                {
                    return "";
                }
            }
        }
        public int IdTarea { get; private set; }
        public string CodigoArticulo
        {
            get
            {
                if (TrabajoEjecucion != null)
                {
                    return TrabajoEjecucion.OrdenesFabricacionOperacionesTallasCantidad.OrdenesFabricacionOperacionesTallas.OrdenesFabricacionOperaciones.OrdenesFabricacion.CodigoArticulo;
                }
                else
                {
                    return "";
                }
            }
        }
        public double SgCiclo { get; private set; }
        public double ParesCiclo { get; set; }

        public double CantidadCaja
        {
            get
            {
                if (Modo == ModoMaquina.Calentamiento)
                {
                    return 0;
                }
                if (TrabajoEjecucion != null)
                {
                    return TrabajoEjecucion.CantidadEtiquetaFichada;
                }
                else
                {
                    return 0;
                }
            }
        }

        public double CantidadCajaRealizada
        {
            get
            {
                if (Modo == ModoMaquina.Calentamiento)
                {
                    return 0;
                }
                if (this.TrabajoEjecucion != null)
                {
                    var pulsos = this.Pulsos.Where(x => x.CodigoEtiqueta == this.TrabajoEjecucion.CodigoEtiquetaFichada);
                    if (pulsos.Count() > 0)
                    {
                        return pulsos.Sum(x => x.Pares);
                    }
                    else
                    {
                        return 0;
                    }

                }
                else
                {
                    return 0;
                }
            }
        }

        public MaquinasColasTrabajo TrabajoEjecucion
        {
            get
            {
                if (IdTarea != 0)
                {
                    if (this.MaquinasColasTrabajo.Count > 0)
                    {
                        var trabajo = this.MaquinasColasTrabajo.FirstOrDefault(x => x.IdTarea == IdTarea);
                        return trabajo;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    if (this.MaquinasColasTrabajo.Count > 0)
                    {
                        var enEjecucion = this.MaquinasColasTrabajo.Where(x => x.Ejecucion).ToList();
                        if (enEjecucion.Count != 1)
                        {
                            if (enEjecucion.Count == 0)
                            {
                                var planificadas = this.MaquinasColasTrabajo.Where(x => !x.Ejecucion).ToList();
                                if (planificadas.Count == 0)
                                {
                                    return null;
                                }
                                else
                                {
                                    int minPosicion = planificadas.Min(x => x.Posicion);
                                    return planificadas.FirstOrDefault(x => x.Posicion == minPosicion);
                                }
                            }
                            else
                            {

                                // agrupacion
                                double maxPendientes = this.MaquinasColasTrabajo.Max(x => x.ParesPendientes);
                                return this.MaquinasColasTrabajo.FirstOrDefault(x => x.ParesPendientes == maxPendientes);
                            }
                        }
                        else
                        {

                            return enEjecucion[0];

                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }

        }
        public int NumMoldes { get; set; }
        public double Tinf { get; set; }
        public double Tmed { get; set; }
        public double Tsup { get; set; }
        public double SetInf { get; set; }
        public double SetMed { get; set; }
        public double SetSup { get; set; }

        public bool TemperaturaOK
        {
            get
            {
                return (1 - COEF_VARIACION) * SetInf <= Tinf && Tinf <= (1 + COEF_VARIACION) * SetInf
                    &&
                    (1 - COEF_VARIACION) * SetMed <= Tmed && Tmed <= (1 + COEF_VARIACION) * SetMed
                    &&
                    (1 - COEF_VARIACION) * SetSup <= Tsup && Tsup <= (1 + COEF_VARIACION) * SetSup;
            }
        }

        public ModoMaquina Modo { get; set; } = ModoMaquina.Normal;

        public List<PulsoMaquina> Pulsos { get; private set; } = new List<PulsoMaquina>();

        public void CambiarModo(ModoMaquina modo)
        {
            this.Modo = modo;
            Notifica();
            if (OnModoCambiado != null)
            {
                OnModoCambiado(this, new ModoMaquinaCambioEventArgs(modo));
            }
        }

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
            Notifica();
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
                || consumo.SgCiclo != this.SgCiclo
                || consumo.Utillaje != this.Utillaje
                || consumo.IdTarea != this.IdTarea
                || consumo.Tinf != this.Tinf
                || consumo.Tmed != this.Tmed
                || consumo.Tsup != this.Tsup)
            {
                infoActualizada = true;
            }

            this.IdTarea = consumo.IdTarea;
            this.SgCiclo = consumo.SgCiclo;
            this.NumMoldes = consumo.NumMoldes;
            this.Tinf = consumo.Tinf;
            this.Tmed = consumo.Tmed;
            this.Tsup = consumo.Tsup;
            this.SetInf = consumo.SetInf;
            this.SetMed = consumo.SetMed;
            this.SetSup = consumo.SetSup;

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

            this.InfoEjecucionActualizada();
            Notifica();
        }

        public void CargarInformacion(AsociacionTarea asociacion)
        {
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
