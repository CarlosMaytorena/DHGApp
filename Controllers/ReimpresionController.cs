using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.Services.Admin;
using AgricolaDH_GApp.ViewModels;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;

namespace AgricolaDH_GApp.Controllers
{
    public class ReimpresionRequest
    {
        public string? Orden { get; set; }
        public List<string>? Seriales { get; set; }
    }

    public class ReimpresionController : Controller
    {
        private readonly AppDbContext _db;
        public ReimpresionController(AppDbContext db) => _db = db;

        [HttpGet]
        public IActionResult Index() => PartialView("~/Views/Reimpresion/Index.cshtml");

        [HttpPost]
        public IActionResult ValidarSeriales([FromBody] ReimpresionRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Orden) || req.Seriales == null || req.Seriales.Count == 0)
                return Json(new { success = false, message = "Datos incompletos." });

            var orden = req.Orden.Trim();
            var seriales = req.Seriales
                .Select(s => (s ?? "").Trim().ToUpper())
                .Where(s => s != "")
                .Distinct()
                .ToList();

            // 1) Trae los seriales válidos (por orden y serial) y su PN desde SerialMap
            var valid = _db.SerialMap
                .Where(sm => sm.OrderNumber == orden && seriales.Contains(sm.SerialKey))
                .Select(sm => new { sm.SerialKey, sm.PartNumber })   // PartNumber == PN
                .ToList();

            var serialesValidos = valid.Select(v => v.SerialKey).ToList();
            if (serialesValidos.Count == 0)
                return Json(new { success = true, seriales = Array.Empty<string>(), productos = new Dictionary<string, string>() });

            // 2) Busca NombreProducto en Producto usando PN
            var pnSet = valid.Select(v => v.PartNumber).Distinct().ToList();

            var nombrePorPn = _db.Productos
                .Where(p => pnSet.Contains(p.PN))
                .Select(p => new { p.PN, p.NombreProducto })
                .ToDictionary(x => x.PN, x => x.NombreProducto);

            // 3) Mapea { serial -> NombreProducto } (si no encuentra, deja null/empty)
            var productosPorSerial = valid.ToDictionary(
                v => v.SerialKey,
                v => nombrePorPn.TryGetValue(v.PartNumber, out var nombre) ? nombre : ""
            );

            return Json(new
            {
                success = true,
                seriales = serialesValidos,
                productos = productosPorSerial
            });
        }
    }
}