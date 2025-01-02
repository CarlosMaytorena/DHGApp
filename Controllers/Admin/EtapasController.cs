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
    public class EtapasController : Controller
    {
        private readonly ILogger<EtapasController> _logger;

        private readonly AppDbContext context;
        private EtapaService etapaService;
        private ViewRenderService renderService;


        public EtapasController(ILogger<EtapasController> logger, AppDbContext _ctx, EtapaService _etapaService, ViewRenderService _renderService)
        {
            _logger = logger;
            context = _ctx;
            etapaService = _etapaService;
            renderService = _renderService;
        }

        [HttpGet]
        public IActionResult Index()
        {

            EtapasVM model = new EtapasVM();
            model.etapaList = etapaService.SelectEtapas();

            return PartialView("~/Views/Admin/Etapas/Index.cshtml", model);
        }

        //[HttpPost]
        //public IActionResult CrearProveedores()
        //{
        //    ProveedoresVM model = new ProveedoresVM();

        //    return PartialView("~/Views/Admin/Proveedores/Proveedor.cshtml", model);
        //}

        [HttpPost]
        public IActionResult EditarEtapa(int IdEtapa)
        {
            EtapasVM model = new EtapasVM();

            model.etapa = etapaService.SelectEtapa(IdEtapa);

            return PartialView("~/Views/Admin/Etapas/Etapa.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> InsertEtapa(EtapasVM model)
        {
            int res = etapaService.InsertEtapa(model.etapa);

            model = new EtapasVM();
            model.etapaList = etapaService.SelectEtapas();

            return Json(new { res, url = await renderService.RenderViewToStringAsync("~/Views/Admin/Etapas/Index.cshtml", model) });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateEtapa(EtapasVM model)
        {
            int res = etapaService.UpdateEtapa(model.etapa);

            model = new EtapasVM();
            model.etapaList = etapaService.SelectEtapas();

            return Json(new { res, url = await renderService.RenderViewToStringAsync("~/Views/Admin/Etapas/Index.cshtml", model) });
        }


        [HttpPost]
        public async Task<IActionResult> BorrarEtapa(int IdEtapa)
        {

            int res = etapaService.DeleteEtapa(IdEtapa);

            EtapasVM model = new EtapasVM();
            model.etapaList = etapaService.SelectEtapas();

            return Json(new {res, url = await renderService.RenderViewToStringAsync("~/Views/Admin/Etapas/Index.cshtml", model) });
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