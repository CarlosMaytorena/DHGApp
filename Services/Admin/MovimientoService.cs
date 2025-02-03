using AgricolaDH_GApp.Controllers.Admin;
using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.ViewModels;
using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;

namespace AgricolaDH_GApp.Services.Admin
{
    public class MovimientoService
    {
        private readonly AppDbContext context;

        public MovimientoService(AppDbContext _ctx)
        {
            context = _ctx;
        }

        /// <summary>
        /// Devuelve los datos de tabla Movimientos
        /// </summary>
        /// <returns></returns>
        public List<Movimiento> SelectMovimientos()
        {
            List<Movimiento> movimientosList;
            try
            {
                movimientosList = context.Movimientos.OrderByDescending(m => m.IdMovimiento).ToList();
            }
            catch
            {
                movimientosList = new List<Movimiento>();
            }
            return movimientosList;
        }

        public int Entrada(AlmacenDTO registro)
        {
            try
            {
                Producto producto = context.Productos.Find(registro.Movimiento.IdProducto);
                registro.Movimiento.Fecha = DateTime.Now;
                registro.Movimiento.TipoMovimiento = "Entrada";
                registro.Movimiento.Uso = "N/A";
                registro.Movimiento.Producto = producto.NombreProducto;
                context.Movimientos.Add(registro.Movimiento);
                return context.SaveChanges();
            }
            catch
            {
                return -1;
            }
        }

        public int Salida(AlmacenDTO registro)
        {
            try
            {
                Producto producto = context.Productos.Find(registro.Movimiento.IdProducto);

                registro.Movimiento.Fecha = DateTime.Now;
                registro.Movimiento.TipoMovimiento = "Salida";
                registro.Movimiento.Producto = producto.NombreProducto;
                context.Movimientos.Add(registro.Movimiento);
                return context.SaveChanges();
            }
            catch
            {
                return -1;
            }
        }

    }
}
