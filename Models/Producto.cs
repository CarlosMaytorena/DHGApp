using Microsoft.EntityFrameworkCore;

namespace AgricolaDH_GApp.Models
{
    public class Producto
    {
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; }
        public string Descripcion { get; set; }
        public string Proveedor { get; set; }
        public double Costo { get; set; }
       
    }
}
