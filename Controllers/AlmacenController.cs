using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.Services.Admin;
using AgricolaDH_GApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AgricolaDH_GApp.Controllers
{
	public class AlmacenController : Controller
	{
        private AlmacenVM model = new AlmacenVM();
        private readonly ILogger<AlmacenController> _logger;

        private readonly AppDbContext context;
		private AlmacenService almacenService;
        private ViewRenderService renderService;

        public AlmacenController(ILogger<AlmacenController> logger, AppDbContext _ctx, ViewRenderService _renderService, AlmacenService _almacenService)
		{
			_logger = logger;
			almacenService = _almacenService;
            context = _ctx;
            renderService = _renderService;
        }

		[HttpGet]
		public IActionResult Index()
		{
            model.almacenList = almacenService.SelectAlmacen();
            return PartialView("~/Views/Almacen/Index.cshtml", model);
		}
		[HttpGet]
        public IActionResult Entrada()
        {
            model.productoList = almacenService.SelectProductos();
            return PartialView("~/Views/Almacen/Entrada.cshtml", model);
        }
		[HttpGet]
        public IActionResult Salida()
        {
            model.productoList = almacenService.SelectProductos();
            return PartialView("~/Views/Almacen/Salida.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> AltaAlmacen([FromBody] AlmacenDTO registro)
        {
            int res= almacenService.EntradaParaAlmacen(registro);
            model.almacenList = almacenService.SelectAlmacen();
            return Json( new {res, url = await renderService.RenderViewToStringAsync("~/Views/Almacen/Index.cshtml", model) });
        }

        [HttpPost]
        public async Task<IActionResult> BajaAlmacen([FromBody] AlmacenDTO registro)
        {
            int res = almacenService.SalidaDeAlmacen(registro);
            model.almacenList = almacenService.SelectAlmacen();
            return Json(new { res, url = await renderService.RenderViewToStringAsync("~/Views/Almacen/Index.cshtml", model) });
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