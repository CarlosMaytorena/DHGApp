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
    public class CultivosController : Controller
    {
        private readonly ILogger<CultivosController> _logger;

        private readonly AppDbContext context;
        private CultivoService cultivoService;
        private ViewRenderService renderService;


        public CultivosController(ILogger<CultivosController> logger, AppDbContext _ctx, CultivoService _cultivoService, ViewRenderService _renderService)
        {
            _logger = logger;
            context = _ctx;
            cultivoService = _cultivoService;
            renderService = _renderService;
        }

        [HttpGet]
        public IActionResult Index()
        {

            CultivosVM model = new CultivosVM();
            model.cultivoList = cultivoService.SelectCultivos();

            return PartialView("~/Views/Admin/Cultivos/Index.cshtml", model);
        }

        //[HttpPost]
        //public IActionResult CrearProveedores()
        //{
        //    ProveedoresVM model = new ProveedoresVM();

        //    return PartialView("~/Views/Admin/Proveedores/Proveedor.cshtml", model);
        //}

        [HttpPost]
        public IActionResult EditarCultivo(int IdCultivo)
        {
            CultivosVM model = new CultivosVM();

            model.cultivo = context.Cultivos.Find(IdCultivo);

            return PartialView("~/Views/Admin/Cultivos/Cultivo.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> InsertCultivo(CultivosVM model)
        {
            int res = cultivoService.InsertCultivo(model.cultivo);

            model = new CultivosVM();
            model.cultivoList = cultivoService.SelectCultivos();

            return Json(new { res, url = await renderService.RenderViewToStringAsync("~/Views/Admin/Cultivos/Index.cshtml", model) });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCultivo(CultivosVM model)
        {
            int res = cultivoService.UpdateCultivo(model.cultivo);

            model = new CultivosVM();
            model.cultivoList = cultivoService.SelectCultivos();

            return Json(new { res, url = await renderService.RenderViewToStringAsync("~/Views/Admin/Cultivos/Index.cshtml", model) });
        }


        [HttpPost]
        public async Task<IActionResult> BorrarCultivo(int IdCultivo)
        {

            int res = cultivoService.DeleteCultivo(IdCultivo);

            CultivosVM model = new CultivosVM();
            model.cultivoList = cultivoService.SelectCultivos();

            return Json(new {res, url = await renderService.RenderViewToStringAsync("~/Views/Admin/Cultivos/Index.cshtml", model) });
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