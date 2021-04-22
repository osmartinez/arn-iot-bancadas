using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF.AddProcedimientos
{
    public class AddColaTrabajo
    {
        public List<MaquinasColasTrabajo> ActualizarColaTrabajo(string codigoBarquilla, List<int> idsTareas, int? agrupacion, int idMaquina, int idOperario,double cantidad)
        {
            List<MaquinasColasTrabajo> trabajosInsertar = new List<MaquinasColasTrabajo>();

            using (SistemaGlobalPREEntities db = new SistemaGlobalPREEntities())
            {
                if (idsTareas.Any())
                {
                    // recupero la cola de la maquina
                    var trabajos = db.MaquinasColasTrabajo
                        .Include("OrdenesFabricacionOperacionesTallasCantidad.OrdenesFabricacionOperacionesTallas.OrdenesFabricacionOperaciones.OrdenesFabricacion.Campos_ERP")
                        .Include("OrdenesFabricacionOperacionesTallasCantidad.OrdenesFabricacionProductos")
                        .Where(x => x.IdMaquina == idMaquina).ToList();

                    // elimino todos los trabajos que tengan que ver con la tarea que quiero ejecutar
                    trabajos.RemoveAll(x => idsTareas.Contains(x.IdTarea));

                    // elimino los trabajos que se encuentren en ejecución actualmente
                    trabajos.RemoveAll(x => x.Ejecucion);

                    // los ordeno por posicion
                    var trabajosOrdenados = trabajos.OrderBy(x => x.Posicion).ToList();

                    // actualizo sus posiciones en +1
                    int i = 1;
                    int anterior = 0;
                    foreach (var trabajo in trabajosOrdenados)
                    {
                        if (anterior == 0)
                        {
                            anterior = trabajo.Posicion;
                            trabajo.Posicion = i;
                        }
                        else
                        {
                            if (anterior == trabajo.Posicion)
                            {
                                trabajo.Posicion = i;
                            }
                            else
                            {
                                i++;
                                anterior = trabajo.Posicion;
                                trabajo.Posicion = i;
                            }
                        }
                    }

                    // busco el trabajo en ejecucion actual de la cola
                    MaquinasColasTrabajo trabajoEjecucionActual = db.MaquinasColasTrabajo.FirstOrDefault(x => x.IdMaquina == idMaquina && x.Ejecucion);
                    // si tiene trabajo en ejecución
                    if (trabajoEjecucionActual != null)
                    {
                        // lo desubico
                        var barquilla = db.Barquillas.FirstOrDefault(x => x.CodigoEtiqueta == trabajoEjecucionActual.CodigoEtiquetaFichada);
                        barquilla.CodUbicacion = null;
                    }

                    // elimino toda la cola
                    db.MaquinasColasTrabajo.RemoveRange(db.MaquinasColasTrabajo.Where(x => x.IdMaquina == idMaquina).ToList());


                    // inserto los trabajos antiguos con las posiciones actualizadas en +1
                    foreach (var trabajo in trabajosOrdenados)
                    {
                        trabajosInsertar.Add(new MaquinasColasTrabajo
                        {
                            CantidadEtiquetaFichada = trabajo.CantidadEtiquetaFichada,
                            Ejecucion = false,
                            Posicion = trabajo.Posicion + 1,
                            IdTarea = trabajo.IdTarea,
                            Agrupacion = trabajo.Agrupacion,
                            FechaProgramado = trabajo.FechaProgramado,
                            IdMaquina = idMaquina,
                            IdOperarioEjecuta = trabajo.IdOperarioEjecuta,
                            IdOperarioPrograma = trabajo.IdOperarioPrograma,
                            CodigoEtiquetaFichada = trabajo.CodigoEtiquetaFichada,
                            OrdenesFabricacionOperacionesTallasCantidad = trabajo.OrdenesFabricacionOperacionesTallasCantidad
                        });

                    }

                    // inserto la nueva tarea en ejecución
                    foreach (var id in idsTareas)
                    {
                        var tarea = db.OrdenesFabricacionOperacionesTallasCantidad
                            .Include("OrdenesFabricacionOperacionesTallas.OrdenesFabricacionOperaciones.OrdenesFabricacion.Campos_ERP")
                            .Include("OrdenesFabricacionProductos")
                            .FirstOrDefault(x => x.ID == id);
                        trabajosInsertar.Add(new MaquinasColasTrabajo
                        {
                            CantidadEtiquetaFichada = cantidad,
                            IdMaquina = idMaquina,
                            IdOperarioEjecuta = idOperario,
                            IdTarea = id,
                            Posicion = 1,
                            Agrupacion = agrupacion ?? 0,
                            FechaProgramado = DateTime.Now,
                            Ejecucion = true,
                            CodigoEtiquetaFichada = codigoBarquilla,
                            OrdenesFabricacionOperacionesTallasCantidad = tarea,
                        });
                    }

                    db.MaquinasColasTrabajo.AddRange(trabajosInsertar);
                    db.SaveChanges();
                }

            }

            return trabajosInsertar;
        }
    }
}
