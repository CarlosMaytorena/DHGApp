using AgricolaDH_GApp.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AgricolaDH_GApp.Models
{
    /// <summary>
    /// Estructura para mapear en db
    /// </summary>
    public class Movimiento
    {
        public int IdMovimiento { get; set; }
        public int IdProducto { get; set; }
        public string? Producto { get; set; }
        public int Cantidad { get; set; }
        public string Solicitante { get; set; }
        public string AprobadoPor { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Date only")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime Fecha { get; set; }
        public string Uso { get; set; }
        public string TipoMovimiento{ get; set; }
        public string Motivo { get; set; }

    }
}
