using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Models;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace AgricolaDH_GApp.Controllers
{
    [Authorize(Roles ="1")]
    public class DashboardController : Controller
	{
        private readonly AppDbContext _context;

        public DashboardController(ILogger<DashboardController> logger, AppDbContext context)
        {
			_context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetChartData()
        {
            // Group by ProductBarCodeID and count items
            var data = await _context.Productos
                .GroupBy(p => p.PN)
                .Select(g => new
                {
                    ProductBarCodeID = g.Key, // Group key
                    Quantity = g.Count() // Count of items in the group
                })
                .ToListAsync();

            return Json(data);
        }

        // ======================================== KPIs ========================================

        [HttpGet]
        public async Task<IActionResult> ventaPorRancho(string? ranchoName)
        {
            // Calcular total general como decimal
            var totalGeneral = await _context.ProductosOrdenar.SumAsync(p => p.Total);

            var query = _context.ProductosOrdenar
                .Join(
                    _context.OrdenesDeCompra,
                    po => po.IdOrdenDeCompra,
                    oc => oc.IdOrdenDeCompra,
                    (po, oc) => new { po.Total, oc.IdRancho }
                )
                .Join(
                    _context.Ranchos,
                    temp => temp.IdRancho,
                    r => r.IdRancho,
                    (temp, r) => new { temp.Total, r.Descripcion });

            if (!string.IsNullOrEmpty(ranchoName))
            {
                query = query.Where(r => r.Descripcion == ranchoName);
            }

            var data = await query
                .GroupBy(x => x.Descripcion)
                .Select(g => new
                {
                    Rancho = g.Key,
                    Porcentaje = totalGeneral == 0
                        ? 0
                        : Math.Round((decimal)g.Sum(x => x.Total) / (decimal)totalGeneral, 4) // <-- Forzar decimal y redondear
                })
                .ToListAsync();

            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> opcionesRancho()
        {
            var ranchos = await _context.Ranchos
                .Select(o => o.Descripcion) // Select only the IdRancho
                .Distinct() // Ensure no duplicates
                .ToListAsync();

            return Json(ranchos); // Return the list of IdRancho values as JSON
        }

        [HttpGet]
        public async Task<IActionResult> ordenDeCompraPorStatus(string? descripcionStatus)
        {
            var query = _context.OrdenesDeCompra
            .Join(_context.OrdenDeCompraStatus,
                  orden => orden.IdOrdenDeCompraStatus,
                  status => status.IdOrdenDeCompraStatus,
                  (orden, status) => new { orden.IdOrdenDeCompra, status.Descripcion });

            // Filtro opcional por la descripción del statusstatus
            query = query.Where(x => x.Descripcion == descripcionStatus);

            var data = await query
                .GroupBy(x => x.Descripcion)
                .Select(g => new
                {
                    status = g.Key,
                    Cantidad = g.Count() // Contar el número de IdOrdenDeCompra
                })
                .ToListAsync();

            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> opcionesStatus()
        {
            var status = await _context.OrdenDeCompraStatus
                .Select(o => o.Descripcion)
                .Distinct()
                .OrderBy(d => d) // Ordenar alfabéticamente las opciones
                .ToListAsync();

            return Json(status);
        }

        [HttpGet]
        public async Task<IActionResult> almacenPorMovimiento(string? descripcionMovimiento)
        {
            var query = _context.Almacen
            .GroupBy(a => a.Movimiento) // Group by Movimiento
            .Select(g => new
            {
                Movimiento = g.Key,
                Cantidad = g.Count() // Count IdAlmacen
            });

            // Optional filter by Movimiento
            if (descripcionMovimiento != null)
            {

            }
            query = query.Where(x => x.Movimiento == descripcionMovimiento);
            var data = await query.ToListAsync();

            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> opcionesMovimiento()
        {
            var status = await _context.Almacen
                .Select(o => o.Movimiento)
                .Distinct()
                .OrderBy(d => d) // Ordenar alfabéticamente las opciones
                .ToListAsync();

            return Json(status);
        }

        // ======================================== Primer Linea de graficas ========================================

        [HttpGet]
        public async Task<IActionResult> ordenesPorArea()
        {
            var query = _context.OrdenesDeCompra
                .Where(oc => oc.FechaRequisicion.Month == DateTime.Now.Month &&
                             oc.FechaRequisicion.Year == DateTime.Now.Year)
                .Join(_context.Areas,
                      oc => oc.IdArea,
                      a => a.IdArea,
                      (oc, a) => new { a.Descripcion, oc.IdOrdenDeCompra })
                .GroupBy(x => x.Descripcion)
                .Select(g => new
                {
                    Area = g.Key,
                    Cantidad = g.Count()
                });

            var data = await query.ToListAsync();

            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> ordenesPorCultivo()
        {
            var query = _context.OrdenesDeCompra
                .Where(oc => oc.FechaRequisicion.Month == DateTime.Now.Month &&
                             oc.FechaRequisicion.Year == DateTime.Now.Year)
                .Join(_context.Cultivos,
                      oc => oc.IdCultivo,
                      c => c.IdCultivo,
                      (oc, c) => new { c.Descripcion, oc.IdOrdenDeCompra })
                .GroupBy(x => x.Descripcion)
                .Select(g => new
                {
                    Cultivo = g.Key,
                    Cantidad = g.Count()
                });

            var data = await query.ToListAsync();

            return Json(data);
        }

        // ======================================== Segunda Linea de graficas ========================================

        [HttpGet]
        public async Task<IActionResult> ordenesPorEtapa()
        {
            var query = _context.OrdenesDeCompra
                .Where(oc => oc.FechaRequisicion.Month == DateTime.Now.Month &&
                             oc.FechaRequisicion.Year == DateTime.Now.Year)
                .Join(_context.Etapas,
                      oc => oc.IdEtapa,
                      e => e.IdEtapa,
                      (oc, e) => new { e.Descripcion, oc.IdOrdenDeCompra })
                .GroupBy(x => x.Descripcion)
                .Select(g => new
                {
                    Etapa = g.Key,
                    Cantidad = g.Count()
                });

            var data = await query.ToListAsync();

            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> ordenesPorProveedor()
        {
            var query = _context.OrdenesDeCompra
                .Where(oc => oc.FechaRequisicion.Month == DateTime.Now.Month &&
                             oc.FechaRequisicion.Year == DateTime.Now.Year)
                .Join(_context.Proveedores,
                      oc => oc.IdProveedor,
                      p => p.IdProveedor,
                      (oc, p) => new { p.Nombre, oc.IdOrdenDeCompra })
                .GroupBy(x => x.Nombre)
                .Select(g => new
                {
                    Proveedor = g.Key,
                    Cantidad = g.Count()
                });

            var data = await query.ToListAsync();

            return Json(data);
        }

        // ======================================== Tercer Linea de graficas ========================================

        [HttpGet]
        public async Task<IActionResult> GastoPorEtapa()
        {
            var query = _context.ProductosOrdenar
            .Join(
                _context.OrdenesDeCompra,
                po => po.IdOrdenDeCompra,
                oc => oc.IdOrdenDeCompra,
                (po, oc) => new { po.Total, oc.IdEtapa }
            )
            .GroupBy(x => x.IdEtapa)
            .Select(g => new
            {
                IdEtapa = g.Key,
                TotalPorEtapa = Math.Round(
                    g.Sum(x => (double?)x.Total) ?? 0,
                    1
                )
            });

            var data = await query.ToListAsync();

            return Json(data);

        }

        [HttpGet]
        public async Task<IActionResult> ProductoEnAlmacen()
        {
            var now = DateTime.Now;

            var start = new DateTime(now.Year, now.Month, 1);
            var end = start.AddMonths(1);

            var query = _context.Almacen
                .Where(a => a.Fecha >= start && a.Fecha < end)
                .Join(
                    _context.Productos,
                    a => a.IdProducto,
                    p => p.IdProducto,
                    (a, p) => new { a.Movimiento, p.NombreProducto }
                )
                .GroupBy(x => x.NombreProducto)
                .Select(g => new
                {
                    Producto = g.Key,
                    Entradas = g.Count(x => x.Movimiento == "Entrada"),
                    Salidas = g.Count(x => x.Movimiento == "Salida")
                });


            var data = await query.ToListAsync();

            return Json(data);


        }

        [HttpGet]
        public async Task<IActionResult> ingresosPorMes()
        {
            var data = await _context.ProductosOrdenar
            .Join(_context.OrdenesDeCompra,
                    productos => productos.IdOrdenDeCompra,
                    ordenes => ordenes.IdOrdenDeCompra,
                    (productos, ordenes) => new { productos.Total, ordenes.FechaOrdenDeCompra })
            .GroupBy(x => x.FechaOrdenDeCompra)
            .Select(g => new
            {
                FechaOrden = g.Key,
                Ingreso = g.Sum(x => x.Total)
            })
            .ToListAsync();

            return Json(data); // Return JSON response
        }



        // ======================================== stuff lol ========================================

        private readonly ILogger<DashboardController> _logger;

		[HttpGet]
		public IActionResult Index()
		{
			return PartialView("~/Views/Dashboard/Index.cshtml");
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}