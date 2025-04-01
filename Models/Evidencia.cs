using AgricolaDH_GApp.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgricolaDH_GApp.Models
{
    public class Evidencia
    {
        public int IdEvidencia { get; set; }
        public string? PathAntes { get; set; }
        public string? PathDespues { get; set; }
    }
}
