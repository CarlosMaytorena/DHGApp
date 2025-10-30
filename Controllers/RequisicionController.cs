using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Helper;
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
        private ConstanteService constanteService;
        private Email email;

        public RequisicionController(ILogger<RequisicionController> logger, AppDbContext _ctx, ViewRenderService _renderService, UsuarioService _usuarioService, ProveedorService _proveedorService, AreaService _areaService, CultivoService _cultivoService, RanchoService _ranchoService, EtapaService _etapaService, TemporadaService _temporadaService, ProductoService _productoService, OrdenDeCompraService _requisicionService, ConstanteService _constanteService, Email _email)
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
            constanteService = _constanteService;
            email = _email;
        }

		[HttpGet]
		public IActionResult Index()
		{
            int idUsuario = Convert.ToInt32(HttpContext.Session.GetInt32("IdUsuario"));
            RequisicionesVM model = new RequisicionesVM();

            model.requisicionList = requisicionService.SelectOrdenDeCompraTableList(OrdenDeCompraStatusEnumerators.Enviado, idUsuario);
            model.requisicionAceptadaList = requisicionService.SelectOrdenDeCompraTableList(OrdenDeCompraStatusEnumerators.Aceptado, idUsuario, 2);
            model.requisicionRechazadaList = requisicionService.SelectOrdenDeCompraTableList(OrdenDeCompraStatusEnumerators.Rechazado, idUsuario, 2);

            return PartialView("~/Views/Requisicion/Index.cshtml", model);
		}

        [HttpPost]
        public IActionResult CrearRequisicion()
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            int? idRol = HttpContext.Session.GetInt32("IdRol");

            RequisicionesVM model = new RequisicionesVM();

			model.requisicion.FechaRequisicion = DateTime.Now;

            model.productosOrdenar = new List<ProductoOrdenar>() {
                new ProductoOrdenar()
            };

            model.solicitanteList = usuarioService.SelectUsuariosByIdRol(RolEnumerators.Administrador);
			model.solicitanteList.AddRange(usuarioService.SelectUsuariosByIdRol(RolEnumerators.Ingeniero));
			model.proveedorList = proveedorService.SelectProveedores();
			model.areaList = areaService.SelectAreas();
			model.cultivoList = cultivoService.SelectCultivos();
			//model.ranchoList = ranchoService.SelectRanchos();
			model.etapaList = etapaService.SelectEtapas();
			model.temporadaList = temporadaService.SelectTemporadas();
			//model.productoList = productoService.SelectProductos();

            model.requisicion.IdSolicitante = idUsuario != null ? (int)idUsuario : 0;

            if (idRol == RolEnumerators.Ingeniero)
            {
                model.esAutorizado = false;
            }

            return PartialView("~/Views/Requisicion/RequisicionForm.cshtml", model);
        }

        [HttpPost]
        public IActionResult EditarRequisicion(int IdOrdenDeCompra)
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            int? idRol = HttpContext.Session.GetInt32("IdRol");

            RequisicionesVM model = new RequisicionesVM();

			model.requisicion = requisicionService.SelectRequisicion(IdOrdenDeCompra);
            model.productosOrdenar = requisicionService.SelectProductosOrdenar(IdOrdenDeCompra);

            model.solicitanteList = usuarioService.SelectUsuariosByIdRol(RolEnumerators.Administrador);
            model.solicitanteList.AddRange(usuarioService.SelectUsuariosByIdRol(RolEnumerators.Ingeniero));
            model.proveedorList = proveedorService.SelectProveedores();
            model.areaList = areaService.SelectAreas();
            model.cultivoList = cultivoService.SelectCultivos();
            model.ranchoList = ranchoService.SelectRanchos();
            model.etapaList = etapaService.SelectEtapas();
            model.temporadaList = temporadaService.SelectTemporadas();
            model.productoList = productoService.SelectProductos().Where(p => p.IdProveedor == model.requisicion.IdProveedor).ToList();

            if(idRol == RolEnumerators.Ingeniero)
            {
                model.esAutorizado = false;
            }

            return PartialView("~/Views/Requisicion/RequisicionForm.cshtml", model);
        }

        [HttpPost]
        public IActionResult AgregarProductoOrdenar(RequisicionesVM model)
        {
            model.productosOrdenar.Add(new ProductoOrdenar());
            model.productoList = productoService.SelectProductos().Where(p => p.IdProveedor == model.requisicion.IdProveedor).ToList();

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
            int idUsuario = Convert.ToInt32(HttpContext.Session.GetInt32("IdUsuario"));

            model.requisicion.IdOrdenDeCompraStatus = 1; //Status Enviado
			int IdOrdenDeCompra = requisicionService.InsertRequisicion(model.requisicion);
			int res = 0;

			if (IdOrdenDeCompra > 0) 
			{
                foreach (var productoOrdenar in model.productosOrdenar)
                {
                    productoOrdenar.IdOrdenDeCompra = IdOrdenDeCompra;
                    productoOrdenar.PorRecibir = productoOrdenar.Cantidad;
                    res = requisicionService.InsertProductoOrdenar(productoOrdenar);
                }
            }
			else
			{
				res = -1;
			}

            model = new RequisicionesVM();
            model.requisicionList = requisicionService.SelectOrdenDeCompraTableList(OrdenDeCompraStatusEnumerators.Enviado, idUsuario);
            model.requisicionAceptadaList = requisicionService.SelectOrdenDeCompraTableList(OrdenDeCompraStatusEnumerators.Aceptado, idUsuario, 2);
            model.requisicionRechazadaList = requisicionService.SelectOrdenDeCompraTableList(OrdenDeCompraStatusEnumerators.Rechazado, idUsuario, 2);

            return Json(new { res, url = await renderService.RenderViewToStringAsync("~/Views/Requisicion/Index.cshtml", model) });

        }

        [HttpPost]
        public async Task<IActionResult> AcceptRejectRequisicion(RequisicionesVM model, int IdOrdenDeCompraStatus)
        {
            int idUsuario = Convert.ToInt32(HttpContext.Session.GetInt32("IdUsuario"));

            int res = 0;
            foreach (var productoOrdenar in model.productosOrdenar)
            {
                ProductoOrdenar productoOrdenarUpdate = requisicionService.SelectProductoOrdenar(productoOrdenar.IdProductoOrdenar);
                if (productoOrdenarUpdate != null)
                {
                    productoOrdenarUpdate.IdProducto = productoOrdenar.IdProducto;
                    productoOrdenarUpdate.Cantidad = productoOrdenar.Cantidad;
                    res = requisicionService.UpdateProductoOrdenar(productoOrdenarUpdate);
                }
                else
                {
                    productoOrdenar.IdOrdenDeCompra = model.requisicion.IdOrdenDeCompra;
                    res = requisicionService.InsertProductoOrdenar(productoOrdenar);
                }
            }

            if (res == 0)
            {
                model.requisicion.FechaOrdenDeCompra = DateTime.Now;
                model.requisicion.IdOrdenDeCompraStatus = IdOrdenDeCompraStatus; //Status Change
                res = requisicionService.UpdateOrdenDeCompra(model.requisicion);

                OrdenDeCompraTable ordenDeCompra = requisicionService.SelectOrdenDeCompra(model.requisicion.IdOrdenDeCompra);
                List<ProductoOrdenarSelected> productosOrdenar = requisicionService.SelectProductosOrdenarSelected(model.requisicion.IdOrdenDeCompra);

                //Proveedor proveedor = proveedorService.SelectProveedor(model.requisicion.IdProveedor);
                var solicitante = usuarioService.SelectUsuario(model.requisicion.IdSolicitante);

                if(res == 0)
                {
                    string sendgridKey = constanteService.SelectConstante("SendgridKey").Valor;
                    string defaultEmailFrom = constanteService.SelectConstante("DefaultEmailFrom").Valor;
                    email.SendMail(sendgridKey, defaultEmailFrom, solicitante.Correo, solicitante.Nombre, "Requisicion #" + ordenDeCompra.IdOrdenDeCompra, ordenDeCompra.FechaRequisicion.ToShortDateString(), ordenDeCompra.Cultivo, ordenDeCompra.Rancho, ordenDeCompra.Etapa, ordenDeCompra.Temporada, productosOrdenar);
                }
            }
            else
            {
                res = -1;
            }

            model = new RequisicionesVM();
            model.requisicionList = requisicionService.SelectOrdenDeCompraTableList(OrdenDeCompraStatusEnumerators.Enviado, idUsuario);
            model.requisicionAceptadaList = requisicionService.SelectOrdenDeCompraTableList(OrdenDeCompraStatusEnumerators.Aceptado, idUsuario, 2);
            model.requisicionRechazadaList = requisicionService.SelectOrdenDeCompraTableList(OrdenDeCompraStatusEnumerators.Rechazado, idUsuario, 2);

            return Json(new { res, url = await renderService.RenderViewToStringAsync("~/Views/Requisicion/Index.cshtml", model) });

        }

        [HttpGet]
        public IActionResult GetRanchoList(int IdArea)
        {
            var ranchoList = ranchoService.SelectRanchos().Where(r => r.IdArea == IdArea).ToList();

            return Json(new { ranchoList });
        }

        [HttpGet]
        public IActionResult GetProductoList(int IdProveedor)
        {
            RequisicionesVM model = new RequisicionesVM();

            model.productosOrdenar = new List<ProductoOrdenar>() {
                new ProductoOrdenar()
            };

            model.productoList = productoService.SelectProductos().Where(p => p.IdProveedor == IdProveedor).ToList();

            return PartialView("~/Views/Requisicion/ProductosOrdenar.cshtml", model);
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