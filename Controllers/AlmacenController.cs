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
        private MovimientoService movimientoService;
        private UsuarioService usuarioService;
        private ProductoService productoService;
        public AlmacenController(
            ILogger<AlmacenController> logger,
            AppDbContext _ctx,
            ViewRenderService _renderService,
            AlmacenService _almacenService,
            MovimientoService _movimientoService,
            UsuarioService _usuarioService,
            ProductoService _productoService
            )
		{
			_logger = logger;
			almacenService = _almacenService;
            context = _ctx;
            renderService = _renderService;
            movimientoService = _movimientoService;
            usuarioService = _usuarioService;
            productoService = _productoService;
        }

		[HttpGet]
		public IActionResult Index()
		{
            model.almacenList = almacenService.SelectAlmacen();
            model.movimientosList = movimientoService.SelectMovimientos();
            return PartialView("~/Views/Almacen/Index.cshtml", model);
		}
		[HttpGet]
        public IActionResult Entrada()
        {
            model.productoList = almacenService.SelectProductos();
            model.usuariosList = usuarioService.SelectUsuarios();
            return PartialView("~/Views/Almacen/Entrada.cshtml", model);
        }
		[HttpGet]
        public IActionResult Salida()
        {
            model.productoList = almacenService.SelectProductos();
            model.usuariosList = usuarioService.SelectUsuarios();
            return PartialView("~/Views/Almacen/Salida.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> AltaAlmacen([FromBody] AlmacenDTO registro)
        {
            int res = almacenService.Entrada(registro);
            int resp_mov = movimientoService.Entrada(registro);
            if (res < 0 || resp_mov < 0) 
            {
                res = -1;
            }
            model.almacenList = almacenService.SelectAlmacen();
            model.movimientosList = movimientoService.SelectMovimientos();
            return Json( new {res, url = await renderService.RenderViewToStringAsync("~/Views/Almacen/Index.cshtml", model) });
        }

        [HttpPost]
        public async Task<IActionResult> BajaAlmacen([FromBody] AlmacenDTO registro)
        {
            int res = almacenService.Salida(registro);
            int resp_mov = movimientoService.Salida(registro);
            if (res < 0 || resp_mov < 0)
            {
                res = -1;
            }
            model.almacenList = almacenService.SelectAlmacen();
            model.movimientosList = movimientoService.SelectMovimientos();
            return Json(new { res, url = await renderService.RenderViewToStringAsync("~/Views/Almacen/Index.cshtml", model) });
        }

        [HttpPost]
        public async Task<IActionResult> EncontrarProducto([FromBody] Producto registro)
        {
            model.producto = almacenService.SelectProductoByBarcode(registro);
            if (model.producto == null)
            {
                return Json(new { res = false});
            }
            return Json(new { res = true, model.producto });
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