using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgricolaDH_GApp.Models
{
    public class Producto
    {
        public int IdProducto { get; set; }
        public int ClaveProveedor { get; set; }
        public string NombreProducto { get; set; }
        public string UnidadDeVenta { get; set; }
        public string UnidadDeSKU { get; set; }
        public int Contenido { get; set; }
        public string NombreInterno { get; set; }
        public string Compania { get; set; }
        public string Descripcion { get; set; }
        public string SKUInterno { get; set; }
        public string PN { get; set; }
    }
}
