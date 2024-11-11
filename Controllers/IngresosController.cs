using AgricolaDH_GApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AgricolaDH_GApp.Controllers
{
	public class IngresosController : Controller
	{
		private readonly ILogger<IngresosController> _logger;

		public IngresosController(ILogger<IngresosController> logger)
		{
			_logger = logger;
		}

		[HttpGet]
		public IActionResult Index()
		{
			return PartialView("~/Views/Ingresos/Index.cshtml");
		}
        [HttpPost]
        public IActionResult RealizarIngreso()
        {
            return PartialView("~/Views/Ingresos/IngresoForm.cshtml");
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