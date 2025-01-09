using AgricolaDH_GApp.Controllers.Admin;
using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.ViewModels;
using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgricolaDH_GApp.Services.Admin
{
    public class AlmacenService
    {
        private readonly AppDbContext context;

        public AlmacenService(AppDbContext _ctx)
        {
            context = _ctx;
        }

        /// <summary>
        /// Mostrar datos de tabla principal de Almacen
        /// </summary>
        /// <returns></returns>
        public List<AlmacenView> SelectAlmacen()
        {
            List<AlmacenView> almacenList;
            try
            {
                almacenList = context.AlmacenView.FromSqlRaw("exec SP_JoinAlmacen").ToList();
            }
            catch
            {
                almacenList = new List<AlmacenView>();
            }
            return almacenList;
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
        /// <summary>
        /// Actualizar nuevo registro en tabla Almacen
        /// </summary>
        /// <returns></returns>
        public int EntradaParaAlmacen(Almacen registro)
        {
            if (registro == null)
            {
                return -1;
            }
            try
            {
                var registroAlmacen = context.Almacen.SingleOrDefault(a => a.IdProducto == registro.IdProducto);
                var registroProducto = context.Productos.SingleOrDefault(a => a.IdProducto == registro.IdProducto);
                if (registroProducto == null)
                {
                    return -1;
                }
                if (registroAlmacen == null)
                {
                    //Insert new 
                    context.Almacen.Add(registro);
                }
                else
                {
                    //update
                    registroAlmacen.Cantidad += registro.Cantidad;
                }
                return context.SaveChanges();
            }
            catch
            {
                return -1;
            }
        }

        public int SalidaDeAlmacen(Almacen registro)
        {
            if (registro == null)
            {
                return -1;
            }
            try
            {
                var registroAlmacen = context.Almacen.SingleOrDefault(a => a.IdProducto == registro.IdProducto);
                var registroProducto = context.Productos.SingleOrDefault(a => a.IdProducto == registro.IdProducto);
                if (registroProducto == null | registroAlmacen == null)
                {
                    return -1;
                }
                int dec = registroAlmacen.Cantidad - registro.Cantidad;
                if (dec < 0)
                {
                    return -1;
                }
                registroAlmacen.Cantidad -= registro.Cantidad;
                return context.SaveChanges();
            }
            catch
            {
                return -1;
            }
        }

        private int JsonResult(object value)
        {
            throw new NotImplementedException();
        }
    }
}
