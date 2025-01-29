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
        public int Entrada(AlmacenDTO registro)
        {
            if (registro == null)
            {
                return -1;
            }
            try
            {
                var registroAlmacen = context.Almacen.SingleOrDefault(a => a.IdProducto == registro.Almacen.IdProducto);
                var registroProducto = context.Productos.SingleOrDefault(a => a.IdProducto == registro.Almacen.IdProducto);
                if (registroProducto == null | (registroAlmacen == null && registro.Motivo == 2))
                {
                    return -1;
                }
                
                if (registroAlmacen == null && registro.Motivo == 1)
                {
                    //Insert
                    context.Almacen.Add(registro.Almacen);
                }
                if (registroAlmacen != null && registro.Motivo == 1)
                {
                    //Update
                    registroAlmacen.Disponible += registro.Almacen.Disponible;
                }
                if(registroAlmacen != null && registro.Motivo == 2)
                {
                    //Update
                    int validacion = registroAlmacen.EnUso - registro.Almacen.Disponible;
                    if (validacion < 0)
                    {
                        return -1;
                    }
                    registroAlmacen.Disponible += registro.Almacen.Disponible;
                    registroAlmacen.EnUso -= registro.Almacen.Disponible;
                }
                return context.SaveChanges();
            }
            catch
            {
                return -1;
            }
        }

        public int Salida(AlmacenDTO registro)
        {
            if (registro == null)
            {
                return -1;
            }
            try
            {
                var registroAlmacen = context.Almacen.SingleOrDefault(a => a.IdProducto == registro.Almacen.IdProducto);
                var registroProducto = context.Productos.SingleOrDefault(a => a.IdProducto == registro.Almacen.IdProducto);
                if (registroProducto == null | registroAlmacen == null)
                {
                    return -1;
                }
                int dec = registroAlmacen.Disponible - registro.Almacen.Disponible;
                if (dec < 0)
                {
                    return -1;
                }
                if (registro.Motivo == 1)
                {
                    registroAlmacen.EnUso += registro.Almacen.Disponible;
                }
                if (registro.Motivo == 2)
                {
                    registroAlmacen.Terminados += registro.Almacen.Disponible;
                }
                return context.SaveChanges();
            }
            catch
            {
                return -1;
            }
        }

        public Producto SelectProductoByBarcode(Producto registro)
        {
            Producto registroTable = new Producto();
            try
            {
                registroTable = context.Productos.SingleOrDefault(a => a.ProductBarcodeID == registro.ProductBarcodeID);
            }
            catch
            {
            }
            return registroTable;
        }

        private int JsonResult(object value)
        {
            throw new NotImplementedException();
        }
    }
}
