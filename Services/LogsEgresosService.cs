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

        public int InsertarLog(LogsEgresos model)
        {
            try
            {
                context.LogsEgresos.Add(model);                
                context.SaveChanges();

                return model.IdLogsEgresos;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al guardar los logs de egresos: " + ex.Message, ex);
            }
        }
    }
}
