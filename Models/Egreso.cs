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
        public string? Producto { get; set; }
        public int Cantidad { get; set; }
        public string? AprobadoPor { get; set; }
        public string? Solicitante { get; set; }
        public string? Comentarios { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Date only")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime Fecha { get; set; }
        public string? EvidenciaAntesPath { get; set; }
        public string? EvidenciaDespuesPath { get; set; }

        [NotMapped]
        public IFormFile EvidenciaAntesFile { get; set; }
        
        [NotMapped]
        public IFormFile EvidenciaDespuesFile { get; set; }
    }
    public class EgresoDTO
    {
        public Egreso Egreso { get; set; }

        public Producto Producto{ get; set; }

    }
}
