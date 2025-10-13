using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.Services.Admin;
using AgricolaDH_GApp.ViewModels;
using Antlr.Runtime.Tree;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Diagnostics;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace AgricolaDH_GApp.Controllers
{
	public class AlmacenController : Controller
	{
        private AlmacenVM model = new AlmacenVM();
        private readonly ILogger<AlmacenController> _logger;

        private readonly AppDbContext context;
		private AlmacenService almacenService;
        private ViewRenderService renderService;
        private UsuarioService usuarioService;
        private ProductoService productoService;
        public AlmacenController(
            ILogger<AlmacenController> logger,
            AppDbContext _ctx,
            ViewRenderService _renderService,
            AlmacenService _almacenService,
            UsuarioService _usuarioService,
            ProductoService _productoService
            )
		{
			_logger = logger;
			almacenService = _almacenService;
            context = _ctx;
            renderService = _renderService;
            usuarioService = _usuarioService;
            productoService = _productoService;
        }

		[HttpGet]
		public IActionResult Index()
        {
            model.almacenLista = almacenService.SelectAlmacen();
            return PartialView("~/Views/Almacen/Index.cshtml", model);
		}
        [HttpGet]
        public IActionResult Entrada()
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            int? idRol = HttpContext.Session.GetInt32("IdRol");

            model.usuariosList = usuarioService.SelectUsuarios();
            model.ingenieroList = usuarioService.SelectUsuariosByIdRol(RolEnumerators.Ingeniero);

            model.almacen.IdAlmacenista = idUsuario != null ? (int)idUsuario : 0;
            model.almacen.esAutorizado = idRol == RolEnumerators.Administrador ? true : false;

            return PartialView("~/Views/Almacen/Entrada.cshtml", model);
        }

        [HttpGet]
        public IActionResult Salida()
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            int? idRol = HttpContext.Session.GetInt32("IdRol");

            model.usuariosList = usuarioService.SelectUsuarios();
            model.ingenieroList = usuarioService.SelectUsuariosByIdRol(RolEnumerators.Ingeniero);

            model.almacen.IdAlmacenista = idUsuario != null ? (int)idUsuario : 0;
            model.almacen.esAutorizado = idRol == RolEnumerators.Administrador ? true : false;

            return PartialView("~/Views/Almacen/Salida.cshtml", model);
        }
        [HttpPost]
        public async Task<IActionResult> AltaAlmacen(AlmacenVM model)
        {
            int res = 1;
            try
            {
                almacenService.Entrada(model);
            }
            catch
            {
                res = -1;
            }
            model.almacenLista = almacenService.SelectAlmacen();
            return Json( new {res, url = await renderService.RenderViewToStringAsync("~/Views/Almacen/Index.cshtml", model) });
        }

        [HttpPost]
        public async Task<IActionResult> BajaAlmacen(AlmacenVM model)
        {
            int res = 1;
            try
            {
                almacenService.Salida(model);
            }
            catch
            {
                res = -1;
            }
            model.almacenLista = almacenService.SelectAlmacen();
            return Json(new { res, url = await renderService.RenderViewToStringAsync("~/Views/Almacen/Index.cshtml", model) });
        }

        [HttpPost]
        public IActionResult AgregarProductoLista(AlmacenVM model)
        {
            bool cond1 = context.Almacen.Any(x => x.SerialNumber.Equals(model.almacen.SerialNumber)); //duplicados Almacen
            bool cond2 = !model.almacenLista.Exists(x => x.SerialNumber.Equals(model.almacen.SerialNumber)); //duplicados en ListaProductos
            if (cond1 && cond2)
            {
                Almacen a = context.Almacen.FirstOrDefault(x => x.SerialNumber.Equals(model.almacen.SerialNumber));
                a.NombreProducto = context.Productos.FirstOrDefault(x => x.IdProducto.Equals(a.IdProducto)).NombreProducto;
                a.Descripcion = context.Productos.FirstOrDefault(x => x.IdProducto.Equals(a.IdProducto)).Descripcion;
                Estatus estatus = context.Estatus.Single(x => x.IdEstatus.Equals(a.IdEstatus));
                a.Estatus = estatus.NombreEstatus;

                model.almacenLista.Add(a);
            }
            return PartialView("~/Views/Almacen/ListaProductos.cshtml", model);
        }

        [HttpPost]
        public IActionResult EliminarProductoLista(AlmacenVM model)
        {
            if (model.almacenLista.Count > 0)
                model.almacenLista.RemoveAt(model.almacenLista.Count-1);
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