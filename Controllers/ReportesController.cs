using System.Diagnostics;
using System.Web.WebPages.Html;
using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.Services.Admin;
using AgricolaDH_GApp.ViewModels;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace AgricolaDH_GApp.Controllers
{
    public class ReportesController : Controller
    {
        private readonly ReporteService _reporteService;
        private readonly ILogger<AlmacenController> _logger;
        private readonly AppDbContext _context;



        public ReportesController(
            ILogger<AlmacenController> logger,
            ReporteService reporteService,
            AppDbContext context
            )

        {
            _logger = logger;
            _reporteService = reporteService;
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // 🩹 para que el layout no truene
            ViewBag.Usuario = new Usuario();

            var areas = _context.Areas
                .Select(a => new SelectListItem
                {
                    Value = a.IdArea.ToString(),
                    Text = a.Descripcion
                })
                .ToList();

            var solicitantes = _context.Usuarios
                .Select(u => new SelectListItem
                {
                    Value = u.IdUsuario.ToString(),
                    Text = u.Nombre + " " + u.ApellidoPaterno
                })
                .ToList();

            var model = new ReporteEgresosPageVM
            {
                Reporte = new List<ReporteEgresosVM>(),
                Filtros = new ReporteEgresosFiltroVM
                {
                    Areas = areas,
                    Solicitantes = solicitantes
                }
            };

            return PartialView("~/Views/Reportes/Index.cshtml", model);
        }



        [HttpGet]
        public IActionResult LoadReporte(
            string tipo,
            DateTime? fechaInicio,
            DateTime? fechaFin,
            int? idArea,
            int? idSolicitante
        )
        {
            if (tipo == "egresos")
            {
                var model = _reporteService.SelectReporteEgresos(
                    fechaInicio,
                    fechaFin,
                    idArea,
                    idSolicitante
                );

                return PartialView(
                    "~/Views/Reportes/Partials/_ReporteEgresos.cshtml",
                    model
                );
            }

            return Content("Reporte no válido");
        }


        [HttpGet]
        public IActionResult ReporteEgresos()
        {
            var data = (
                from l in _context.LogsEgresos
                join e in _context.Egresos on l.IdLogsEgresos equals e.IdLogsEgresos
                join u in _context.Usuarios on l.IdSolicitante equals u.IdUsuario
                select new
                {
                    l.IdLogsEgresos,
                    l.Folio,
                    Serial = e.SerialNumber,
                    Solicitante = u.Nombre + " " + u.ApellidoPaterno,
                    l.Fecha
                }
            ).ToList();

            var model = data
                .GroupBy(x => x.IdLogsEgresos)
                .Select(g => new ReporteEgresosVM
                {
                    IdLogsEgresos = g.Key,
                    Folio = g.First().Folio,
                    Solicitante = g.First().Solicitante,
                    Fecha = g.First().Fecha,
                    Seriales = g.Select(x => x.Serial).ToList()
                })
                .ToList();

            return View(model);


        }

        public ReporteEgresosFiltroVM GetFiltrosReporteEgresos()
        {
            return new ReporteEgresosFiltroVM
            {
                Areas = _context.Areas
                    .Select(a => new SelectListItem
                    {
                        Value = a.IdArea.ToString(),
                        Text = a.Descripcion
                    }).ToList(),

                Solicitantes = _context.Usuarios
                    .Select(u => new SelectListItem
                    {
                        Value = u.IdUsuario.ToString(),
                        Text = (u.Nombre ?? "") + " " + (u.ApellidoPaterno ?? "")
                    }).ToList()
            };
        }


    }
}
