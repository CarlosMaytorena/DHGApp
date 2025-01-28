using AgricolaDH_GApp.Controllers.Admin;
using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.ViewModels;
using Microsoft.EntityFrameworkCore;

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
                using (var transaction = context.Database.BeginTransaction())
                {

                    // Generate the ProductBarcodeID
                    var lastSequence = context.UltimoProductoID.First();
                    int newBarcodeNumber = lastSequence.LastBarcodeNumber + 1;

                    // Update the LastBarcodeNumber in the sequence table
                    lastSequence.LastBarcodeNumber = newBarcodeNumber;
                    context.SaveChanges();

                    // Check if a product with the same name already exists
                    bool exists = context.Productos.Any(p => p.NombreProducto == producto.NombreProducto);

                    if (exists)
                    {
                        // Return a specific code indicating a duplicate name
                        return -2;
                    }

                    // Assign the new ProductBarcodeID
                    producto.ProductBarcodeID = $"PN{newBarcodeNumber.ToString("D6")}";

                    // Add the new product
                    context.Productos.Add(producto);
                    context.SaveChanges();

                    // Commit transaction
                    transaction.Commit();
                }
            }
            catch (Exception)
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

        public Producto SelectProductoByName(string nombreProducto)
        {
            Producto producto = null;

            try
            {
                // Use LINQ to search for the product by its name
                producto = context.Productos
                    .FirstOrDefault(p => p.NombreProducto == nombreProducto);
            }
            catch (Exception)
            {
                // Handle exception or log the error
            }

            return producto;
        }

    }
}
