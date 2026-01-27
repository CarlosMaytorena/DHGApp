using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.Services;
using AgricolaDH_GApp.Services.Admin;
using AgricolaDH_GApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Security.Policy;

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
        private LogsEgresosService logsEgresosService;
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
            LogsEgresosService _logsEgresosService,
            BlobStorageService __blobStorageService)
		{
			_logger = logger;
			context = _ctx;
			egresoService = _egresoService;
			productoService = _productoService;
			usuarioService = _usuarioService;
            almacenService = _almacenService;
            logsEgresosService = _logsEgresosService;
			renderService = _renderService;
            blobStorageService = __blobStorageService;

        }

        private void InicializarSesionUsuario(EgresosVM model)
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            int? idRol = HttpContext.Session.GetInt32("IdRol");

            model.productosList = productoService.SelectProductos();
            model.usuariosList = usuarioService.SelectUsuarios();
            model.ingenieroList = usuarioService.SelectUsuariosByIdRol(RolEnumerators.Ingeniero);

            model.egreso.IdSolicitante = idUsuario != null ? (int)idUsuario : 0;
            model.egreso.esAutorizado = idRol == RolEnumerators.Administrador ? true : false;
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
            InicializarSesionUsuario(model);
            return PartialView("~/Views/Egresos/EgresoForm.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> GenerarEgreso(EgresosVM model)
        {
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                Evidencia e = new Evidencia();
                context.Evidencia.Add(e);
                await context.SaveChangesAsync();

                // Insertar log
                LogsEgresos log = new LogsEgresos
                {
                    IdSolicitante = model.egreso.IdSolicitante,
                    Fecha = DateTime.Now
                };
                int idLogsEgresos = logsEgresosService.InsertarLog(log);

                // Subir archivos a BlobStorage
                model.egreso.IdEvidencia = e.IdEvidencia;
                model.egreso.PathAntes = egresoService.UploadFile(model, "Antes");
                model.egreso.PathDespues = egresoService.UploadFile(model, "Despues");

                egresoService.Generar(model, idLogsEgresos);

                await transaction.CommitAsync();
                return Ok(new { message = "Egreso generado correctamente."});
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(new { message = "Error al generar egreso", error = ex.Message });
            }
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

        [HttpGet]
        public async Task<IActionResult> DescargarArchivo([FromQuery] string filename)
        {
            int res = 1;
            try
            {
				if (filename.IsNullOrEmpty())
					throw new Exception();
                var download = await blobStorageService.DownloadFileAsync(filename);
                return File(download.Content, download.ContentType ?? "application/octet-stream", filename);
            }
            catch
			{
                res = -1;
            }
            return Json(new { res });
        }

        [HttpPost]
        public IActionResult AgregarProductoLista(EgresosVM model)
        {
            InicializarSesionUsuario(model);

            if (model.almacen == null)
                return PartialView("~/Views/Egresos/ListaProductos.cshtml", model);

            bool existeAlmacen = context.Almacen.Any(x => x.SerialNumber.Equals(model.almacen.SerialNumber));
            bool dupliLista = !model.almacenLista.Exists(x => x.SerialNumber.Equals(model.almacen.SerialNumber));
            bool existeEgreso = context.Egresos.Any(x => x.SerialNumber.Equals(model.almacen.SerialNumber));

            Almacen row = context.Almacen.SingleOrDefault(x => x.SerialNumber == model.almacen.SerialNumber);
            bool usado = context.Almacen.Any(x => x.SerialNumber == row.SerialNumber && x.Uso == true);
            if (existeAlmacen && dupliLista && !existeEgreso && usado)
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
            InicializarSesionUsuario(model);

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