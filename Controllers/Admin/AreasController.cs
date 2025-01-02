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
    public class AreasController : Controller
    {
        private readonly ILogger<AreasController> _logger;

        private readonly AppDbContext context;
        private AreaService areaService;
        private ViewRenderService renderService;


        public AreasController(ILogger<AreasController> logger, AppDbContext _ctx, AreaService _areaService, ViewRenderService _renderService)
        {
            _logger = logger;
            context = _ctx;
            areaService = _areaService;
            renderService = _renderService;
        }

        [HttpGet]
        public IActionResult Index()
        {

            AreasVM model = new AreasVM();
            model.areaList = areaService.SelectAreas();

            return PartialView("~/Views/Admin/Areas/Index.cshtml", model);
        }

        //[HttpPost]
        //public IActionResult CrearProveedores()
        //{
        //    ProveedoresVM model = new ProveedoresVM();

        //    return PartialView("~/Views/Admin/Proveedores/Proveedor.cshtml", model);
        //}

        [HttpPost]
        public IActionResult EditarArea(int IdArea)
        {
            AreasVM model = new AreasVM();

            model.area = areaService.SelectArea(IdArea);

            return PartialView("~/Views/Admin/Areas/Area.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> InsertArea(AreasVM model)
        {
            int res = areaService.InsertArea(model.area);

            model = new AreasVM();
            model.areaList = areaService.SelectAreas();

            return Json(new { res, url = await renderService.RenderViewToStringAsync("~/Views/Admin/Areas/Index.cshtml", model) });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateArea(AreasVM model)
        {
            int res = areaService.UpdateArea(model.area);

            model = new AreasVM();
            model.areaList = areaService.SelectAreas();

            return Json(new { res, url = await renderService.RenderViewToStringAsync("~/Views/Admin/Areas/Index.cshtml", model) });
        }


        [HttpPost]
        public async Task<IActionResult> BorrarArea(int IdArea)
        {

            int res = areaService.DeleteArea(IdArea);

            AreasVM model = new AreasVM();
            model.areaList = areaService.SelectAreas();

            return Json(new {res, url = await renderService.RenderViewToStringAsync("~/Views/Admin/Areas/Index.cshtml", model) });
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