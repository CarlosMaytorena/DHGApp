using AgricolaDH_GApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AgricolaDH_GApp.Controllers
{
	public class AlmacenController : Controller
	{
		private readonly ILogger<AlmacenController> _logger;

		public AlmacenController(ILogger<AlmacenController> logger)
		{
			_logger = logger;
		}

		[HttpGet]
		public IActionResult Index()
		{
			return PartialView("~/Views/Almacen/Index.cshtml");
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