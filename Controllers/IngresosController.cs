using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.Services;
using AgricolaDH_GApp.Services.Admin;
using AgricolaDH_GApp.ViewModels;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AgricolaDH_GApp.Controllers
{
    public class IngresosController : Controller
    {
        private readonly ILogger<IngresosController> _logger;
        private readonly OrdenDeCompraService _ordenDeCompraService;
        private readonly AlmacenService _almacenService;
        private readonly ProductoService _productoService;
        private readonly SerialMapService _serialMapService;
        private readonly LogsAlmacenService _logsAlmacenService;
        private readonly AppDbContext _context;

        public IngresosController(
            ILogger<IngresosController> logger,
            OrdenDeCompraService ordenDeCompraService,
            AlmacenService almacenService,
            ProductoService productoService,
            SerialMapService serialMapService,
            LogsAlmacenService logsAlmacenService,
            AppDbContext context)
        {
            _logger = logger;
            _ordenDeCompraService = ordenDeCompraService;
            _almacenService = almacenService;
            _productoService = productoService;
            _serialMapService = serialMapService;
            _logsAlmacenService = logsAlmacenService;
            _context = context;
        }

        [HttpGet]
        public IActionResult Index(int closedWeeks = 1)
        {
            int idUsuario = Convert.ToInt32(HttpContext.Session.GetInt32("IdUsuario"));

            var model = new SubirFacturaVM
            {
                // Abiertas: sin filtro de semanas
                subirFacturaList = _ordenDeCompraService.SelectOrdenDeCompraTableList(4, idUsuario, 0),
                // Cerradas: con el parámetro que venga del dropdown (1, 2 o 0 = sin filtro)
                ordenesCerradas = _ordenDeCompraService.SelectOrdenDeCompraTableList(5, idUsuario, closedWeeks)
            };

            ViewBag.ClosedWeeks = closedWeeks; // para que el dropdown recuerde el valor
            return PartialView("~/Views/Ingresos/Index.cshtml", model);
        }

        [HttpPost]
        public IActionResult RealizarIngreso(int idOrdenDeCompra)
        {
            SubirFacturaVM model = new SubirFacturaVM
            {
                ordenDeCompra = _ordenDeCompraService.SelectOrdenDeCompra(idOrdenDeCompra),
                productosOrdenar = _ordenDeCompraService.SelectProductosOrdenarSelected(idOrdenDeCompra)
            };

            return PartialView("~/Views/Ingresos/IngresoForm.cshtml", model);
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

        [HttpGet]
        public JsonResult GetProductBarcodeID(string nombreProducto, [FromServices] ProductoService productoService)
        {
            var producto = productoService.SelectProductoByName(nombreProducto);

            if (producto != null)
                return Json(new { success = true, barcodeID = producto.PN });

            return Json(new { success = false, message = "Producto no encontrado." });
        }

        [HttpPost]
        public IActionResult ActualizarPorRecibir([FromBody] List<ProductoRecibidoDTO> receivedProducts)
        {
            int res = 0;
            if (receivedProducts == null || receivedProducts.Count == 0)
                return Json(new { success = false });

            // Get the purchase order and its order number (adjust if you use a different field)
            int idOrdenDeCompra = _ordenDeCompraService
                .SelectProductoOrdenar(receivedProducts[0].IdProductoOrdenar)?.IdOrdenDeCompra ?? 0;

            var orden = _ordenDeCompraService.SelectOrdenDeCompra(idOrdenDeCompra);
            var orderNumber = orden?.IdOrdenDeCompra.ToString() ?? string.Empty;

            foreach (var item in receivedProducts)
            {
                var productoOrdenar = _ordenDeCompraService.SelectProductoOrdenar(item.IdProductoOrdenar);
                if (productoOrdenar == null) continue;

                // Update PorRecibir
                productoOrdenar.PorRecibir = item.PorRecibir;
                res = _ordenDeCompraService.UpdateProductoOrdenar(productoOrdenar);

                // Get PN for SerialMap insert
                var producto = _productoService.SelectProducto(productoOrdenar.IdProducto);
                if (producto == null) continue;

                // Modelo para Almacen y Logs
                AlmacenVM model = new AlmacenVM
                {
                    almacenLista = new List<Almacen>(),
                };
                // Save the short 12-char serials to SerialMap
                if (item.SerialesCortos != null)
                {

                    foreach (var serial in item.SerialesCortos)
                    {
                        try
                        {
                            _serialMapService.InsertSerial(
                                serialKey: (serial ?? string.Empty).ToUpperInvariant(),
                                orderNumber: orderNumber,
                                partNumber: producto.PN ?? string.Empty
                            );
                            _almacenService.GuardarEnAlmacen(producto.IdProducto, serial);

                            // Registro de log en Almacen
                            Almacen a = _context.Almacen.
                                Single(x => x.IdProducto == producto.IdProducto && x.SerialNumber == serial);
                            model.almacenLista.Add(a);
                        }
                        catch (System.Exception ex)
                        {
                            // Ignore duplicates or log as needed
                            _logger.LogWarning(ex, "SerialMap insert failed for {SerialKey}", serial);
                        }
                    }
                    //Agregar logs
                    model.logsAlmacen = _logsAlmacenService.InsertarLogsAlmacen(model);
                    _logsAlmacenService.InsertarLogsAlmacenProductos(model);
                }
            }

            var productosOrden = _ordenDeCompraService.SelectProductosOrdenarSelected(idOrdenDeCompra);
            bool allReceived = productosOrden.All(p => p.PorRecibir == 0);
            if (allReceived)
            {
                res = _ordenDeCompraService.UpdateOrdenDeCompraStatus(idOrdenDeCompra, 5);
            }

            return Json(new { success = (res == 0) });
        }

        [HttpPost]
        public IActionResult VerOrden(int idOrdenDeCompra)
        {
            SubirFacturaVM model = new SubirFacturaVM
            {
                ordenDeCompra = _ordenDeCompraService.SelectOrdenDeCompra(idOrdenDeCompra),
                productosOrdenar = _ordenDeCompraService.SelectProductosOrdenarSelected(idOrdenDeCompra)
            };

            return PartialView("~/Views/Ingresos/IngresoForm.cshtml", model);
        }
    }
}