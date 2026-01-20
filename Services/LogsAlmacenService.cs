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
                // Supongamos que almacenLista es una lista de productos
                foreach (Almacen a in model.almacenLista)
                {
                    LogsAlmacen log = new LogsAlmacen
                    {
                        IdProducto = a.IdProducto,
                        SerialKey = a.SerialNumber,
                        Fecha = model.almacen.Fecha,
                        IdSolicitante = a.IdSolicitante,
                        IdAlmacenista = a.IdAlmacenista,
                        IdMovimiento = a.IdEstatus,
                    };
                    context.LogsAlmacen.Add(log);
                }
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al guardar los logs de almacén: " + ex.Message, ex);
            }
        }
    }
}
