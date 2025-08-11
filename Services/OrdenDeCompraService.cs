using AgricolaDH_GApp.Controllers.Admin;
using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.ViewModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace AgricolaDH_GApp.Services.Admin
{
    public class OrdenDeCompraService
    {
        private readonly AppDbContext context;

        public OrdenDeCompraService(AppDbContext _ctx)
        {
            context = _ctx;
        }

        public List<OrdenDeCompraTable> SelectRequisiciones()
        {
            List<OrdenDeCompraTable> requisicionList;

            try
            {

                requisicionList = context.OrdenDeCompraTable.FromSqlRaw("exec SP_SelectRequisiciones").ToList();

            }
            catch
            {
                requisicionList = new List<OrdenDeCompraTable>();
            }

            return requisicionList;
        }

        public List<OrdenDeCompraTable> SelectOrdenDeCompraTableList(int IdOrdenDeCompraStatus, int IdUsuario, int LimiteDeSemanas = 0)
        {
            List<OrdenDeCompraTable> ordenDeCompraList;

            try
            {
                ordenDeCompraList = context.OrdenDeCompraTable.FromSqlRaw("exec SP_SelectOrdenDeCompraTable @IdOrdenDeCompraStatus, @IdUsuario, @LimiteDeSemanas",
                    new SqlParameter("@IdOrdenDeCompraStatus", IdOrdenDeCompraStatus),
                    new SqlParameter("@IdUsuario", IdUsuario),
                    new SqlParameter("@LimiteDeSemanas", LimiteDeSemanas)
                    ).ToList();

            }
            catch
            {
                ordenDeCompraList = new List<OrdenDeCompraTable>();
            }

            return ordenDeCompraList;
        }

        public OrdenDeCompraTable SelectOrdenDeCompra(int IdOrdenDeCompra)
        {
            OrdenDeCompraTable ordenDeCompra;

            try
            {

                ordenDeCompra = context.OrdenDeCompraTable.FromSqlRaw("exec SP_SelectOrdenDeCompraTableById @IdOrdenDeCompra",
                    new SqlParameter("@IdOrdenDeCompra", IdOrdenDeCompra)).ToList().FirstOrDefault();

            }
            catch
            {
                ordenDeCompra = new OrdenDeCompraTable();
            }

            return ordenDeCompra;
        }

        public List<ProductoOrdenar> SelectProductosOrdenar(int IdOrdenDeCompra)
        {
            List<ProductoOrdenar> productosOrdenar;

            try
            {
                productosOrdenar = context.ProductosOrdenar.Where(m => m.IdOrdenDeCompra == IdOrdenDeCompra).ToList();
            }
            catch
            {
                productosOrdenar = new List<ProductoOrdenar>();
            }

            return productosOrdenar;
        }

        public ProductoOrdenar SelectProductoOrdenar(int IdProductoOrdenar)
        {
            ProductoOrdenar productoOrdenar;

            try
            {
                productoOrdenar = context.ProductosOrdenar.Find(IdProductoOrdenar);
            }
            catch
            {
                productoOrdenar = new ProductoOrdenar();
            }

            return productoOrdenar;
        }

        public List<ProductoOrdenarSelected> SelectProductosOrdenarSelected(int IdOrdenDeCompra)
        {
            List<ProductoOrdenarSelected> productosOrdenar;

            try
            {
                productosOrdenar = context.ProductoOrdenarSelected.FromSqlRaw("exec SP_SelectProductoOrdenarByIdOrdenDeCompra @IdOrdenDeCompra",
                    new SqlParameter("@IdOrdenDeCompra", IdOrdenDeCompra)).ToList();
            }
            catch
            {
                productosOrdenar = new List<ProductoOrdenarSelected>();
            }

            return productosOrdenar;
        }

        public OrdenDeCompra SelectRequisicion(int IdRequisicion)
        {
            OrdenDeCompra requisicion;

            try
            {
                requisicion = context.OrdenesDeCompra.Find(IdRequisicion);
            }
            catch
            {
                requisicion = new OrdenDeCompra();
            }

            return requisicion;
        }

        public int InsertRequisicion(OrdenDeCompra requisicion)
        {
            int res = 0;

            try
            {
                context.OrdenesDeCompra.Add(requisicion);
                context.SaveChanges();

                res = requisicion.IdOrdenDeCompra; // Yes it's here
            }
            catch (Exception ex)
            {
                res = -1;
            }

            return res;

        }

        public int InsertProductoOrdenar(ProductoOrdenar productoOrdenar)
        {
            int res = 0;

            try
            {
                context.ProductosOrdenar.Add(productoOrdenar);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                res = -1;
            }

            return res;

        }

        public int UpdateOrdenDeCompra(OrdenDeCompra ordenDeCompra)
        {
            int res = 0;

            try
            {
                context.OrdenesDeCompra.Update(ordenDeCompra);
                context.SaveChanges();
            }
            catch(Exception ex) 
            { 
                res = -1;
            }

            return res;
        }

        public int UpdateOrdenDeCompraStatus(int IdOrdenDeCompra, int IdOrdenDeCompraStatus)
        {
            int res = 0;

            OrdenDeCompra ordenDeCompra = new OrdenDeCompra() 
            {
                IdOrdenDeCompra = IdOrdenDeCompra, 
                IdOrdenDeCompraStatus = IdOrdenDeCompraStatus 
            };

            try
            {
                context.OrdenesDeCompra.Attach(ordenDeCompra);
                context.Entry(ordenDeCompra).Property(m => m.IdOrdenDeCompraStatus).IsModified = true;
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                res = -1;
            }

            return res;
        }

        public int UpdateProductoOrdenar(ProductoOrdenar productoOrdenarUpdate)
        {
            int res = 0;

            try
            {

                context.ProductosOrdenar.Update(productoOrdenarUpdate);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                res = -1;
            }

            return res;
        }

        public int UpdateProductoOrdenarConFactura(ProductoOrdenarSelected productoOrdenarConFactura)
        {
            int res = 0;

            ProductoOrdenar productoOrdenar = new ProductoOrdenar()
            {
                IdProductoOrdenar = productoOrdenarConFactura.IdProductoOrdenar,
                Cantidad = productoOrdenarConFactura.Cantidad,
                Unidad = productoOrdenarConFactura.Unidad,
                Total = productoOrdenarConFactura.Total,
                PorRecibir = productoOrdenarConFactura.Cantidad

            };

            try
            {
                context.ProductosOrdenar.Attach(productoOrdenar);
                context.Entry(productoOrdenar).Property(m => m.Cantidad).IsModified = true;
                context.Entry(productoOrdenar).Property(m => m.Unidad).IsModified = true;
                context.Entry(productoOrdenar).Property(m => m.Total).IsModified = true;
                context.Entry(productoOrdenar).Property(m => m.PorRecibir).IsModified = true;

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                res = -1;
            }

            return res;
        }

        public int DeleteRequisiciones(int IdRequisicion)
        {
            int res = 0;

            try
            {
                OrdenDeCompra requisicion = context.OrdenesDeCompra.Find(IdRequisicion);

                context.OrdenesDeCompra.Remove(requisicion);
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
