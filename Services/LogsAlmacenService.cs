using AgricolaDH_GApp.Controllers.Admin;
using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.ViewModels;

namespace AgricolaDH_GApp.Services
{
    public class LogsAlmacenService
    {
        private readonly AppDbContext context;

        public LogsAlmacenService(AppDbContext _ctx)
        {
            context = _ctx;
        }

        public void InsertarLog(AlmacenVM model)
        {
            try
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    var i = model.almacenLista.First();
                    model.almacen = context.Almacen.Single(x => x.IdAlmacen == i.IdAlmacen);

                    LogsAlmacen log = new LogsAlmacen
                    {
                        Fecha = model.almacen.Fecha,
                        IdSolicitante = model.almacen.IdSolicitante,
                        IdAlmacenista = model.almacen.IdAlmacenista,
                        IdMovimiento = model.almacen.IdEstatus,
                    };
                    context.LogsAlmacen.Add(log);
                    context.SaveChanges();

                    foreach (Almacen a in model.almacenLista)
                    {
                        LogsAlmacenProductos logProducto = new LogsAlmacenProductos
                        {
                            IdLogsAlmacen = log.IdLogsAlmacen,
                            IdProducto = a.IdProducto,
                            SerialKey = a.SerialNumber
                        };
                        context.LogsAlmacenProductos.Add(logProducto);
                        context.SaveChanges();
                    }
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
