using Microsoft.EntityFrameworkCore;

namespace AgricolaDH_GApp.Models
{
    public class Etapa
    {
        public int IdEtapa { get; set; }
        public string Descripcion { get; set; }
        public int? IdTemporada { get; set; }
        public int? IdArea { get; set; }
        public int? IdCultivo { get; set; }
        public int? IdRancho { get; set; }

    }
}
