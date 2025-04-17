using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;

namespace AgricolaDH_GApp.Controllers
{
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

        [HttpGet]
        public async Task<IActionResult> IngresosEgresosPorMes()
        {
            var data = await _context.OrdenesDeCompra
        .Where(o => o.FechaOrdenDeCompra.HasValue && o.FechaOrdenDeCompra.Value.Year == DateTime.Now.Year)
        .Join(
            _context.ProductosOrdenar,
            o => o.IdOrdenDeCompra,
            p => p.IdOrdenDeCompra,
            (o, p) => new { o.FechaOrdenDeCompra, p.Total }
        )
        .GroupBy(g => g.FechaOrdenDeCompra.HasValue ? g.FechaOrdenDeCompra.Value.Month : 0)
        .Select(g => new
        {
            Month = g.Key,  // Mes de la orden de compra
            Total = g.Sum(x => x.Total) // Suma del total de productos ordenados
        })
        .ToListAsync();

            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> IngresosEgresosPorDia()
        {
            var data = await _context.OrdenesDeCompra
        .Where(o => o.FechaOrdenDeCompra.HasValue && o.FechaOrdenDeCompra.Value.Month == DateTime.Now.Month && o.FechaOrdenDeCompra.Value.Year == DateTime.Now.Year)
        .Join(
            _context.ProductosOrdenar,
            o => o.IdOrdenDeCompra,
            p => p.IdOrdenDeCompra,
            (o, p) => new { o.FechaOrdenDeCompra, p.Total }
        )
        .GroupBy(g => g.FechaOrdenDeCompra.HasValue ? g.FechaOrdenDeCompra.Value.Day : 0)
        .Select(g => new
        {
            Month = g.Key,  // Mes de la orden de compra
            Total = g.Sum(x => x.Total) // Suma del total de productos ordenados
        })
        .ToListAsync();

            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> TotalPorProducto()
        {
            var query = _context.OrdenesDeCompra
            .Join(_context.ProductosOrdenar,
                  oc => oc.IdOrdenDeCompra,
                  po => po.IdOrdenDeCompra,
                  (oc, po) => new { oc.FechaOrdenDeCompra, po.IdProducto })
            .Join(_context.Productos,
                  temp => temp.IdProducto,
                  p => p.IdProducto,
                  (temp, p) => new { temp.FechaOrdenDeCompra, p.Descripcion })
            .Where(x => x.FechaOrdenDeCompra.HasValue && x.FechaOrdenDeCompra.Value.Month == DateTime.Now.Month) // Filtra por el mes actual
            .GroupBy(x => x.Descripcion) // Agrupa por descripción del producto
            .Select(g => new
            {
                Producto = g.Key,
                Cantidad = g.Count() // Cuenta el número de órdenes de compra
            });

            var data = await query.ToListAsync();

            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> ordenesPorProveedor()
        {
            var query = _context.OrdenesDeCompra
            .Join(_context.Proveedores,
                  oc => oc.IdProveedor,
                  p => p.IdProveedor,
                  (oc, p) => new { oc.FechaOrdenDeCompra, p.Nombre })
            .Where(x => x.FechaOrdenDeCompra.HasValue && x.FechaOrdenDeCompra.Value.Month == DateTime.Now.Month) // Filter by current month
            .GroupBy(x => x.Nombre) // Group by supplier name
            .Select(g => new
            {
                Proveedor = g.Key,
                Cantidad = g.Count() // Count the number of orders
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

        [HttpGet]
        public async Task<IActionResult> ventaPorRancho(string? ranchoName)
        {
            // Calculate total sum from ProductosOrdenar
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

            // If an idRancho is provided, filter by that IdRancho
            query = query.Where(r => r.Descripcion == ranchoName);

            var data = await query
                .GroupBy(x => x.Descripcion)
                .Select(g => new
                {
                    Rancho = g.Key,
                    Porcentaje = totalGeneral == 0 ? 0 : g.Sum(x => x.Total) / totalGeneral
                })
                .ToListAsync();

            return Json(data); // Return JSON response

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