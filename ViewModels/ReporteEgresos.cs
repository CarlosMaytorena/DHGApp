using AgricolaDH_GApp.Models;

namespace AgricolaDH_GApp.ViewModels
{
    public class ReporteEgresosVM
    {
        public string? Folio { get; set; } = "";

        public string SerialNumber { get; set; }

        public string SerialEgreso { get; set; } = "";
        public DateTime? Fecha { get; set; }
        public string NombreProducto { get; set; }
        public string SerialAlmacen { get; set; }
        public string Solicitante { get; set; } = "";

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
}
