using AgricolaDH_GApp.Controllers.Admin;
using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.ViewModels;

namespace AgricolaDH_GApp.Services.Admin
{
    public class AlmacenService
    {
        private readonly AppDbContext context;

        public AlmacenService(AppDbContext _ctx)
        {
            context = _ctx;
        }

        public List<Almacen> SelectAlmacen()
        {
            List<Almacen> almacenList;

            try
            {
                almacenList = context.Almacen.ToList();
            }
            catch
            {
                almacenList = new List<Almacen>();
            }

            return almacenList;
        }
    }
}
