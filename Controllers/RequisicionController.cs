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
        private ViewRenderService renderService;
        private UsuarioService usuarioService;
		private ProveedorService proveedorService;
		private AreaService areaService;
		private CultivoService cultivoService;
		private RanchoService ranchoService;
		private EtapaService etapaService;
		private TemporadaService temporadaService;
		private ProductoService productoService;
		private OrdenDeCompraService requisicionService;

        public RequisicionController(ILogger<RequisicionController> logger, AppDbContext _ctx, ViewRenderService _renderService, UsuarioService _usuarioService, ProveedorService _proveedorService, AreaService _areaService, CultivoService _cultivoService, RanchoService _ranchoService, EtapaService _etapaService, TemporadaService _temporadaService, ProductoService _productoService, OrdenDeCompraService _requisicionService)
		{
			_logger = logger;
			context = _ctx;
			renderService = _renderService;
			usuarioService = _usuarioService;
			proveedorService = _proveedorService;
			areaService = _areaService;
			cultivoService = _cultivoService;
			ranchoService = _ranchoService;
			etapaService = _etapaService;
			temporadaService = _temporadaService;
			productoService = _productoService;
			requisicionService = _requisicionService;
		}

		[HttpGet]
		public IActionResult Index()
		{
			RequisicionesVM model = new RequisicionesVM();
			model.requisicionList = requisicionService.SelectOrdenDeCompraTable(1);

			return PartialView("~/Views/Requisicion/Index.cshtml", model);
		}

        [HttpPost]
        public IActionResult CrearRequisicion()
        {
            RequisicionesVM model = new RequisicionesVM();

			RolEnumerators rolEnumerators = new RolEnumerators();

			model.requisicion.FechaRequisicion = DateTime.Now;

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

        [HttpPost]
        public IActionResult EditarRequisicion(int IdOrdenDeCompra)
        {
            RequisicionesVM model = new RequisicionesVM();

			model.requisicion = requisicionService.SelectRequisicion(IdOrdenDeCompra);
            model.productosOrdenar = requisicionService.SelectProductosOrdenar(IdOrdenDeCompra);

            RolEnumerators rolEnumerators = new RolEnumerators();

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

        [HttpPost]
        public IActionResult AgregarProductoOrdenar(RequisicionesVM model)
        {
            model.productosOrdenar.Add(new ProductoOrdenar());
            model.productoList = productoService.SelectProductos();

            return PartialView("~/Views/Requisicion/ProductosOrdenar.cshtml", model);
        }

        [HttpPost]
        public IActionResult EliminarProductoOrdenar(RequisicionesVM model)
        {
			model.productosOrdenar.RemoveAt(model.productosOrdenar.Count - 1);
            model.productoList = productoService.SelectProductos();

            return PartialView("~/Views/Requisicion/ProductosOrdenar.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> InsertRequisicion(RequisicionesVM model)
        {
			model.requisicion.IdOrdenDeCompraStatus = 1; //Status Enviado
			int IdOrdenDeCompra = requisicionService.InsertRequisicion(model.requisicion);
			int res = 0;

			if (IdOrdenDeCompra > 0) 
			{
                foreach (var productoOrdenar in model.productosOrdenar)
                {
                    productoOrdenar.IdOrdenDeCompra = IdOrdenDeCompra;
                    res = requisicionService.InsertProductoOrdenar(productoOrdenar);
                }
            }
			else
			{
				res = -1;
			}

            model = new RequisicionesVM();
            model.requisicionList = requisicionService.SelectRequisiciones();

            return Json(new { res, url = await renderService.RenderViewToStringAsync("~/Views/Requisicion/Index.cshtml", model) });

        }

        [HttpPost]
        public async Task<IActionResult> AcceptRejectRequisicion(RequisicionesVM model, int IdOrdenDeCompraStatus)
        {
            model.requisicion.FechaOrdenDeCompra = DateTime.Now;
            model.requisicion.IdOrdenDeCompraStatus = IdOrdenDeCompraStatus; //Status Change
            int res = 0;
            res = requisicionService.UpdateRequisicion(model.requisicion);

            if (res == 0)
            {
                foreach (var productoOrdenar in model.productosOrdenar)
                {
                    res = requisicionService.UpdateProductoOrdenar(productoOrdenar);
                }
            }
            else
            {
                res = -1;
            }

            model = new RequisicionesVM();
            model.requisicionList = requisicionService.SelectRequisiciones();

            return Json(new { res, url = await renderService.RenderViewToStringAsync("~/Views/Requisicion/Index.cshtml", model) });

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