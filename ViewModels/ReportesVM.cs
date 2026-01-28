using System.Web.WebPages.Html;
using AgricolaDH_GApp.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace AgricolaDH_GApp.ViewModels
{
    public class ReporteEgresosVM
    {
        public int IdLogsEgresos { get; set; }
        public string? Folio { get; set; } = "";

        public string SerialNumber { get; set; }

        public string SerialEgreso { get; set; } = "";
        public DateTime? Fecha { get; set; }
        public string NombreProducto { get; set; }
        public string SerialAlmacen { get; set; }
        public string Solicitante { get; set; } = "";
        public List<string> Seriales { get; set; } = new();
        public List<ReporteEgresoDetalleVM> Detalles { get; set; } = new();



        public ReporteEgresosVM()
        {
            SerialNumber = string.Empty;
            SerialEgreso = string.Empty;
            NombreProducto = string.Empty;
            SerialAlmacen = string.Empty;
            Solicitante = string.Empty;
            Folio = string.Empty;
        }
    }

    public class ReporteEgresoDetalleVM
    {
        public string Serial { get; set; }
        public string Solicitante { get; set; }
        public DateTime? Fecha { get; set; }
    }

    public class ReporteEgresosFiltroVM
    {
        public int? IdArea { get; set; }
        public int? IdSolicitante { get; set; }

        public List<SelectListItem> Areas { get; set; } = new();
        public List<SelectListItem> Solicitantes { get; set; } = new();
    }

    public class ReporteEgresosPageVM
    {
        public List<ReporteEgresosVM> Reporte { get; set; } = new();
        public ReporteEgresosFiltroVM Filtros { get; set; } = new();
    }




    public class MovimientoPlanoVM
    {
        public int IdLogsAlmacen { get; set; }
        public string Almacenista { get; set; }
        public string Solicitante { get; set; }
        public string SerialKey { get; set; }
        public DateTime Fecha { get; set; }
    }


    public class MovimientoProductoVM
    {
        public string SerialKey { get; set; }
        public string Solicitante { get; set; }

    }

    public class MovimientoAlmacenVM
    {
        public int IdLogsAlmacen { get; set; }
        public string Almacenista { get; set; }
        public string Solicitante { get; set; }
        public DateTime Fecha { get; set; }

        public List<MovimientoProductoVM> Productos { get; set; } = new();
    }

    public class ReporteMovimientosVM
    {
        public List<MovimientoAlmacenVM> Movimientos { get; set; }
    }



}
