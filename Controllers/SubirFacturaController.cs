using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.Services.Admin;
using AgricolaDH_GApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AgricolaDH_GApp.Controllers
{
    public class SubirFacturaController : Controller
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
		private OrdenDeCompraService ordenDeCompraService;
        private OrdenDeCompraStatusEnumerators OrdenDeCompraEnumerator = new OrdenDeCompraStatusEnumerators();

        public SubirFacturaController(ILogger<RequisicionController> logger, AppDbContext _ctx, ViewRenderService _renderService, UsuarioService _usuarioService, ProveedorService _proveedorService, AreaService _areaService, CultivoService _cultivoService, RanchoService _ranchoService, EtapaService _etapaService, TemporadaService _temporadaService, ProductoService _productoService, OrdenDeCompraService _ordenDeCompraService)
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
            ordenDeCompraService = _ordenDeCompraService;
		}

		[HttpGet]
		public IActionResult Index()
		{
			SubirFacturaVM model = new SubirFacturaVM();
			model.subirFacturaList = ordenDeCompraService.SelectOrdenDeCompraTable(OrdenDeCompraEnumerator.Aceptado);

			return PartialView("~/Views/SubirFactura/Index.cshtml", model);
		}

        [HttpPost]
        public IActionResult SubirFactura(int IdOrdenDeCompra)
        {
            SubirFacturaVM model = new SubirFacturaVM();

			model.ordenDeCompra = ordenDeCompraService.SelectOrdenDeCompra(IdOrdenDeCompra);
            model.productosOrdenar = ordenDeCompraService.SelectProductosOrdenarSelected(IdOrdenDeCompra);

            return PartialView("~/Views/SubirFactura/SubirFacturaForm.cshtml", model);
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
			int IdOrdenDeCompra = ordenDeCompraService.InsertRequisicion(model.requisicion);
			int res = 0;

			if (IdOrdenDeCompra > 0) 
			{
                foreach (var productoOrdenar in model.productosOrdenar)
                {
                    productoOrdenar.IdOrdenDeCompra = IdOrdenDeCompra;
                    res = ordenDeCompraService.InsertProductoOrdenar(productoOrdenar);
                }
            }
			else
			{
				res = -1;
			}

            model = new RequisicionesVM();
            model.requisicionList = ordenDeCompraService.SelectRequisiciones();

            return Json(new { res, url = await renderService.RenderViewToStringAsync("~/Views/Requisicion/Index.cshtml", model) });

        }

        [HttpPost]
        public async Task<IActionResult> AcceptSubirFactura(RequisicionesVM model)
        {
            model.requisicion.IdOrdenDeCompraStatus = OrdenDeCompraEnumerator.PorIngresar; //Status Change
            int res = 0;
            res = ordenDeCompraService.UpdateOrdenDeCompra(model.requisicion);

            if (res == 0)
            {
                foreach (var productoOrdenar in model.productosOrdenar)
                {
                    res = ordenDeCompraService.UpdateProductoOrdenar(productoOrdenar);
                }
            }
            else
            {
                res = -1;
            }

            model = new RequisicionesVM();
            model.requisicionList = ordenDeCompraService.SelectRequisiciones();

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