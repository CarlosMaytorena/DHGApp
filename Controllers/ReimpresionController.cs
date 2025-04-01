using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.Services.Admin;
using AgricolaDH_GApp.ViewModels;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AgricolaDH_GApp.Controllers
{

    public class ReimpresionController : Controller
    {
        private readonly OrdenDeCompraService _ordenDeCompraService;
        private readonly ProductoService _productoService;

        public ReimpresionController(OrdenDeCompraService ordenDeCompraService, ProductoService productoService)
        {
            _ordenDeCompraService = ordenDeCompraService;
            _productoService = productoService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return PartialView("~/Views/Reimpresion/Index.cshtml");
        }
        [HttpGet]
        public JsonResult ValidarOrdenYProducto(int ordenDeCompra, string nombreProducto)
        {
            var orden = _ordenDeCompraService.SelectOrdenDeCompra(ordenDeCompra);
            if (orden == null)
            {
                return Json(new { success = false, message = "Orden de compra no encontrada." });
            }

            var dbProducto = _productoService.SelectProductoByBarcodeID(nombreProducto);
            if (dbProducto == null)
            {
                return Json(new { success = false, message = "Producto no encontrado." });
            }

            return Json(new
            {
                success = true,
                productName = dbProducto.NombreProducto,
                productBarcodeID = dbProducto.PN
            });
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