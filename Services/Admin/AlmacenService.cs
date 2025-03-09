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
        public List<Almacen> SelectAlmacen()
        {
            List<Almacen> almacenList;
            try
            {
                //almacenList = context.Almacen.FromSqlRaw("exec SP_JoinAlmacen").ToList();
                almacenList = context.Almacen.ToList();
                foreach (Almacen a in almacenList)
                {
                    Producto p = context.Productos.Single(x => x.IdProducto.Equals(a.IdProducto));
                    Estatus estatus = context.Estatus.Single(x => x.IdEstatus.Equals(a.IdEstatus));
                    a.Estatus = estatus.NombreEstatus;
                    switch (a.Estatus)
                    {
                        case "Ingreso":
                            a.Almacenista = "N/A";
                            a.Solicitante = "N/A";
                            break;

                        case "Almacen":
                            Usuario almacenista = context.Usuarios.Single(x => x.IdUsuario.Equals(a.IdAlmacenista));
                            Usuario solicitante = context.Usuarios.Single(x => x.IdUsuario.Equals(a.IdSolicitante));
                            a.Almacenista = almacenista.Username;
                            a.Solicitante = solicitante.Username;
                            break;
                    }
                    a.NombreProducto = p.NombreProducto;
                    a.Descripcion = p.Descripcion;
                }
                context.SaveChanges();
            }
            catch
            {
                almacenList = new List<Almacen>();
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
