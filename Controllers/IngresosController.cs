using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.Services;
using AgricolaDH_GApp.Services.Admin;
using AgricolaDH_GApp.ViewModels;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AgricolaDH_GApp.Controllers
{
    public class IngresosController : Controller
    {
        private readonly ILogger<IngresosController> _logger;
        private readonly OrdenDeCompraService _ordenDeCompraService;
        private readonly AlmacenService _almacenService;
        private readonly ProductoService _productoService;
        private readonly SerialMapService _serialMapService;
        private readonly LogsAlmacenService _logsAlmacenService;
        private readonly IngresoService _ingresoService;
        private readonly AppDbContext _context;

        public IngresosController(
            ILogger<IngresosController> logger,
            OrdenDeCompraService ordenDeCompraService,
            AlmacenService almacenService,
            ProductoService productoService,
            SerialMapService serialMapService,
            LogsAlmacenService logsAlmacenService,
            IngresoService ingresoService,
            AppDbContext context)
        {
            _logger = logger;
            _ordenDeCompraService = ordenDeCompraService;
            _almacenService = almacenService;
            _productoService = productoService;
            _serialMapService = serialMapService;
            _logsAlmacenService = logsAlmacenService;
            _ingresoService = ingresoService;
            _context = context;
        }

        [HttpGet]
        public IActionResult Index(int closedWeeks = 1)
        {
            int idUsuario = Convert.ToInt32(HttpContext.Session.GetInt32("IdUsuario"));

            var model = new SubirFacturaVM
            {
                // Para Recibo: cualquier orden aceptada y no cerrada (ya no depende de factura ni de status exacto).
                subirFacturaList = _ordenDeCompraService.SelectOrdenDeCompraTableEnProceso(idUsuario, 0),
                // Cerradas: cierre manual desde Requisiciones (status Cerrado). El parámetro viene del dropdown (1, 2 o 0 = sin filtro).
                ordenesCerradas = _ordenDeCompraService.SelectOrdenDeCompraTableList(OrdenDeCompraStatusEnumerators.Cerrado, idUsuario, closedWeeks)
            };

            ViewBag.ClosedWeeks = closedWeeks; // para que el dropdown recuerde el valor
            return PartialView("~/Views/Ingresos/Index.cshtml", model);
        }

        [HttpPost]
        public IActionResult RealizarIngreso(int idOrdenDeCompra)
        {
            var model = CargarFormularioIngreso(idOrdenDeCompra);
            model.productoList = _productoService.SelectProductos();

            return PartialView("~/Views/Ingresos/IngresoForm.cshtml", model);
        }

        private SubirFacturaVM CargarFormularioIngreso(int idOrdenDeCompra)
        {
            var ordenDeCompra = _ordenDeCompraService.SelectOrdenDeCompra(idOrdenDeCompra);
            var productosOrdenar = _ordenDeCompraService.SelectProductosOrdenarSelected(idOrdenDeCompra);

            var acumulados = _ingresoService.SelectCantidadRecibidaAcumulada(idOrdenDeCompra);

            // Precio unitario de la factura más reciente por producto, si alguna vez se subió una
            // (left join: no todos los productos tienen factura cargada).
            var preciosFactura = _ordenDeCompraService.SelectResumenFacturacionByIdOrdenDeCompra(idOrdenDeCompra)
                .Where(r => r.IdProductoOrdenar.HasValue)
                .ToDictionary(r => r.IdProductoOrdenar!.Value, r => r.PrecioUnitario);

            foreach (var producto in productosOrdenar)
            {
                producto.RecibidoAcumulado = acumulados.TryGetValue(producto.IdProductoOrdenar, out var cantidad) ? cantidad : 0;
                producto.PrecioUnitarioFactura = preciosFactura.TryGetValue(producto.IdProductoOrdenar, out var precio) ? precio : null;
            }

            return new SubirFacturaVM
            {
                ordenDeCompra = ordenDeCompra,
                productosOrdenar = productosOrdenar,
                soloLectura = ordenDeCompra != null && ordenDeCompra.IdOrdenDeCompraStatus == OrdenDeCompraStatusEnumerators.Cerrado
            };
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

        [HttpGet]
        public JsonResult GetProductBarcodeID(string nombreProducto, [FromServices] ProductoService productoService)
        {
            var producto = productoService.SelectProductoByName(nombreProducto);

            if (producto != null)
                return Json(new { success = true, barcodeID = producto.PN });

            return Json(new { success = false, message = "Producto no encontrado." });
        }

        [HttpPost]
        public IActionResult ActualizarPorRecibir([FromBody] List<ProductoRecibidoDTO> receivedProducts)
        {
            if (receivedProducts == null || receivedProducts.Count == 0)
                return Json(new { success = false });

            int idUsuario = Convert.ToInt32(HttpContext.Session.GetInt32("IdUsuario"));

            int idOrdenDeCompra = _ordenDeCompraService
                .SelectProductoOrdenar(receivedProducts[0].IdProductoOrdenar)?.IdOrdenDeCompra ?? 0;

            var orden = _ordenDeCompraService.SelectOrdenDeCompra(idOrdenDeCompra);

            if (orden == null || orden.IdOrdenDeCompraStatus == OrdenDeCompraStatusEnumerators.Cerrado)
            {
                return Json(new { success = false });
            }

            var orderNumber = orden.IdOrdenDeCompra.ToString();

            var ingreso = new Ingreso
            {
                IdOrdenDeCompra = idOrdenDeCompra,
                IdUsuario = idUsuario,
                Fecha = DateTime.Now
            };
            int res = _ingresoService.InsertIngreso(ingreso);
            if (res != 0)
            {
                return Json(new { success = false });
            }

            foreach (var item in receivedProducts)
            {
                if (item.Recibida <= 0) continue;

                var productoOrdenar = _ordenDeCompraService.SelectProductoOrdenar(item.IdProductoOrdenar);
                if (productoOrdenar == null) continue;

                var producto = _productoService.SelectProducto(productoOrdenar.IdProducto);
                if (producto == null) continue;

                _ingresoService.InsertIngresoDetalle(new IngresoDetalle
                {
                    IdIngreso = ingreso.IdIngreso,
                    IdOrdenDeCompra = idOrdenDeCompra,
                    IdProductoOrdenar = item.IdProductoOrdenar,
                    IdProducto = producto.IdProducto,
                    CantidadRecibida = item.Recibida
                });

                if (item.SerialesCortos == null || item.SerialesCortos.Count == 0) continue;

                // Modelo para Almacen y Logs
                AlmacenVM model = new AlmacenVM
                {
                    almacenLista = new List<Almacen>(),
                };

                foreach (var serial in item.SerialesCortos)
                {
                    try
                    {
                        _serialMapService.InsertSerial(
                            serialKey: (serial ?? string.Empty).ToUpperInvariant(),
                            orderNumber: orderNumber,
                            partNumber: producto.PN ?? string.Empty,
                            idIngreso: ingreso.IdIngreso
                        );
                        _almacenService.GuardarEnAlmacen(producto.IdProducto, serial, ingreso.IdIngreso);

                        // Registro de log en Almacen
                        Almacen a = _context.Almacen.
                            Single(x => x.IdProducto == producto.IdProducto && x.SerialNumber == serial);
                        model.almacenLista.Add(a);
                    }
                    catch (System.Exception ex)
                    {
                        // Ignore duplicates or log as needed
                        _logger.LogWarning(ex, "SerialMap insert failed for {SerialKey}", serial);
                    }
                }

                if (model.almacenLista.Count > 0)
                {
                    //Agregar logs
                    model.logsAlmacen = _logsAlmacenService.InsertarLogsAlmacen(model, ingreso.IdIngreso);
                    _logsAlmacenService.InsertarLogsAlmacenProductos(model);
                }
            }

            return Json(new { success = true, idIngreso = ingreso.IdIngreso });
        }

        [HttpPost]
        public IActionResult CancelarIngreso(int idIngreso)
        {
            int idUsuario = Convert.ToInt32(HttpContext.Session.GetInt32("IdUsuario"));

            var ingreso = _ingresoService.SelectIngreso(idIngreso);
            if (ingreso == null || ingreso.Cancelado)
            {
                return Json(new { success = false, message = "Ingreso no encontrado o ya cancelado." });
            }

            if (ingreso.IdUsuario != idUsuario)
            {
                return Json(new { success = false, message = "No autorizado para cancelar este ingreso." });
            }

            var ultimo = _ingresoService.SelectUltimoIngreso(ingreso.IdOrdenDeCompra);
            if (ultimo == null || ultimo.IdIngreso != idIngreso)
            {
                return Json(new { success = false, message = "Ya no se puede cancelar: se registraron otros movimientos después." });
            }

            int res = _ingresoService.CancelarIngreso(idIngreso);
            return Json(new { success = res == 0 });
        }

        [HttpPost]
        public IActionResult AgregarProductoCatalogo([FromBody] AgregarProductoCatalogoRequest request)
        {
            var orden = _ordenDeCompraService.SelectOrdenDeCompra(request.IdOrdenDeCompra);
            if (orden == null || orden.IdOrdenDeCompraStatus == OrdenDeCompraStatusEnumerators.Cerrado)
            {
                return Json(new { success = false, message = "La orden está cerrada." });
            }

            var producto = _productoService.SelectProducto(request.IdProducto);
            if (producto == null)
            {
                return Json(new { success = false, message = "Producto no encontrado." });
            }

            var existente = _ordenDeCompraService.SelectProductosOrdenar(request.IdOrdenDeCompra)
                .FirstOrDefault(p => p.IdProducto == request.IdProducto);

            if (existente == null)
            {
                var nuevo = new ProductoOrdenar
                {
                    IdOrdenDeCompra = request.IdOrdenDeCompra,
                    IdProducto = request.IdProducto,
                    Cantidad = 0,
                    PorRecibir = 0
                };
                _ordenDeCompraService.InsertProductoOrdenar(nuevo);
            }

            var model = CargarFormularioIngreso(request.IdOrdenDeCompra);
            model.productoList = _productoService.SelectProductos();
            AplicarCapturaPrevia(model, request.CapturaPrevia);

            return PartialView("~/Views/Ingresos/IngresoForm.cshtml", model);
        }

        [HttpPost]
        public IActionResult EliminarProductoCatalogo([FromBody] EliminarProductoCatalogoRequest request)
        {
            var orden = _ordenDeCompraService.SelectOrdenDeCompra(request.IdOrdenDeCompra);
            if (orden == null || orden.IdOrdenDeCompraStatus == OrdenDeCompraStatusEnumerators.Cerrado)
            {
                return Json(new { success = false, message = "La orden está cerrada." });
            }

            var productoOrdenar = _ordenDeCompraService.SelectProductoOrdenar(request.IdProductoOrdenar);
            var acumulado = _ingresoService.SelectCantidadRecibidaAcumulada(request.IdOrdenDeCompra);
            bool recibidoAlgo = acumulado.TryGetValue(request.IdProductoOrdenar, out var cantidad) && cantidad > 0;

            // Solo se puede quitar una línea que nunca se ordenó formalmente (Cantidad=0,
            // agregada desde catálogo) y que tampoco tiene ningún ingreso registrado.
            if (productoOrdenar == null || productoOrdenar.IdOrdenDeCompra != request.IdOrdenDeCompra ||
                productoOrdenar.Cantidad != 0 || recibidoAlgo)
            {
                return Json(new { success = false, message = "Este producto ya no se puede quitar." });
            }

            _ordenDeCompraService.EliminarProductoOrdenar(request.IdProductoOrdenar);

            var model = CargarFormularioIngreso(request.IdOrdenDeCompra);
            model.productoList = _productoService.SelectProductos();
            AplicarCapturaPrevia(model, request.CapturaPrevia);

            return PartialView("~/Views/Ingresos/IngresoForm.cshtml", model);
        }

        private static void AplicarCapturaPrevia(SubirFacturaVM model, List<CapturaPreviaDTO>? capturaPrevia)
        {
            if (capturaPrevia == null || capturaPrevia.Count == 0) return;

            var valores = capturaPrevia.ToDictionary(c => c.IdProductoOrdenar, c => c.Cantidad);
            foreach (var producto in model.productosOrdenar)
            {
                if (valores.TryGetValue(producto.IdProductoOrdenar, out var cantidad))
                {
                    producto.CantidadCapturada = cantidad;
                }
            }
        }

        [HttpPost]
        public IActionResult VerOrden(int idOrdenDeCompra)
        {
            var model = CargarFormularioIngreso(idOrdenDeCompra);
            if (!model.soloLectura)
            {
                model.productoList = _productoService.SelectProductos();
            }

            return PartialView("~/Views/Ingresos/IngresoForm.cshtml", model);
        }
    }
}