using AgricolaDH_GApp.Models;

namespace AgricolaDH_GApp.ViewModels
{
    public class ReporteEgresosVM
    {
        public string SerialEgreso { get; set; }
        public DateTime Fecha { get; set; }
        public string NombreProducto { get; set; }
        public string SerialAlmacen { get; set; }
        public string Solicitante { get; set; }
        public string Path { get; set; }

        public ReporteEgresosVM()
        {
            SerialEgreso = string.Empty;
            NombreProducto = string.Empty;
            SerialAlmacen = string.Empty;
            Solicitante = string.Empty;
            Path = string.Empty;
        }
    }
}
