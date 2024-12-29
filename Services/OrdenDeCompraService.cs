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

                requisicionList = context.RequisicionesTable.FromSqlRaw("exec SP_SelectRequisiciones").ToList();

            }
            catch
            {
                requisicionList = new List<OrdenDeCompraTable>();
            }

            return requisicionList;
        }

        public List<OrdenDeCompraTable> SelectOrdenDeCompraTable(int IdOrdenDeCompraStatus)
        {
            List<OrdenDeCompraTable> ordenDeCompraList;

            try
            {

                ordenDeCompraList = context.RequisicionesTable.FromSqlRaw("exec SP_SelectOrdenDeCompraTable @IdOrdenDeCompraStatus",
                    new SqlParameter("@IdOrdenDeCompraStatus", IdOrdenDeCompraStatus)).ToList();

            }
            catch
            {
                ordenDeCompraList = new List<OrdenDeCompraTable>();
            }

            return ordenDeCompraList;
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

        public int UpdateRequisicion(OrdenDeCompra requisicion)
        {
            int res = 0;

            try
            {
                context.OrdenesDeCompra.Update(requisicion);
                context.SaveChanges();
            }
            catch(Exception ex) 
            { 
                res = -1;
            }

            return res;
        }

        public int UpdateProductoOrdenar(ProductoOrdenar productoOrdenar)
        {
            int res = 0;

            try
            {
                context.ProductosOrdenar.Update(productoOrdenar);
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
