using AgricolaDH_GApp.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgricolaDH_GApp.Models
{
    /// <summary>
    /// Estructura para mapear en db
    /// </summary>
    public class Almacen
    {
        public int IdAlmacen { get; set; }
        public int IdProducto { get; set; }
        public string? SerialNumber { get; set; } //Unique ID
        public int? IdSolicitante { get; set; }
        public int? IdAlmacenista { get; set; }
        public string? Movimiento { get; set; }
        public string? RazonUso { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Date only")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime Fecha { get; set; }
        public int IdEstatus { get; set; }
        [NotMapped]
        public string? NombreProducto { get; set; }
        [NotMapped]
        public string? Descripcion { get; set; }
        [NotMapped]
        public int Unidades { get; set; }
        [NotMapped]
        public string? Almacenista { get; set; }
        [NotMapped]
        public string? Solicitante { get; set; }
        [NotMapped]
        public string? Estatus { get; set; }
    }
    /// <summary>
    /// Estructura para mostrar en vista
    /// </summary>

    public class AlmacenDTO
    {
        public Almacen Almacen { get; set; }
        public int Motivo { get; set; }
        public Producto Producto { get; set; }
    }
}
