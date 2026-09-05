using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Models;
using Microsoft.EntityFrameworkCore;

namespace AgricolaDH_GApp.Services.Admin
{
    public class IngresoService
    {
        private readonly AppDbContext context;
        private readonly SerialMapService serialMapService;
        private readonly AlmacenService almacenService;
        private readonly LogsAlmacenService logsAlmacenService;

        public IngresoService(
            AppDbContext _ctx,
            SerialMapService _serialMapService,
            AlmacenService _almacenService,
            LogsAlmacenService _logsAlmacenService)
        {
            context = _ctx;
            serialMapService = _serialMapService;
            almacenService = _almacenService;
            logsAlmacenService = _logsAlmacenService;
        }

        public Ingreso? SelectIngreso(int idIngreso)
        {
            try { return context.Ingresos.Find(idIngreso); }
            catch { return null; }
        }

        public Ingreso? SelectUltimoIngreso(int idOrdenDeCompra)
        {
            try
            {
                return context.Ingresos
                    .Where(i => i.IdOrdenDeCompra == idOrdenDeCompra && !i.Cancelado)
                    .OrderByDescending(i => i.IdIngreso)
                    .FirstOrDefault();
            }
            catch { return null; }
        }

        public int InsertIngreso(Ingreso ingreso)
        {
            int res = 0;
            try
            {
                context.Ingresos.Add(ingreso);
                context.SaveChanges();
            }
            catch { res = -1; }
            return res;
        }

        public int UpdateIngreso(Ingreso ingreso)
        {
            int res = 0;
            try
            {
                context.Ingresos.Update(ingreso);
                context.SaveChanges();
            }
            catch { res = -1; }
            return res;
        }

        public int InsertIngresoDetalle(IngresoDetalle detalle)
        {
            int res = 0;
            try
            {
                context.IngresoDetalle.Add(detalle);
                context.SaveChanges();
            }
            catch { res = -1; }
            return res;
        }

        // Acumulado recibido (no cancelado) por línea de ProductoOrdenar, para una orden completa.
        public Dictionary<int, int> SelectCantidadRecibidaAcumulada(int idOrdenDeCompra)
        {
            try
            {
                return context.IngresoDetalle
                    .Where(d => d.IdOrdenDeCompra == idOrdenDeCompra && context.Ingresos
                        .Any(i => i.IdIngreso == d.IdIngreso && !i.Cancelado))
                    .GroupBy(d => d.IdProductoOrdenar)
                    .Select(g => new { IdProductoOrdenar = g.Key, Total = g.Sum(x => x.CantidadRecibida) })
                    .ToDictionary(x => x.IdProductoOrdenar, x => x.Total);
            }
            catch
            {
                return new Dictionary<int, int>();
            }
        }

        /// <summary>
        /// Revierte por completo un ingreso (seriales, almacén, folio de log y detalle),
        /// solo permitido para el ingreso más reciente no cancelado de la orden.
        /// </summary>
        public int CancelarIngreso(int idIngreso)
        {
            int res = 0;
            try
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    var ingreso = context.Ingresos.Find(idIngreso);
                    if (ingreso == null || ingreso.Cancelado)
                    {
                        return -1;
                    }

                    // Los folios de este ingreso se borran primero (del más reciente al más
                    // antiguo) para no dejar huecos en la secuencia diaria; si alguno ya no es
                    // el más reciente del día (otro movimiento se registró después), se aborta
                    // toda la cancelación.
                    var folios = logsAlmacenService.SelectByIdIngresoDescending(idIngreso);
                    foreach (var folio in folios)
                    {
                        bool borrado = logsAlmacenService.EliminarLogsAlmacenSiEsElUltimo(folio.IdLogsAlmacen!.Value);
                        if (!borrado)
                        {
                            transaction.Rollback();
                            return -1;
                        }
                    }

                    serialMapService.DeleteSerialsByIdIngreso(idIngreso);
                    almacenService.EliminarPorIdIngreso(idIngreso);

                    var detalles = context.IngresoDetalle.Where(d => d.IdIngreso == idIngreso).ToList();
                    context.IngresoDetalle.RemoveRange(detalles);

                    ingreso.Cancelado = true;
                    context.Ingresos.Update(ingreso);

                    context.SaveChanges();
                    transaction.Commit();
                }
            }
            catch
            {
                res = -1;
            }
            return res;
        }
    }
}
