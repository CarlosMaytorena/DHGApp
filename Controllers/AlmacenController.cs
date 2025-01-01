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
            AlmacenVM model = new AlmacenVM();
            model.almacenList = almacenService.SelectAlmacen();
            return PartialView("~/Views/Almacen/Index.cshtml", model);
		}
		[HttpGet]
        public IActionResult EntradaFormato()
        {
            return PartialView("~/Views/Almacen/EntradaFormato.cshtml");
        }
		[HttpGet]
        public IActionResult SalidaFormato()
        {
            return PartialView("~/Views/Almacen/SalidaFormato.cshtml");
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