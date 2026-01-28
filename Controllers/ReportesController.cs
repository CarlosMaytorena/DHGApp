using System.Diagnostics;
using System.Web.WebPages.Html;
using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.Services.Admin;
using AgricolaDH_GApp.ViewModels;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

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
            
            
            if (tipo == "entradas"){

                var model = _reporteService.SelectReporteMovimientos(
                    2,
                    fechaInicio,
                    fechaFin,
                    idArea,
                    idSolicitante
                    );
                return PartialView(
                        "~/Views/Reportes/Partials/_ReporteEntradasSalidas.cshtml",
                        model
                    );
            }

            if (tipo == "salidas")
            {
                var model = _reporteService.SelectReporteMovimientos(
                    3,
                    fechaInicio,
                    fechaFin,
                    idArea,
                    idSolicitante
                    );
                return PartialView(
                        "~/Views/Reportes/Partials/_ReporteEntradasSalidas.cshtml",
                        model
                    );
            }



            //int idMovimiento, // 2 = Entradas, 3 = Salidas
            //DateTime? fechaInicio,
            //DateTime? fechaFin,
            //int? idArea,
            //int? idSolicitante


            if (tipo == "salidas"){
                return PartialView("_ReporteEntradasSalidas");
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

        [HttpGet]
        public IActionResult ReporteSalidas()
        {
            return CargarMovimientos(3); // 3 = Salidas
        }

        [HttpGet]
        public IActionResult ReporteEntradas()
        {
            return CargarMovimientos(2); // 2 = Entradas
        }

        private IActionResult CargarMovimientos(int idMovimiento)
        {
            var listaPlano = _context.Database
                .SqlQueryRaw<MovimientoPlanoVM>(@"
                select 
                    lap.IdLogsAlmacen,
                    concat(alm.Nombre, ' ', alm.ApellidoPaterno) as Almacenista,
                    concat(sol.Nombre, ' ', sol.ApellidoPaterno) as Solicitante,
                    lap.SerialKey,
                    la.Fecha
                from LogsAlmacenProductos lap
                join LogsAlmacen la on lap.IdLogsAlmacen = la.IdLogsAlmacen
                join Usuarios sol on la.IdSolicitante = sol.IdUsuario
                join Usuarios alm on la.IdAlmacenista = alm.IdUsuario
                where la.IdMovimiento = @idMovimiento
                order by la.Fecha desc
            ",
                new SqlParameter("@idMovimiento", idMovimiento)
                ).ToList();

            var movimientos = listaPlano
                .GroupBy(x => x.IdLogsAlmacen)
                .Select(g => new MovimientoAlmacenVM
                {
                    IdLogsAlmacen = g.Key,
                    Almacenista = g.First().Almacenista,
                    Solicitante = g.First().Solicitante,
                    Fecha = g.First().Fecha,
                    Productos = g.Select(p => new MovimientoProductoVM
                    {
                        SerialKey = p.SerialKey
                    }).ToList()
                })
                .ToList();

            var vm = new ReporteMovimientosVM
            {
                Movimientos = movimientos
            };

            return PartialView("~/Views/Reportes/Movimientos.cshtml", vm);
        }


}
}
