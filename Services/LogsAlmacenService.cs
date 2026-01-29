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

        public LogsAlmacen InsertarLogsAlmacen(AlmacenVM model)
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
                        Folio = $"{hoy:yyyyMMdd}-{nuevaSecuencia:D4}"
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
    }
}
