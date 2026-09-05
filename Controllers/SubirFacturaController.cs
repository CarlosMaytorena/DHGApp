using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.Services.Admin;
using AgricolaDH_GApp.ViewModels;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace AgricolaDH_GApp.Controllers
{
    public class SubirFacturaController : Controller
	{
		private readonly ILogger<RequisicionController> _logger;

		private readonly AppDbContext context;
        private ViewRenderService renderService;
		private OrdenDeCompraService ordenDeCompraService;

        public SubirFacturaController(ILogger<RequisicionController> logger, AppDbContext _ctx, ViewRenderService _renderService, OrdenDeCompraService _ordenDeCompraService)
		{
			_logger = logger;
			context = _ctx;
            ordenDeCompraService = _ordenDeCompraService;
			renderService = _renderService;
		}

		[HttpGet]
		public IActionResult Index()
		{
            int idUsuario = Convert.ToInt32(HttpContext.Session.GetInt32("IdUsuario"));

            SubirFacturaVM model = new SubirFacturaVM();
			model.subirFacturaList = ordenDeCompraService.SelectOrdenDeCompraTableEnProceso(idUsuario);

			return PartialView("~/Views/SubirFactura/Index.cshtml", model);
		}

        [HttpPost]
        public IActionResult SubirFactura(int IdOrdenDeCompra)
        {
            SubirFacturaVM model = CargarDetalleOrden(IdOrdenDeCompra);

            return PartialView("~/Views/SubirFactura/SubirFacturaForm.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarFactura(int IdOrdenDeCompra, bool? MonedaNacional, List<FacturaDetalleVM> facturaDetallePreview)
        {
            int idUsuario = Convert.ToInt32(HttpContext.Session.GetInt32("IdUsuario"));

            OrdenDeCompraTable ordenActual = ordenDeCompraService.SelectOrdenDeCompra(IdOrdenDeCompra);
            int res;

            if (ordenActual == null || ordenActual.IdOrdenDeCompraStatus == OrdenDeCompraStatusEnumerators.Cerrado)
            {
                res = -1;
            }
            else if (facturaDetallePreview == null || facturaDetallePreview.Count == 0)
            {
                res = -1;
            }
            else
            {
                Factura factura = new Factura()
                {
                    IdOrdenDeCompra = IdOrdenDeCompra,
                    MonedaNacional = MonedaNacional,
                    FechaCarga = DateTime.Now,
                    IdUsuarioCarga = idUsuario
                };

                List<FacturaDetalle> detalles = facturaDetallePreview.Select(d => new FacturaDetalle()
                {
                    IdProductoOrdenar = d.EsExtra ? null : d.IdProductoOrdenar,
                    DescripcionExtra = d.EsExtra ? d.Producto : null,
                    CantidadFacturada = d.CantidadFacturada,
                    PrecioUnitario = d.PrecioUnitario,
                    PrecioTotal = d.PrecioTotal
                }).ToList();

                int idFactura = ordenDeCompraService.InsertFactura(factura, detalles);
                res = idFactura > 0 ? 0 : -1;
            }

            SubirFacturaVM model = CargarDetalleOrden(IdOrdenDeCompra);

            return Json(new { res, url = await renderService.RenderViewToStringAsync("~/Views/SubirFactura/SubirFacturaForm.cshtml", model) });
        }

        [HttpPost]
        public async Task<ActionResult> UploadFileAsync(int IdOrdenDeCompra, IFormFile file)
        {
            int res = 0;

            SubirFacturaVM model = CargarDetalleOrden(IdOrdenDeCompra);

            try
            {

                // Validación básica
                if (file == null || file.Length == 0)
                {
                    return Json(new { res = -1, msg = "Archivo vacío o no recibido." });
                }

                XmlDocument doc = new XmlDocument();

                // ✅ Leer directo del stream (sin usar disco)
                using (var stream = file.OpenReadStream())
                {
                    doc.Load(stream);
                }

                var moneda = doc
                    .GetElementsByTagName("cfdi:Comprobante")
                    .Cast<XmlElement>()
                    .FirstOrDefault()?.GetAttribute("Moneda");

                model.monedaNacionalPreview = moneda == "MXN";

                List<FacturaDetalleVM> preview = new List<FacturaDetalleVM>();

                foreach (XmlElement item in doc.GetElementsByTagName("cfdi:Concepto"))
                {
                    string descripcionOriginal = item.GetAttribute("Descripcion");
                    string descripcion = Regex.Replace(ReplaceDiacritics(descripcionOriginal.ToLower()), @"[^a-zA-Z0-9]", "");
                    string noIdentificacion = item.GetAttribute("NoIdentificacion");

                    ProductoOrdenarSelected productoEncontrado = null;
                    foreach (var productoOrdenar in model.productosOrdenar)
                    {
                        var productoOrdenarName = Regex.Replace(ReplaceDiacritics(productoOrdenar.Producto), @"[^a-zA-Z0-9]", "").ToLower();

                        bool matchDescripcion = !string.IsNullOrEmpty(descripcion) && descripcion == productoOrdenarName;
                        bool matchClaveProveedor = !string.IsNullOrEmpty(noIdentificacion) && int.TryParse(noIdentificacion, out int claveProveedor) && claveProveedor == productoOrdenar.ClaveProveedor;

                        if (matchDescripcion || matchClaveProveedor)
                        {
                            productoEncontrado = productoOrdenar;
                            break;
                        }
                    }

                    decimal cantidadFactura = Convert.ToDecimal(item.GetAttribute("Cantidad"));
                    decimal importe = Convert.ToDecimal(item.GetAttribute("Importe"));

                    if (productoEncontrado != null)
                    {
                        int cantidad = (!productoEncontrado.CalculoAlterno) ? Convert.ToInt32(cantidadFactura) : Convert.ToInt32(cantidadFactura / productoEncontrado.Contenido);

                        preview.Add(new FacturaDetalleVM()
                        {
                            IdProductoOrdenar = productoEncontrado.IdProductoOrdenar,
                            Producto = productoEncontrado.NombreDropdown,
                            EsExtra = false,
                            CantidadFacturada = cantidad,
                            PrecioUnitario = cantidad != 0 ? importe / cantidad : 0,
                            PrecioTotal = importe
                        });
                    }
                    else
                    {
                        preview.Add(new FacturaDetalleVM()
                        {
                            IdProductoOrdenar = null,
                            Producto = descripcionOriginal,
                            EsExtra = true,
                            CantidadFacturada = Convert.ToInt32(cantidadFactura),
                            PrecioUnitario = cantidadFactura != 0 ? importe / cantidadFactura : 0,
                            PrecioTotal = importe
                        });
                    }
                }

                model.facturaDetallePreview = preview;

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error procesando archivo");
                return Json(new { res = -1, msg = ex.Message });
            }


            return Json(new { res, url = await renderService.RenderViewToStringAsync("~/Views/SubirFactura/ProductosOrdenar.cshtml", model) });

        }

        private SubirFacturaVM CargarDetalleOrden(int IdOrdenDeCompra)
        {
            SubirFacturaVM model = new SubirFacturaVM();

            model.ordenDeCompra = ordenDeCompraService.SelectOrdenDeCompra(IdOrdenDeCompra);
            model.productosOrdenar = ordenDeCompraService.SelectProductosOrdenarSelected(IdOrdenDeCompra);
            model.historialFacturas = ordenDeCompraService.SelectFacturasByIdOrdenDeCompra(IdOrdenDeCompra);
            model.resumenFacturacion = ordenDeCompraService.SelectResumenFacturacionByIdOrdenDeCompra(IdOrdenDeCompra);

            return model;
        }

        static string ReplaceDiacritics(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            // First, remove decomposable accents
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            string result = stringBuilder.ToString().Normalize(NormalizationForm.FormC);

            // Handle special letters that don't decompose automatically
            result = result
                .Replace('ł', 'l')
                .Replace('Ł', 'L')
                .Replace('đ', 'd')
                .Replace('Đ', 'D')
                .Replace('ø', 'o')
                .Replace('Ø', 'O')
                .Replace('ß', 's'); // or "ss" if you prefer

            return result;
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
