using AgricolaDH_GApp.Controllers.Admin;
using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.ViewModels;

namespace AgricolaDH_GApp.Services
{
    public class LogsEgresosService
    {
        private readonly AppDbContext context;

        public LogsEgresosService(AppDbContext _ctx)
        {
            context = _ctx;
        }

        public int InsertarLog(LogsEgresos log)
        {
            try
            {
                context.LogsEgresos.Add(log);                
                context.SaveChanges();

                return log.IdLogsEgresos;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al generar logs de Egresos: " + ex.Message, ex);
            }
        }
    }
}
