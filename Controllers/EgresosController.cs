using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.Services.Admin;
using AgricolaDH_GApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AgricolaDH_GApp.Controllers
{
	public class EgresosController : Controller
	{
        private EgresosVM model = new EgresosVM();

        private readonly ILogger<EgresosController> _logger;
        private readonly AppDbContext context;
		private EgresoService egresoService;
		private ProductoService productoService;
		private UsuarioService usuarioService;
		private AlmacenService almacenService;
		private ViewRenderService renderService;

        public EgresosController(
			ILogger<EgresosController> logger, 
			AppDbContext _ctx, 
			EgresoService _egresoService,
			ViewRenderService _renderService,
            ProductoService _productoService,
			UsuarioService _usuarioService,
			AlmacenService _almacenService)
		{
			_logger = logger;
			context = _ctx;
			egresoService = _egresoService;
			productoService = _productoService;
			usuarioService = _usuarioService;
            almacenService = _almacenService;
			renderService = _renderService;

        }

        [HttpGet]
		public IActionResult Index()
		{
			model.egresosList = egresoService.SelectEgresos();
			return PartialView("~/Views/Egresos/Index.cshtml", model);
		}
		[HttpGet]
        public IActionResult EgresoForm()
        {
			model.productosList = productoService.SelectProductos();
			model.usuariosList = usuarioService.SelectUsuarios();
			model.almacenList = almacenService.SelectAlmacen();
            return PartialView("~/Views/Egresos/EgresoForm.cshtml", model);
        }
		[HttpPost]
        public async Task<IActionResult> GenerarEgreso([FromBody] Egreso egreso)
        {
			int res = egresoService.Generar(egreso);
            model.egresosList = egresoService.SelectEgresos();
            return Json(new { res, url = await renderService.RenderViewToStringAsync("~/Views/Egresos/Index.cshtml", model) });
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