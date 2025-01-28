using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.Services.Admin;
using AgricolaDH_GApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Diagnostics;

namespace AgricolaDH_GApp.Controllers.Admin
{
    public class ProductosController : Controller
    {
        private readonly ILogger<ProductosController> _logger;

        private readonly AppDbContext context;
        private ProductoService productoService;
        private ViewRenderService renderService;

        public ProductosController(ILogger<ProductosController> logger, AppDbContext _ctx,
            ProductoService _productoService, ViewRenderService _renderService)
        {
            _logger = logger;
            context = _ctx;
            productoService = _productoService;
            renderService = _renderService;
        }

        [HttpGet]
        public IActionResult Index()
        {

            ProductosVM model = new ProductosVM();

            model.productoList = productoService.SelectProductos();

            return PartialView("~/Views/Admin/Productos/Index.cshtml", model);
        }

        [HttpPost]
        public IActionResult AgregarProducto()
        {
            ProductosVM model = new ProductosVM();

            return PartialView("~/Views/Admin/Productos/Productos.cshtml", model);
        }

        [HttpPost]
        public IActionResult EditarProducto(int IdProducto)
        {
            ProductosVM model = new ProductosVM();

            model.producto = productoService.SelectProducto(IdProducto);

            return PartialView("~/Views/Admin/Productos/Productos.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> InsertProducto(ProductosVM model)
        {
            int res = productoService.InsertProducto(model.producto);

            if (res == -2)
            {
                return Json(new { res, message = "El nombre del producto ya existe. Por favor, elija un nombre diferente." });
            }
            else if (res == -1)
            {
                return Json(new { res, message = "Hubo un error al realizar la transacción. Favor de contactar al administrador." });
            }

            model = new ProductosVM();
            model.productoList = productoService.SelectProductos();

            return Json(new { res, url = await renderService.RenderViewToStringAsync("~/Views/Admin/Productos/Index.cshtml", model) });
        }


        [HttpPost]
        public async Task<IActionResult> UpdateProducto(ProductosVM model)
        {
            int res = productoService.UpdateProducto(model.producto);


            model = new ProductosVM();
            model.productoList = productoService.SelectProductos();

            return Json(new { res, url = await renderService.RenderViewToStringAsync("~/Views/Admin/Productos/Index.cshtml", model) });

        }

        [HttpPost]
        public async Task<IActionResult> BorrarProducto(int IdProducto)
        {

            int res = productoService.DeleteProducto(IdProducto);

            ProductosVM model = new ProductosVM();
            model.productoList = productoService.SelectProductos();

            return Json(new { res, url = await renderService.RenderViewToStringAsync("~/Views/Admin/Productos/Index.cshtml", model) });
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