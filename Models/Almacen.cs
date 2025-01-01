using Microsoft.EntityFrameworkCore;

namespace AgricolaDH_GApp.Models
{
    public class Almacen
    {
        public int IdAlmacen { get; set; }
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }

    }
}
