using AgricolaDH_GApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AgricolaDH_GApp.Controllers
{
	public class EgresosController : Controller
	{
		private readonly ILogger<EgresosController> _logger;

		public EgresosController(ILogger<EgresosController> logger)
		{
			_logger = logger;
		}

		[HttpGet]
		public IActionResult Index()
		{
			return PartialView("~/Views/Egresos/Index.cshtml");
		}
		[HttpGet]
        public IActionResult EgresoForm()
        {
            return PartialView("~/Views/Egresos/EgresoForm.cshtml");
        }
		[HttpGet]
        public IActionResult GenerarEgreso()
        {
            return PartialView("~/Views/Egresos/Index.cshtml");
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