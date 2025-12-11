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
            List<Almacen> almacenList = new();

            try
            {
                almacenList = context.Almacen.ToList();

                foreach (Almacen a in almacenList)
                {
                    var producto = context.Productos.FirstOrDefault(x => x.IdProducto == a.IdProducto);
                    var estatus = context.Estatus.FirstOrDefault(x => x.IdEstatus == a.IdEstatus);

                    a.Estatus = estatus?.NombreEstatus ?? "Desconocido";

                    var almacenista = context.Usuarios.FirstOrDefault(x => x.IdUsuario == a.IdAlmacenista);
                    var solicitante = context.Usuarios.FirstOrDefault(x => x.IdUsuario == a.IdSolicitante);

                    a.Almacenista = almacenista?.Username ?? "N/A";
                    a.Solicitante = solicitante?.Username ?? "N/A";

                    a.NombreProducto = producto?.NombreProducto ?? "Producto desconocido";
                    a.Descripcion = producto?.Descripcion ?? "-";
                }
                // Agrupar por producto y tomar solo un registro por producto
                almacenList = almacenList
                    .GroupBy(x => x.NombreProducto)
                    .Select(g =>
                    {
                        var first = g.First();

                        // Totales por estatus
                        first.EstatusFuera = g.Count(x => x.Estatus == "Fuera");
                        first.EstatusAlmacen = g.Count(x => x.Estatus == "Almacen");

                        // Total general si lo quieres seguir usando
                        first.Cantidad = g.Count();

                        return first;
                    })
                    .ToList();
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
                if (model.almacenLista.Any(x => x.Estatus.Equals("Almacen")))
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
        public int GuardarEnAlmacen(int idProducto, string serial)
        {
            try
            {
                var now = DateTime.Now;

                var existente = context.Almacen.SingleOrDefault(a => a.SerialNumber == serial);
                if (existente == null)
                {
                    context.Almacen.Add(new Almacen
                    {
                        IdProducto = idProducto,
                        SerialNumber = serial,
                        Movimiento = "Entrada",
                        IdEstatus = 2,     // <-- Almacén
                        Uso = false,
                        IdAlmacenista = 0,
                        IdSolicitante = 0,
                        Fecha = now
                    });
                }
                else
                {
                    existente.IdProducto = idProducto;
                    existente.Movimiento = "Entrada";
                    existente.IdEstatus = 2;   // <-- Almacén
                    existente.Uso = false;
                    existente.Fecha = now;

                    context.Almacen.Update(existente);
                }

                context.SaveChanges();
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public List<string> ObtenerSerialesValidosPorOrden(string orden, IEnumerable<string> seriales)
        {
            if (string.IsNullOrWhiteSpace(orden) || seriales == null) return new List<string>();

            var solicitados = seriales
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim().ToUpper())
                .ToList();

            if (solicitados.Count == 0) return new List<string>();

            // Validate in SerialMap (order + serial)
            var validos = context.SerialMap
                .Where(sm => sm.OrderNumber == orden && solicitados.Contains(sm.SerialKey))
                .Select(sm => sm.SerialKey)
                .Distinct()
                .ToList();

            // (Optional) also require they exist in Almacen:
            // validos = (from a in context.Almacen
            //            join sm in context.SerialMap on a.SerialNumber equals sm.SerialKey
            //            where sm.OrderNumber == orden && validos.Contains(sm.SerialKey)
            //            select sm.SerialKey).Distinct().ToList();

            return validos;
        }

        public int ContarSerialesPorProductoOrden(string pn, int ordenId)
        {
            return context.Almacen.Count(a => a.SerialNumber.StartsWith($"{pn}-{ordenId}-"));
        }

        /// <summary>
        /// Valida el estado del producto en Almacen para Entrada o Salida. Si el producto ya se encuentra en el estado contrario, retorna true.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="SourceView"></param>
        /// <returns></returns>
        public bool ValidarEstadoProducto(Almacen a, string SourceView)
        {
            bool exists = false; //Valor default
            if (SourceView == "Entrada")
            {
                // Verificar en Salida
                exists = context.Almacen.Any(x => x.SerialNumber.Equals(a.SerialNumber) && x.Movimiento.Equals("Salida"));
            }
            else if (SourceView == "Salida")
            {
                // Verificar en Entrada
                exists = context.Almacen.Any(x => x.SerialNumber.Equals(a.SerialNumber) && x.Movimiento.Equals("Entrada"));
            }
            return exists;
        }
    }
}
