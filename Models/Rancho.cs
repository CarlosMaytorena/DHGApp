using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgricolaDH_GApp.Models
{
    public class Rancho
    {
        public int IdRancho { get; set; }
        public string Descripcion { get; set; }
        public int IdArea { get; set; }

        [NotMapped]
        public string? Area { get; set; }

    }
}
