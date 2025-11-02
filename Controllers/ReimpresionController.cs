using AgricolaDH_GApp.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AgricolaDH_GApp.Controllers
{

    public class ReimpresionRequest
    {
        public string? Orden { get; set; }
        public List<string>? Seriales { get; set; }
    }

    public class OrdenRequest
    {
        public string? Orden { get; set; }
    }

    public class ReimpresionController : Controller
    {
        private readonly AppDbContext _db;
        public ReimpresionController(AppDbContext db) => _db = db;

        [HttpGet]
        public IActionResult Index()
        {
            return PartialView("~/Views/Reimpresion/Index.cshtml");
        }
       
        [HttpPost]
        public IActionResult ValidarSeriales([FromBody] ReimpresionRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Orden) || req.Seriales == null || req.Seriales.Count == 0)
                return Json(new { success = false, message = "Datos incompletos." });

            // normalizamos la orden como string (tal cual está guardada en DB)
            var ordenTexto = req.Orden.Trim();

            // normalizamos lista de seriales a MAYÚSCULAS
            var serialesNormalizados = req.Seriales
                .Select(s => (s ?? "").Trim().ToUpper())
                .Where(s => s != "")
                .Distinct()
                .ToList();

            // buscamos en SerialMap por esa orden y por cada serial
            var valid = _db.SerialMap
                .Where(sm =>
                    sm.OrderNumber == ordenTexto &&
                    serialesNormalizados.Contains(sm.SerialKey.ToUpper())
                )
                .Select(sm => new
                {
                    sm.SerialKey,    // serial físico impreso
                    sm.PartNumber    // PN
                })
                .ToList();

            var serialesValidos = valid
                .Select(v => v.SerialKey)
                .Distinct()
                .ToList();

            if (serialesValidos.Count == 0)
            {
                return Json(new
                {
                    success = true,
                    seriales = Array.Empty<string>(),
                    productos = new Dictionary<string, string>()
                });
            }

            // buscamos nombres de producto a partir del PN
            var pnSet = valid
                .Select(v => v.PartNumber)
                .Distinct()
                .ToList();

            var nombrePorPn = _db.Productos
                .Where(p => pnSet.Contains(p.PN))
                .Select(p => new { p.PN, p.NombreProducto })
                .ToDictionary(x => x.PN, x => x.NombreProducto);

            // construimos diccionario serial -> NombreProducto
            var productosPorSerial = valid
                .GroupBy(v => v.SerialKey)
                .ToDictionary(
                    g => g.Key,
                    g =>
                    {
                        var pn = g.Select(x => x.PartNumber).FirstOrDefault();
                        return (pn != null && nombrePorPn.TryGetValue(pn, out var nombre))
                            ? nombre
                            : "";
                    }
                );

            return Json(new
            {
                success = true,
                seriales = serialesValidos,
                productos = productosPorSerial
            });
        }

        [HttpPost]
        public IActionResult ObtenerSerialesPorOrden([FromBody] OrdenRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Orden))
            {
                return Json(new
                {
                    success = false,
                    error = "Orden no proporcionada."
                });
            }

            var ordenTexto = req.Orden.Trim();

            // 1. Traer todos los seriales de esa orden
            var registros = _db.SerialMap
                .Where(sm => sm.OrderNumber == ordenTexto)
                .Select(sm => new
                {
                    sm.SerialKey,    // serial
                    sm.PartNumber,   // PN
                    sm.CreatedAt     // fecha en que se generó el serial
                })
                .ToList();

            if (registros.Count == 0)
            {
                return Json(new
                {
                    success = true,
                    orden = ordenTexto,
                    data = new List<object>()
                });
            }

            // 2. Resolver el nombre del producto a partir del PN
            var pnSet = registros
                .Select(r => r.PartNumber)
                .Distinct()
                .ToList();

            var nombresPorPn = _db.Productos
                .Where(p => pnSet.Contains(p.PN))
                .Select(p => new { p.PN, p.NombreProducto })
                .ToDictionary(x => x.PN, x => x.NombreProducto);

            // 3. Armar filas para la tabla del front
            var data = registros
                .Select(r => new
                {
                    serial = r.SerialKey,
                    producto = nombresPorPn.TryGetValue(r.PartNumber, out var nombre) ? nombre : "",
                    fecha = r.CreatedAt.ToString("yyyy-MM-dd")
                })
                .OrderBy(r => r.serial)
                .ToList();

            return Json(new
            {
                success = true,
                orden = ordenTexto,
                data = data
            });
        }
    }
}
