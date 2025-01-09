using AgricolaDH_GApp.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace AgricolaDH_GApp.Models
{
    /// <summary>
    /// Estructura para mapear en db
    /// </summary>
    public class Almacen
    {
        public int IdAlmacen { get; set; }
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }

    }
    /// <summary>
    /// Estructura para mostrar en vista
    /// </summary>
    public class AlmacenView
    {
        public int IdAlmacen { get; set; }
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
        public string? NombreProducto { get; set; }
        public string? Descripcion { get; set; }
    }

}
