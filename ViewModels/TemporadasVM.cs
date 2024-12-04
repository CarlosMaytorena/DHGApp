using AgricolaDH_GApp.Controllers.Admin;
using AgricolaDH_GApp.Models;

namespace AgricolaDH_GApp.ViewModels
{
    public class TemporadasVM
    {
        public List<Temporada> temporadaList { get; set; }
        public Temporada temporada { get; set; }

        public TemporadasVM()
        {
            temporadaList = new List<Temporada>();
            temporada = new Temporada();
        }        
    }
}
