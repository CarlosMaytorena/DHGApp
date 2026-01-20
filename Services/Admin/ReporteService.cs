using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.ViewModels;

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
            List<ReporteEgresosVM> lista;

            try
            {
                var query =
                    from e in context.Egresos
                    join p in context.Productos on e.IdProducto equals p.IdProducto
                    join u in context.Usuarios on e.IdSolicitante equals u.IdUsuario
                    join ev in context.Evidencia on e.IdEvidencia equals ev.IdEvidencia
                    join a in context.Almacen on e.IdAlmacen equals a.IdAlmacen
                    select new
                    {
                        e,
                        p,
                        u,
                        ev,
                        a
                    };

                // 🔥 FILTRO DE FECHAS
                if (fechaInicio.HasValue)
                {
                    query = query.Where(x => x.e.Fecha >= fechaInicio.Value);
                }

                if (fechaFin.HasValue)
                {
                    // incluye todo el día final
                    var finDia = fechaFin.Value.Date.AddDays(1);
                    query = query.Where(x => x.e.Fecha < finDia);
                }

                lista = query
                    .Select(x => new ReporteEgresosVM
                    {
                        SerialEgreso = x.e.SerialNumber,
                        Fecha = x.e.Fecha,
                        NombreProducto = x.p.NombreProducto,
                        SerialAlmacen = x.a.SerialNumber,
                        Solicitante = x.u.Nombre + " " + x.u.ApellidoPaterno,
                        Path = x.ev.PathDespues
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                // si quieres debuggear:
                // Debug.WriteLine(ex.Message);
                lista = new List<ReporteEgresosVM>();
            }

            return lista;
        }










    }
}
