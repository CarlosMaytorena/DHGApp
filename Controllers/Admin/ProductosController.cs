using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AgricolaDH_GApp.Controllers.Admin
{
    public class ProductosController : Controller
    {
        private readonly ILogger<ProductosController> _logger;

        private readonly AppDbContext context;

        public ProductosController(ILogger<ProductosController> logger, AppDbContext _ctx)
        {
            _logger = logger;
            context = _ctx;
        }

        [HttpGet]
        public IActionResult Index()
        {

            ProductosVM model = new ProductosVM();

            model.productoList = context.Productos.ToList();

            return PartialView("~/Views/Admin/Productos/Index.cshtml", model);
        }

        [HttpPost]
        public IActionResult CrearProducto()
        {
            ProductosVM model = new ProductosVM();

            return PartialView("~/Views/Admin/Productos/Producto.cshtml", model);
        }

        [HttpPost]
        public IActionResult EditarProducto(int IdProducto)
        {
            ProductosVM model = new ProductosVM();

            model.producto = context.Productos.Find(IdProducto);

            return PartialView("~/Views/Admin/Productos/Productos.cshtml", model);
        }

        [HttpPost]
        public IActionResult InsertProducto(ProductosVM model)
        {
            context.Productos.Add(model.producto);
            context.SaveChanges();

            model = new ProductosVM();
            model.productoList = context.Productos.ToList();

            return PartialView("~/Views/Admin/Productos/Index.cshtml", model);
        }

        [HttpPost]
        public IActionResult UpdateProducto(ProductosVM model)
        {
            context.Productos.Update(model.producto);
            context.SaveChanges();

            model = new ProductosVM();
            model.productoList = context.Productos.ToList();

            return PartialView("~/Views/Admin/Productos/Index.cshtml", model);
        }

        [HttpPost]
        public IActionResult BorrarProducto(int IdProducto)
        {

            ProductosVM model = new ProductosVM();
            model.producto = context.Productos.Find(IdProducto);

            context.Productos.Remove(model.producto);
            context.SaveChanges();

            model = new ProductosVM();
            model.productoList = context.Productos.ToList();

            return PartialView("~/Views/Admin/Productos/Index.cshtml", model);
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