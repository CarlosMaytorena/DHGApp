using AgricolaDH_GApp.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgricolaDH_GApp.Models
{
    /// <summary>
    /// Estructura para mapear en db
    /// </summary>
    public class Egreso
    {
        public int IdEgreso { get; set; }
        public int IdProducto { get; set; }
        public string SerialNumber { get; set; } //Unique ID
        public int IdSolicitante { get; set; }
        public int IdAlmacen { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Date only")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime Fecha { get; set; }
        public int IdEvidencia { get; set; }
        [NotMapped]
        public string? PathAntes { get; set; }
        [NotMapped]
        public string? PathDespues { get; set; }
        [NotMapped]
        public IFormFile FileAntes { get; set; }
        [NotMapped]
        public IFormFile FileDespues { get; set; }
        [NotMapped]
        public string? Solicitante { get; set; }
        [NotMapped]
        public string? Producto { get; set; }
        [NotMapped]
        public bool esAutorizado { get;set; }

    }
}
