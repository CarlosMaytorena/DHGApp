using System.Diagnostics;
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
        public List<ReporteEgresosVM> SelectReporteEgresos(
            DateTime? fechaInicio,
            DateTime? fechaFin
        )
        {
            try
            {
                var query =
                    from log in context.LogsEgresos
                    join e in context.Egresos
                        on log.IdLogsEgresos equals e.IdLogsEgresos
                    join u in context.Usuarios
                        on log.IdSolicitante equals u.IdUsuario
                    select new
                    {
                        Log = log,
                        Egreso = e,
                        Usuario = u
                    };

                var totalAntesDeFechas = query.Count();
                Debug.WriteLine("TOTAL ANTES DE FILTROS: " + totalAntesDeFechas);



                // 🔹 FILTRO FECHAS
                if (fechaInicio.HasValue)
                {
                    query = query.Where(x => x.Log.Fecha >= fechaInicio.Value);
                }

                if (fechaFin.HasValue)
                {
                    var fechaFinCompleta = fechaFin.Value.Date.AddDays(1);
                    query = query.Where(x => x.Log.Fecha < fechaFinCompleta);
                }

                var totalDespuesDeFechas = query.Count();
                Debug.WriteLine("TOTAL DESPUÉS DE FILTROS: " + totalDespuesDeFechas);


                return query
                .AsNoTracking()
                .Select(x => new ReporteEgresosVM
                {
                    Folio = x.Log.Folio ?? "",   // 🔥 AQUÍ
                    SerialEgreso = x.Egreso.SerialNumber ?? "",
                    Solicitante =
                        (x.Usuario.Nombre ?? "") + " " + (x.Usuario.ApellidoPaterno ?? ""),
                    Fecha = x.Log.Fecha
                })
                .ToList();

            }
            catch
            {
                return new List<ReporteEgresosVM>();
            }
        }











    }
}
