using AgricolaDH_GApp.Controllers.Admin;
using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.ViewModels;

namespace AgricolaDH_GApp.Services.Admin
{
    public class RolService
    {
        private readonly AppDbContext context;

        public RolService(AppDbContext _ctx)
        {
            context = _ctx;
        }

        public List<Rol> SelectRoles()
        {
            List<Rol> rolList;

            try
            {
                rolList = context.Roles.ToList();
            }
            catch
            {
                rolList = new List<Rol>();
            }

            return rolList;
        }

        public Rol SelectRol(int IdRol)
        {
            Rol rol;

            try
            {
                rol = context.Roles.Find(IdRol);
            }
            catch
            {
                rol = new Rol();
            }

            return rol;
        }

    }
}
