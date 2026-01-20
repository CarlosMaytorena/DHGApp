using AgricolaDH_GApp.Controllers.Admin;
using AgricolaDH_GApp.Models;

namespace AgricolaDH_GApp.ViewModels
{
    public class EtapasVM
    {
        public List<Etapa> etapaList { get; set; }
        public Etapa etapa { get; set; }

        public List<Temporada> temporadaList { get; set; }
        public List<Area> areaList { get; set; }
        public List<Cultivo> cultivoList { get; set; }
        public List<Rancho> ranchoList { get; set; }

        public EtapasVM()
        {
            etapaList = new List<Etapa>();
            etapa = new Etapa();

            temporadaList = new List<Temporada>();
            areaList = new List<Area>();
            cultivoList = new List<Cultivo>();
            ranchoList = new List<Rancho>();
        }
    }
}
