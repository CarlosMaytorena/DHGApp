using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.Services.Admin;
using AgricolaDH_GApp.ViewModels;
using Antlr.Runtime.Tree;
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
            //model.productoList = almacenService.SelectProductos();
            model.usuariosList = usuarioService.SelectUsuarios();
            return PartialView("~/Views/Almacen/Entrada.cshtml", model);
        }
		[HttpGet]
        public IActionResult Salida()
        {
            //model.productoList = almacenService.SelectProductos();
            model.usuariosList = usuarioService.SelectUsuarios();
            return PartialView("~/Views/Almacen/Salida.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> AltaAlmacen(AlmacenVM model)
        {
            model.producto = almacenService.ProductTypeByPN(model.producto.PN);
            //registro.Almacen.IdProducto = model.producto.IdProducto;
            //registro.Movimiento.IdProducto = model.producto.IdProducto;
            //---------------------------------------------------------------------------

            int res = 0; //almacenService.Entrada(registro);
            int resp_mov = 0; // movimientoService.Entrada(registro);
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
            //------------------------------ Arreglo temporal ---------------------------
            model.producto = almacenService.ProductTypeByPN(registro.Producto.PN);
            registro.Almacen.IdProducto = model.producto.IdProducto;
            registro.Movimiento.IdProducto = model.producto.IdProducto;
            //---------------------------------------------------------------------------

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
            //model.producto = almacenService.ProductBySN(registro.PN);
            if (model.producto == null)
            {
                return Json(new { res = false});
            }
            return  Json(new {res = true, model.producto});
        }

        [HttpPost]
        public IActionResult AgregarProductoLista(AlmacenVM model)
        {
            if (context.Almacen.Any(x => x.SerialNumber.Equals(model.almacen.SerialNumber)))
            {
                Almacen a = context.Almacen.FirstOrDefault(x => x.SerialNumber.Equals(model.almacen.SerialNumber));
                a.NombreProducto = context.Productos.FirstOrDefault(x => x.IdProducto.Equals(a.IdProducto)).NombreProducto;
                a.Descripcion = context.Productos.FirstOrDefault(x => x.IdProducto.Equals(a.IdProducto)).Descripcion;
                model.almacenLista.Add(a);
            }

            return PartialView("~/Views/Almacen/ListaProductos.cshtml", model);
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