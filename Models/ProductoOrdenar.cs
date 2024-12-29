using Microsoft.EntityFrameworkCore;

namespace AgricolaDH_GApp.Models
{
    public class ProductoOrdenar
    {
        public int IdProductoOrdenar { get; set; }
        public int IdOrdenDeCompra { get; set; }
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
       
    }
}
