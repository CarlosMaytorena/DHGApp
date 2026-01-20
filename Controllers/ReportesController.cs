using System.Diagnostics;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.Services.Admin;
using Microsoft.AspNetCore.Mvc;

namespace AgricolaDH_GApp.Controllers
{
    public class ReportesController : Controller
    {
        private readonly ReporteService _reporteService;
        private readonly ILogger<AlmacenController> _logger;


        public ReportesController(
            ILogger<AlmacenController> logger,
            ReporteService reporteService)

        {
            _logger = logger;
            _reporteService = reporteService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return PartialView("~/Views/Reportes/Index.cshtml");
        }

        [HttpGet]
        public IActionResult LoadReporte(string tipo, DateTime? fechaInicio, DateTime? fechaFin)
        {
            Debug.WriteLine($"tipo: {tipo}");
            Debug.WriteLine($"inicio: {fechaInicio}");
            Debug.WriteLine($"fin: {fechaFin}");

            if (tipo == "egresos")
            {
                var model = _reporteService.SelectReporteEgresos(fechaInicio, fechaFin);
                return PartialView("~/Views/Reportes/Partials/_ReporteEgresos.cshtml", model);
            }

            return Content("Reporte no válido");
        }


    }
}
