using AgricolaDH_GApp.Controllers.Admin;
using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace AgricolaDH_GApp.Services.Admin
{
    public class RequisicionService
    {
        private readonly AppDbContext context;

        public RequisicionService(AppDbContext _ctx)
        {
            context = _ctx;
        }

        public List<RequisicionTable> SelectRequisiciones()
        {
            List<RequisicionTable> requisicionList;

            try
            {

                requisicionList = context.RequisicionesTable.FromSqlRaw("exec SP_SelectRequisiciones").ToList();

            }
            catch
            {
                requisicionList = new List<RequisicionTable>();
            }

            return requisicionList;
        }

        public List<ProductoOrdenar> SelectProductosOrdenar(int IdRequisicion)
        {
            List<ProductoOrdenar> productosOrdenar;

            try
            {
                productosOrdenar = context.ProductosOrdenar.Where(m => m.IdRequisicion == IdRequisicion).ToList();
            }
            catch
            {
                productosOrdenar = new List<ProductoOrdenar>();
            }

            return productosOrdenar;
        }

        public Requisicion SelectRequisicion(int IdRequisicion)
        {
            Requisicion requisicion;

            try
            {
                requisicion = context.Requisiciones.Find(IdRequisicion);
            }
            catch
            {
                requisicion = new Requisicion();
            }

            return requisicion;
        }

        public int InsertRequisicion(Requisicion requisicion)
        {
            int res = 0;

            try
            {
                context.Requisiciones.Add(requisicion);
                context.SaveChanges();

                res = requisicion.IdRequisicion; // Yes it's here
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

        public int UpdateRequisicion(Requisicion requisicion)
        {
            int res = 0;

            try
            {
                context.Requisiciones.Update(requisicion);
                context.SaveChanges();
            }
            catch(Exception ex) 
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
                Requisicion requisicion = context.Requisiciones.Find(IdRequisicion);

                context.Requisiciones.Remove(requisicion);
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
