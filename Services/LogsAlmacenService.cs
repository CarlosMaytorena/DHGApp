using AgricolaDH_GApp.Controllers.Admin;
using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace AgricolaDH_GApp.Services
{
    public class LogsAlmacenService
    {
        private readonly AppDbContext context;

        public LogsAlmacenService(AppDbContext _ctx)
        {
            context = _ctx;
        }

        public LogsAlmacen InsertarLogsAlmacen(AlmacenVM model, int? idIngreso = null)
        {
            try
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    var i = model.almacenLista.First();
                    model.almacen = context.Almacen.Single(x => x.IdAlmacen == i.IdAlmacen);

                    DateTime hoy = DateTime.Today;
                    DateTime mañana = hoy.AddDays(1);

                    // obtener última secuencia del día (por rango)
                    int ultimaSecuencia = context.LogsAlmacen.Where(x => x.Fecha >= hoy && x.Fecha < mañana).Max(x => (int?)x.SecuenciaDia) ?? 0;

                    int nuevaSecuencia = ultimaSecuencia + 1;

                    // Insertar log
                    LogsAlmacen log = new LogsAlmacen
                    {
                        Fecha = model.almacen.Fecha,
                        IdSolicitante = model.almacen.IdSolicitante,
                        IdAlmacenista = model.almacen.IdAlmacenista,
                        IdMovimiento = model.almacen.IdEstatus,
                        SecuenciaDia = nuevaSecuencia,
                        Folio = $"{hoy:yyyyMMdd}-{nuevaSecuencia:D4}",
                        IdIngreso = idIngreso
                    };
                    context.LogsAlmacen.Add(log);
                    context.SaveChanges();
                    transaction.Commit();
                    return log;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al guardar el log de almacén: " + ex.Message, ex);
            }
        }
        public void InsertarLogsAlmacenProductos(AlmacenVM model)
        {
            try
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    
                    foreach (Almacen a in model.almacenLista)
                    {
                        LogsAlmacenProductos logProducto = new LogsAlmacenProductos
                        {
                            IdLogsAlmacen = model.logsAlmacen.IdLogsAlmacen,
                            IdProducto = a.IdProducto,
                            SerialKey = a.SerialNumber
                        };
                        context.LogsAlmacenProductos.Add(logProducto);
                    }
                    context.SaveChanges();
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                // Si ocurre un error, la transacción se revierte automáticamente al salir del using
                throw new Exception("Error al guardar los logs de almacén: " + ex.Message, ex);
            }

        }

        public List<LogsAlmacen> SelectByIdIngresoDescending(int idIngreso)
        {
            try
            {
                return context.LogsAlmacen
                    .Where(l => l.IdIngreso == idIngreso)
                    .OrderByDescending(l => l.SecuenciaDia)
                    .ToList();
            }
            catch
            {
                return new List<LogsAlmacen>();
            }
        }

        /// <summary>
        /// Borra el folio de LogsAlmacen (y sus LogsAlmacenProductos) solo si sigue siendo
        /// la secuencia máxima del día para esa fecha. Si ya se generó un folio posterior,
        /// no se borra (dejaría un hueco en la secuencia) y se retorna false.
        /// </summary>
        public bool EliminarLogsAlmacenSiEsElUltimo(int idLogsAlmacen)
        {
            try
            {
                var log = context.LogsAlmacen.Find(idLogsAlmacen);
                if (log == null) return true; // nada que borrar

                DateTime hoy = log.Fecha.Date;
                DateTime mañana = hoy.AddDays(1);

                int maxSecuencia = context.LogsAlmacen
                    .Where(x => x.Fecha >= hoy && x.Fecha < mañana)
                    .Max(x => (int?)x.SecuenciaDia) ?? 0;

                if (log.SecuenciaDia != maxSecuencia)
                {
                    return false; // ya no es el último folio del día, no se puede borrar sin dejar hueco
                }

                var detalles = context.LogsAlmacenProductos.Where(p => p.IdLogsAlmacen == idLogsAlmacen).ToList();
                context.LogsAlmacenProductos.RemoveRange(detalles);
                context.LogsAlmacen.Remove(log);
                context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar el log de almacén: " + ex.Message, ex);
            }
        }
    }
}
