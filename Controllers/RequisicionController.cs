using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.Services.Admin;
using AgricolaDH_GApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AgricolaDH_GApp.Controllers
{
    public class RequisicionController : Controller
	{
		private readonly ILogger<RequisicionController> _logger;
		
		private readonly AppDbContext context;
        private UsuarioService usuarioService;
		private ProveedorService proveedorService;
		private AreaService areaService;
		private CultivoService cultivoService;
		private RanchoService ranchoService;
		private EtapaService etapaService;
		private TemporadaService temporadaService;
		private ProductoService productoService;

        public RequisicionController(ILogger<RequisicionController> logger, AppDbContext _ctx, UsuarioService _usuarioService, ProveedorService _proveedorService, AreaService _areaService, CultivoService _cultivoService, RanchoService _ranchoService, EtapaService _etapaService, TemporadaService _temporadaService, ProductoService _productoService)
		{
			_logger = logger;
			context = _ctx;
			usuarioService = _usuarioService;
			proveedorService = _proveedorService;
			areaService = _areaService;
			cultivoService = _cultivoService;
			ranchoService = _ranchoService;
			etapaService = _etapaService;
			temporadaService = _temporadaService;
			productoService = _productoService;
		}

		[HttpGet]
		public IActionResult Index()
		{
			
			return PartialView("~/Views/Requisicion/Index.cshtml");
		}

        [HttpPost]
        public IActionResult CrearRequisicion()
        {
            RequisicionesVM model = new RequisicionesVM();

			RolEnumerators rolEnumerators = new RolEnumerators();

			model.requisicion.Fecha = DateTime.Now;

			model.productosOrdenar = new List<ProductoOrdenar>() { 
				new ProductoOrdenar()
			};

			model.solicitanteList = usuarioService.SelectUsuariosByIdRol(rolEnumerators.Ingeniero);
			model.proveedorList = proveedorService.SelectProveedores();
			model.areaList = areaService.SelectAreas();
			model.cultivoList = cultivoService.SelectCultivos();
			model.ranchoList = ranchoService.SelectRanchos();
			model.etapaList = etapaService.SelectEtapas();
			model.temporadaList = temporadaService.SelectTemporadas();
			model.productoList = productoService.SelectProductos();

            return PartialView("~/Views/Requisicion/RequisicionForm.cshtml", model);
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