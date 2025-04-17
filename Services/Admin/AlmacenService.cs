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
                    Almacen dbRow = context.Almacen.Single(x => x.IdAlmacen.Equals(a.IdAlmacen));
                    dbRow.IdEstatus = 2; //Almacen
                    dbRow.IdAlmacenista = model.almacen.IdAlmacenista;
                    dbRow.IdSolicitante = model.almacen.IdSolicitante;
                    dbRow.RazonUso = model.almacen.RazonUso;
                    dbRow.Movimiento = "Entrada";
                    dbRow.Fecha = fecha;
                    context.Almacen.Update(dbRow);
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
                    Almacen dbRow = context.Almacen.Single(x => x.IdAlmacen.Equals(a.IdAlmacen));
                    dbRow.IdEstatus = 3; //Fuera
                    dbRow.IdAlmacenista = model.almacen.IdAlmacenista;
                    dbRow.IdSolicitante = model.almacen.IdSolicitante;
                    dbRow.RazonUso = model.almacen.RazonUso;
                    dbRow.Movimiento = "Salida";
                    dbRow.Fecha = fecha;
                    dbRow.Uso = true;
                    context.Almacen.Update(dbRow);
                }
                context.SaveChanges();
            }
            catch
            {
                throw;
            }
        }
        // Entrada de Ingreso a Almacen
        public void GuardarEnAlmacen(int idProducto, string serial)
        {
            Almacen nuevoIngreso = new Almacen
            {
                IdProducto = idProducto,
                IdAlmacenista = 0,
                IdSolicitante = 0,
                Movimiento = "Ingreso",
                Fecha = DateTime.Now,
                SerialNumber = serial,
                IdEstatus = 1, // Estatus de Ingreso
                Uso = false
            };

            context.Almacen.Add(nuevoIngreso);
            context.SaveChanges();
        }

        public List<string> ObtenerSerialesValidos(List<string> seriales)
        {
            return context.Almacen
                .Where(a => seriales.Contains(a.SerialNumber))
                .Select(a => a.SerialNumber)
                .ToList();
        }

        public int ContarSerialesPorProductoOrden(string pn, int ordenId)
        {
            return context.Almacen.Count(a => a.SerialNumber.StartsWith($"{pn}-{ordenId}-"));
        }


    }
}
