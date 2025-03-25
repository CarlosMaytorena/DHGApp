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

        public IngresosController(ILogger<IngresosController> logger, OrdenDeCompraService ordenDeCompraService)
        {
            _logger = logger;
            _ordenDeCompraService = ordenDeCompraService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            SubirFacturaVM model = new SubirFacturaVM
            {
                subirFacturaList = _ordenDeCompraService.SelectOrdenDeCompraTableList(4) // Status 4 for Ingresos
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
        public IActionResult ActualizarPorRecibir([FromBody] List<ProductoOrdenar> receivedProducts)
        {
            int res = 0;
            foreach (var item in receivedProducts)
            {
                // Fetch existing product data
                var productoOrdenar = _ordenDeCompraService.SelectProductoOrdenar(item.IdProductoOrdenar);

                if (productoOrdenar != null)
                {
                    productoOrdenar.PorRecibir = item.PorRecibir; // Update with new remaining quantity
                    res = _ordenDeCompraService.UpdateProductoOrdenar(productoOrdenar);
                }
            }

            if (res == 0)
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }
        }


    }
}
