using Microsoft.EntityFrameworkCore;

namespace AgricolaDH_GApp.Models
{
    public class ProductoOrdenar
    {
        public int IdProductoOrdenar { get; set; }
        public int IdOrdenDeCompra { get; set; }
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal? Unidad { get; set; }
        public decimal? Total { get; set; }
        public decimal? Impuesto { get; set; }
        public int PorRecibir { get; set; }
    }

    public class ProductoOrdenarSelected
    {
        public int IdProductoOrdenar { get; set; }
        public int IdOrdenDeCompra { get; set; }
        public string Producto { get; set; }
        public int Cantidad { get; set; }
        public decimal? Unidad { get; set; }
        public decimal? Total { get; set; }
        public decimal? Impuesto { get; set; }
        public decimal? Descuento { get; set; }
        public int PorRecibir { get; set; }

    }

    public class ProductoRecibidoDTO
    {
        public int IdProductoOrdenar { get; set; }
        public int Recibida { get; set; }
        public int PorRecibir { get; set; }
        public List<string> Seriales { get; set; }
    }
}
