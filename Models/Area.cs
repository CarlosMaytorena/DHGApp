using Microsoft.EntityFrameworkCore;

namespace AgricolaDH_GApp.Models
{
    public class Area
    {
        public int IdArea { get; set; }
        public string Descripcion { get; set; }
        public string? Nomenclatura { get; set; }

    }
}
