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
    public class ProveedoresController : Controller
    {
        private readonly ILogger<ProveedoresController> _logger;

        private readonly AppDbContext context;
        private ProveedorService proveedorService;
        private ViewRenderService renderService;


        public ProveedoresController(ILogger<ProveedoresController> logger, AppDbContext _ctx, ProveedorService _proveedorService, ViewRenderService _renderService)
        {
            _logger = logger;
            context = _ctx;
            proveedorService = _proveedorService;
            renderService = _renderService;
        }

        [HttpGet]
        public IActionResult Index()
        {

            ProveedoresVM model = new ProveedoresVM();
            model.proveedorList = proveedorService.SelectProveedores();

            return PartialView("~/Views/Admin/Proveedores/Index.cshtml", model);
        }

        //[HttpPost]
        //public IActionResult CrearProveedores()
        //{
        //    ProveedoresVM model = new ProveedoresVM();

        //    return PartialView("~/Views/Admin/Proveedores/Proveedor.cshtml", model);
        //}

        [HttpPost]
        public IActionResult EditarProveedor(int IdProveedor)
        {
            ProveedoresVM model = new ProveedoresVM();

            model.proveedor = proveedorService.SelectProveedor(IdProveedor);

            return PartialView("~/Views/Admin/Proveedores/Proveedor.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> InsertProveedor(ProveedoresVM model)
        {
            int res = proveedorService.InsertProveedor(model.proveedor);

            model = new ProveedoresVM();
            model.proveedorList = proveedorService.SelectProveedores();

            return Json(new { res, url = await renderService.RenderViewToStringAsync("~/Views/Admin/Proveedores/Index.cshtml", model) });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProveedor(ProveedoresVM model)
        {
            int res = proveedorService.UpdateProveedor(model.proveedor);

            model = new ProveedoresVM();
            model.proveedorList = proveedorService.SelectProveedores();

            return Json(new { res, url = await renderService.RenderViewToStringAsync("~/Views/Admin/Proveedores/Index.cshtml", model) });
        }


        [HttpPost]
        public async Task<IActionResult> BorrarProveedor(int IdProveedor)
        {

            int res = proveedorService.DeleteProveedor(IdProveedor);

            ProveedoresVM model = new ProveedoresVM();
            model.proveedorList = proveedorService.SelectProveedores();

            return Json(new {res, url = await renderService.RenderViewToStringAsync("~/Views/Admin/Proveedores/Index.cshtml", model) });
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