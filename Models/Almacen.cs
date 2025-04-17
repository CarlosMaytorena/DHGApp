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
        public string? SerialNumber { get; set; }//Unique ID
        public int? IdSolicitante { get; set; }
        public int? IdAlmacenista { get; set; }
        public string? Movimiento { get; set; }
        public string? RazonUso { get; set; }
        public bool Uso { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Date only")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime Fecha { get; set; }
        public int IdEstatus { get; set; }
        [NotMapped]
        public string? NombreProducto { get; set; }
        [NotMapped]
        public string? Descripcion { get; set; }
        [NotMapped]
        public int Unidades { get; set; } = 1;
        [NotMapped]
        public string? Almacenista { get; set; }
        [NotMapped]
        public string? Solicitante { get; set; }
        [NotMapped]
        public string? Estatus { get; set; }
    }
}
