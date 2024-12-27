using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AgricolaDH_GApp.Models
{
    public class Requisicion
    {
        public int IdRequisicion { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Date only")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Fecha { get; set; }
        public int IdSolicitante { get; set; }
        public int IdProveedor { get; set; }
        public int IdArea { get; set; }
        public int IdCultivo { get; set; }
        public int IdRancho { get; set; }
        public int IdEtapa { get; set; }
        public int IdTemporada { get; set; }
        public int IdProductoOrdenar { get; set; }

    }
}
