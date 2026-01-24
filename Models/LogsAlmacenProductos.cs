using AgricolaDH_GApp.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgricolaDH_GApp.Models
{
    public class LogsAlmacenProductos
    {
        public int IdLogsAlmacenProducto { get; set; }
        public int? IdLogsAlmacen { get; set; }
        public int IdProducto { get; set; }
        public string? SerialKey { get; set; }
    }
}
