using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgricolaDH_GApp.Models
{
    public class LogsAlmacen
    {
        public int? IdLogsAlmacen { get; set; }
        public int? IdSolicitante { get; set; }
        public int? IdAlmacenista { get; set; }
        public int? IdMovimiento { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Date only")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime Fecha { get; set; }
    }
}
