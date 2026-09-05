using Microsoft.EntityFrameworkCore;

namespace AgricolaDH_GApp.Models
{
    public class Ingreso
    {
        public int IdIngreso { get; set; }
        public int IdOrdenDeCompra { get; set; }
        public int IdUsuario { get; set; }
        public DateTime Fecha { get; set; }
        public bool Cancelado { get; set; }
    }

    public class IngresoDetalle
    {
        public int IdIngresoDetalle { get; set; }
        public int IdIngreso { get; set; }
        public int IdOrdenDeCompra { get; set; }
        public int IdProductoOrdenar { get; set; }
        public int IdProducto { get; set; }
        public int CantidadRecibida { get; set; }
    }
}
