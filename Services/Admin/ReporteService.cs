using System.Diagnostics;
using System.Web.WebPages.Html;
using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace AgricolaDH_GApp.Services.Admin
{
    public class ReporteService
    {
        private readonly AppDbContext context;

        public ReporteService(AppDbContext _ctx)
        {
            context = _ctx;
        }

        public List<SelectListItem> GetAreas()
        {
            return context.Areas
                .AsNoTracking()
                .Select(a => new SelectListItem
                {
                    Value = a.IdArea.ToString(),
                    Text = a.Descripcion
                })
                .ToList();
        }

        public List<SelectListItem> GetSolicitantes()
        {
            return context.Usuarios
                .AsNoTracking()
                .Select(u => new SelectListItem
                {
                    Value = u.IdUsuario.ToString(),
                    Text = (u.Nombre ?? "") + " " + (u.ApellidoPaterno ?? "")
                })
                .ToList();
        }


        public List<ReporteEgresosVM> SelectReporteEgresos(
            DateTime? fechaInicio,
            DateTime? fechaFin,
            int? idArea,
            int? idSolicitante
)
        {
            var query =
                from log in context.LogsEgresos
                join e in context.Egresos
                    on log.IdLogsEgresos equals e.IdLogsEgresos
                join u in context.Usuarios
                    on log.IdSolicitante equals u.IdUsuario
                join a in context.Areas
                    on u.IdArea equals a.IdArea
                select new
                {
                    log.IdLogsEgresos,
                    log.Folio,
                    Serial = e.SerialNumber,
                    Solicitante =
                        (u.Nombre ?? "") + " " + (u.ApellidoPaterno ?? ""),
                    log.Fecha,
                    u.IdArea,
                    Area = a.Descripcion,
                    u.IdUsuario
                };

            // 🔹 Filtros
            if (fechaInicio.HasValue)
                query = query.Where(x => x.Fecha >= fechaInicio.Value);

            if (fechaFin.HasValue)
                query = query.Where(x => x.Fecha < fechaFin.Value.Date.AddDays(1));

            if (idArea.HasValue)
                query = query.Where(x => x.IdArea == idArea.Value);

            if (idSolicitante.HasValue)
                query = query.Where(x => x.IdUsuario == idSolicitante.Value);

            var data = query.AsNoTracking().ToList();

            return data
                .GroupBy(x => x.IdLogsEgresos)
                .Select(g => new ReporteEgresosVM
                {
                    IdLogsEgresos = g.Key,
                    Folio = g.First().Folio ?? "",
                    Solicitante = g.First().Solicitante,
                    Fecha = g.First().Fecha,

                    Detalles = g.Select(x => new ReporteEgresoDetalleVM
                    {
                        Serial = x.Serial,
                        Solicitante = x.Solicitante,
                        Fecha = x.Fecha
                    }).ToList()
                })
                .ToList();
        }

    }
}
