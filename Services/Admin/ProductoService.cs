using AgricolaDH_GApp.Controllers.Admin;
using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.ViewModels;

namespace AgricolaDH_GApp.Services.Admin
{
    public class ProductoService
    {
        private readonly AppDbContext context;

        public ProductoService(AppDbContext _ctx)
        {
            context = _ctx;
        }
        public Producto SelectProducto(int IdProducto)
        {
            Producto producto = new Producto(); 

            try
            {
                producto = context.Productos.Find(IdProducto);
                
            }
            catch
            {


            }

            return producto;
        }

        public List<Producto> SelectProductos()
        {
            List<Producto> productoList;

            try
            {
                productoList = context.Productos.ToList();
            }
            catch
            {
                productoList = new List<Producto>();
            }

            return productoList;
        }

        public int InsertProducto(Producto producto)
        {
            int res = 0;

            try
            {
                context.Productos.Add(producto);
                context.SaveChanges();
            }
            catch(Exception ex)
            {
                res = -1;
            }

            return res;

        }

        public int UpdateProducto(Producto producto)
        {
            int res = 0;

            try
            {
                context.Productos.Update(producto);
                context.SaveChanges();
            }
            catch(Exception ex) 
            { 
                res = -1;
            }

            return res;
        }

        public int DeleteProducto(int IdProducto)
        {
            int res = 0;

            try
            {
                Producto producto = context.Productos.Find(IdProducto);

                context.Productos.Remove(producto);
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
