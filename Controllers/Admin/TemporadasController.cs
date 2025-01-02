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
    public class TemporadasController : Controller
    {
        private readonly ILogger<TemporadasController> _logger;

        private readonly AppDbContext context;
        private TemporadaService temporadaService;
        private ViewRenderService renderService;


        public TemporadasController(ILogger<TemporadasController> logger, AppDbContext _ctx, TemporadaService _temporadaService, ViewRenderService _renderService)
        {
            _logger = logger;
            context = _ctx;
            temporadaService = _temporadaService;
            renderService = _renderService;
        }

        [HttpGet]
        public IActionResult Index()
        {

            TemporadasVM model = new TemporadasVM();
            model.temporadaList = temporadaService.SelectTemporadas();

            return PartialView("~/Views/Admin/Temporadas/Index.cshtml", model);
        }

        //[HttpPost]
        //public IActionResult CrearProveedores()
        //{
        //    ProveedoresVM model = new ProveedoresVM();

        //    return PartialView("~/Views/Admin/Proveedores/Proveedor.cshtml", model);
        //}

        [HttpPost]
        public IActionResult EditarTemporada(int IdTemporada)
        {
            TemporadasVM model = new TemporadasVM();

            model.temporada = temporadaService.SelectTemporada(IdTemporada);

            return PartialView("~/Views/Admin/Temporadas/Temporada.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> InsertTemporada(TemporadasVM model)
        {
            int res = temporadaService.InsertTemporada(model.temporada);

            model = new TemporadasVM();
            model.temporadaList = temporadaService.SelectTemporadas();

            return Json(new { res, url = await renderService.RenderViewToStringAsync("~/Views/Admin/Temporadas/Index.cshtml", model) });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTemporada(TemporadasVM model)
        {
            int res = temporadaService.UpdateTemporada(model.temporada);

            model = new TemporadasVM();
            model.temporadaList = temporadaService.SelectTemporadas();

            return Json(new { res, url = await renderService.RenderViewToStringAsync("~/Views/Admin/Temporadas/Index.cshtml", model) });
        }


        [HttpPost]
        public async Task<IActionResult> BorrarTemporada(int IdTemporada)
        {

            int res = temporadaService.DeleteTemporada(IdTemporada);

            TemporadasVM model = new TemporadasVM();
            model.temporadaList = temporadaService.SelectTemporadas();

            return Json(new {res, url = await renderService.RenderViewToStringAsync("~/Views/Admin/Temporadas/Index.cshtml", model) });
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