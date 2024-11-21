using AgricolaDH_GApp.Controllers.Admin;
using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.ViewModels;

namespace AgricolaDH_GApp.Services.Admin
{
    public class ProveedorService
    {
        private readonly AppDbContext context;

        public ProveedorService(AppDbContext _ctx)
        {
            context = _ctx;
        }

        public List<Proveedor> SelectProveedores()
        {
            List<Proveedor> proveedorList;

            try
            {
                proveedorList = context.Proveedores.ToList();
            }
            catch
            {
                proveedorList = new List<Proveedor>();
            }

            return proveedorList;
        }

        public int InsertProveedor(Proveedor proveedor)
        {
            int res = 0;

            try
            {
                context.Proveedores.Add(proveedor);
                context.SaveChanges();
            }
            catch(Exception ex)
            {
                res = -1;
            }

            return res;

        }

        public int UpdateProveedor(Proveedor proveedor)
        {
            int res = 0;

            try
            {
                context.Proveedores.Update(proveedor);
                context.SaveChanges();
            }
            catch(Exception ex) 
            { 
                res = -1;
            }

            return res;
        }

        public int DeleteProveedor(int IdProveedor)
        {
            int res = 0;

            try
            {
                Proveedor proveedor = context.Proveedores.Find(IdProveedor);

                context.Proveedores.Remove(proveedor);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                res = -1;
            }

            return res;
        }
    }
}
