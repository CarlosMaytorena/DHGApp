using AgricolaDH_GApp.Controllers.Admin;
using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.ViewModels;
using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

                        default:
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

        public void Entrada(AlmacenVM model)
        {
            try
            {
                if (model.almacenLista.Any(x => x.Estatus.Equals("Almacen")) )
                {
                    throw new Exception();
                }
                DateTime fecha = DateTime.Now;
                foreach (Almacen a in model.almacenLista)
                {

                    context.Almacen.Single(x => x.IdAlmacen.Equals(a.IdAlmacen)).IdEstatus = 2; //Almacen
                    context.Almacen.Single(x => x.IdAlmacen.Equals(a.IdAlmacen)).IdAlmacenista = model.almacen.IdAlmacenista;
                    context.Almacen.Single(x => x.IdAlmacen.Equals(a.IdAlmacen)).IdSolicitante = model.almacen.IdSolicitante;
                    context.Almacen.Single(x => x.IdAlmacen.Equals(a.IdAlmacen)).RazonUso = model.almacen.RazonUso;
                    context.Almacen.Single(x => x.IdAlmacen.Equals(a.IdAlmacen)).Movimiento = "Entrada";
                    context.Almacen.Single(x => x.IdAlmacen.Equals(a.IdAlmacen)).Fecha = fecha;
                }
                context.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        public void Salida(AlmacenVM model)
        {
            try
            {
                if (model.almacenLista.Any(x => !x.Estatus.Equals("Almacen")))
                {
                    throw new Exception();
                }
                DateTime fecha = DateTime.Now;
                foreach (Almacen a in model.almacenLista)
                {
                    context.Almacen.Single(x => x.IdAlmacen.Equals(a.IdAlmacen)).IdEstatus = 3; //Fuera
                    context.Almacen.Single(x => x.IdAlmacen.Equals(a.IdAlmacen)).IdAlmacenista = model.almacen.IdAlmacenista;
                    context.Almacen.Single(x => x.IdAlmacen.Equals(a.IdAlmacen)).IdSolicitante = model.almacen.IdSolicitante;
                    context.Almacen.Single(x => x.IdAlmacen.Equals(a.IdAlmacen)).RazonUso = model.almacen.RazonUso;
                    context.Almacen.Single(x => x.IdAlmacen.Equals(a.IdAlmacen)).Movimiento = "Salida";
                    context.Almacen.Single(x => x.IdAlmacen.Equals(a.IdAlmacen)).Fecha = fecha;
                }
                context.SaveChanges();
            }
            catch
            {
                throw;
            }
        }
    }
}
