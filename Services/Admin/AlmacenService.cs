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

        public void Entrada(AlmacenVM model)
        {
            try
            {
                foreach (Almacen a in model.almacenLista)
                {
                    if (context.Almacen.Any(a => a.SerialNumber.Equals(a.SerialNumber)))
                    {
                        //Producto llega a Almacen correctamente
                        context.Almacen.FirstOrDefault(a => a.SerialNumber.Equals(a.SerialNumber)).IdEstatus = 2;
                    }
                }
                context.SaveChanges();
            }
            catch
            {
                throw;
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
                int dec = 0;//registroAlmacen.Disponible - registro.Almacen.Disponible;
                if (dec < 0)
                {
                    return -1;
                }
                //registroAlmacen.Disponible -= registro.Almacen.Disponible;
                if (registro.Motivo == 1)
                {
                    //registroAlmacen.EnUso += registro.Almacen.Disponible;
                }
                if (registro.Motivo == 2)
                {

                    //registroAlmacen.Terminados += registro.Almacen.Disponible;
                }
                return context.SaveChanges();
            }
            catch
            {
                return -1;
            }
        }

        public Producto ProductTypeByPN(string id)
        {
            Producto registro = new Producto();
            try
            {
                registro = context.Productos.SingleOrDefault(a => a.PN.Equals(id));
            }
            catch
            {
            }
            return registro;
        }
    }
}
