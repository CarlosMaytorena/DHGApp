﻿using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.Services.Admin;
using AgricolaDH_GApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
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
        private BlobStorageService blobStorageService;


        public EgresosController(
			ILogger<EgresosController> logger, 
			AppDbContext _ctx, 
			EgresoService _egresoService,
			ViewRenderService _renderService,
            ProductoService _productoService,
			UsuarioService _usuarioService,
			AlmacenService _almacenService,
            BlobStorageService __blobStorageService)
		{
			_logger = logger;
			context = _ctx;
			egresoService = _egresoService;
			productoService = _productoService;
			usuarioService = _usuarioService;
            almacenService = _almacenService;
			renderService = _renderService;
            blobStorageService = __blobStorageService;

        }

        [HttpGet]
		public IActionResult Index()
		{
			model.egresosLista = egresoService.SelectEgresos();
			return PartialView("~/Views/Egresos/Index.cshtml", model);
		}
		[HttpGet]
        public IActionResult EgresoForm()
        {
			model.productosList = productoService.SelectProductos();
			model.usuariosList = usuarioService.SelectUsuarios();
            return PartialView("~/Views/Egresos/EgresoForm.cshtml", model);
        }
		[HttpPost]
        public async Task<IActionResult> GenerarEgreso(EgresosVM model)
        {
            int res = 1;
            try
            {
                context.Evidencia.Add(new Evidencia());
                context.SaveChanges();
                model.egreso.IdEvidencia = context.Evidencia.OrderByDescending(e => e.IdEvidencia).FirstOrDefault().IdEvidencia;

                model.egreso.PathAntes = egresoService.UploadFile(model, "Antes");
                model.egreso.PathDespues = egresoService.UploadFile(model, "Despues");
                egresoService.Generar(model);
            }
			catch
			{
                res = -1;
            }
            return base.Json(new { res, url = await renderService.RenderViewToStringAsync("~/Views/Egresos/Index.cshtml", model) });

        }

        [HttpPost]
        public IActionResult Detalles([FromBody] int IdEgreso)
        {
            model.egreso = egresoService.SelectEgreso(IdEgreso);
			model.egreso.Producto = productoService.SelectProducto(model.egreso.IdProducto).NombreProducto;
            model.egreso.Solicitante = context.Usuarios.Find(model.egreso.IdSolicitante).Username;
            model.egreso.PathAntes = context.Evidencia.Find(model.egreso.IdEvidencia).PathAntes;
            model.egreso.PathDespues = context.Evidencia.Find(model.egreso.IdEvidencia).PathDespues;
            model.egreso.Fecha.ToString();
            return PartialView("~/Views/Egresos/Detalles.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> DescargarArchivo([FromBody] string filename)
        {
			try
			{
				if (filename.IsNullOrEmpty())
					throw new Exception();
				blobStorageService.DownloadFileAsync(filename);
				int res = 1;
                return Json(new { res, url = await renderService.RenderViewToStringAsync("~/Views/Egresos/Index.cshtml", model) });
            }
            catch
			{
                int res = -1;
                return Json(new { res, url = await renderService.RenderViewToStringAsync("~/Views/Egresos/Index.cshtml", model) });
            }
        }

        [HttpPost]
        public IActionResult AgregarProductoLista(EgresosVM model)
        {
            // TODO: Primer escaneo no se agrega a la lista
            if (model.almacen == null)
                return PartialView("~/Views/Egresos/ListaProductos.cshtml", model);

            bool cond1 = context.Almacen.Any(x => x.SerialNumber.Equals(model.almacen.SerialNumber));
            bool cond2 = !model.almacenLista.Exists(x => x.SerialNumber.Equals(model.almacen.SerialNumber));
            if (cond1 && cond2)
            {
                Almacen a = context.Almacen.FirstOrDefault(x => x.SerialNumber.Equals(model.almacen.SerialNumber));
                a.NombreProducto = context.Productos.FirstOrDefault(x => x.IdProducto.Equals(a.IdProducto)).NombreProducto;
                a.Descripcion = context.Productos.FirstOrDefault(x => x.IdProducto.Equals(a.IdProducto)).Descripcion;
                Estatus estatus = context.Estatus.Single(x => x.IdEstatus.Equals(a.IdEstatus));
                a.Estatus = estatus.NombreEstatus;

                model.almacenLista.Add(a);
            }
            return PartialView("~/Views/Egresos/ListaProductos.cshtml", model);
        }

        [HttpPost]
        public IActionResult EliminarProductoLista(EgresosVM model)
        {
            if (model.almacenLista.Count > 0)
                model.almacenLista.RemoveAt(model.almacenLista.Count - 1);
            return PartialView("~/Views/Egresos/ListaProductos.cshtml", model);
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