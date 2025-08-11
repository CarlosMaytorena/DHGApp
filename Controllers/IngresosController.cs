using AgricolaDH_GApp.Models;
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

        public IngresosController(ILogger<IngresosController> logger, OrdenDeCompraService ordenDeCompraService, AlmacenService almacenService, ProductoService productoService)
        {
            _logger = logger;
            _ordenDeCompraService = ordenDeCompraService;
            _almacenService = almacenService;
            _productoService = productoService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            int idUsuario = Convert.ToInt32(HttpContext.Session.GetInt32("IdUsuario"));

            SubirFacturaVM model = new SubirFacturaVM
            {
                subirFacturaList = _ordenDeCompraService.SelectOrdenDeCompraTableList(4, idUsuario), // Status 4 for Ingresos
                ordenesCerradas = _ordenDeCompraService.SelectOrdenDeCompraTableList(5, idUsuario)  // Status 5: Closed

            };
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
            // Use the service to retrieve the product
            var producto = productoService.SelectProductoByName(nombreProducto);

            if (producto != null)
            {
                // Return the barcode if the product is found
                return Json(new { success = true, barcodeID = producto.PN });
            }

            // Return an error message if the product is not found
            return Json(new { success = false, message = "Producto no encontrado." });
        }

        [HttpPost]
        public IActionResult ActualizarPorRecibir([FromBody] List<ProductoRecibidoDTO> receivedProducts)
        {
            int res = 0;

            if (receivedProducts == null || receivedProducts.Count == 0)
                return Json(new { success = false });

            int idOrdenDeCompra = _ordenDeCompraService
                .SelectProductoOrdenar(receivedProducts[0].IdProductoOrdenar)?.IdOrdenDeCompra ?? 0;

            foreach (var item in receivedProducts)
            {
                var productoOrdenar = _ordenDeCompraService.SelectProductoOrdenar(item.IdProductoOrdenar);

                if (productoOrdenar != null)
                {
                    productoOrdenar.PorRecibir = item.PorRecibir;
                    res = _ordenDeCompraService.UpdateProductoOrdenar(productoOrdenar);

                    //Get IdProducto to store in Almacen
                    var producto = _productoService.SelectProducto(productoOrdenar.IdProducto);

                    if (producto != null && item.Seriales != null)
                    {
                        foreach (var serial in item.Seriales)
                        {
                            _almacenService.GuardarEnAlmacen(producto.IdProducto, serial);
                        }
                    }
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
