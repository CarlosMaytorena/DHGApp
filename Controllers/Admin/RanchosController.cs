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
    public class RanchosController : Controller
    {
        private readonly ILogger<RanchosController> _logger;

        private readonly AppDbContext context;
        private RanchoService ranchoService;
        private ViewRenderService renderService;


        public RanchosController(ILogger<RanchosController> logger, AppDbContext _ctx, RanchoService _ranchoService, ViewRenderService _renderService)
        {
            _logger = logger;
            context = _ctx;
            ranchoService = _ranchoService;
            renderService = _renderService;
        }

        [HttpGet]
        public IActionResult Index()
        {

            RanchosVM model = new RanchosVM();
            model.ranchoList = ranchoService.SelectRanchos();

            return PartialView("~/Views/Admin/Ranchos/Index.cshtml", model);
        }

        //[HttpPost]
        //public IActionResult CrearProveedores()
        //{
        //    ProveedoresVM model = new ProveedoresVM();

        //    return PartialView("~/Views/Admin/Proveedores/Proveedor.cshtml", model);
        //}

        [HttpPost]
        public IActionResult EditarRancho(int IdRancho)
        {
            RanchosVM model = new RanchosVM();

            model.rancho = ranchoService.SelectRancho(IdRancho);

            return PartialView("~/Views/Admin/Ranchos/Rancho.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> InsertRancho(RanchosVM model)
        {
            int res = ranchoService.InsertRancho(model.rancho);

            model = new RanchosVM();
            model.ranchoList = ranchoService.SelectRanchos();

            return Json(new { res, url = await renderService.RenderViewToStringAsync("~/Views/Admin/Ranchos/Index.cshtml", model) });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRancho(RanchosVM model)
        {
            int res = ranchoService.UpdateRancho(model.rancho);

            model = new RanchosVM();
            model.ranchoList = ranchoService.SelectRanchos();

            return Json(new { res, url = await renderService.RenderViewToStringAsync("~/Views/Admin/Ranchos/Index.cshtml", model) });
        }


        [HttpPost]
        public async Task<IActionResult> BorrarRancho(int IdRancho)
        {

            int res = ranchoService.DeleteRancho(IdRancho);

            RanchosVM model = new RanchosVM();
            model.ranchoList = ranchoService.SelectRanchos();

            return Json(new {res, url = await renderService.RenderViewToStringAsync("~/Views/Admin/Ranchos/Index.cshtml", model) });
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