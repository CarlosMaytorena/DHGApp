using AgricolaDH_GApp.Controllers.Admin;
using AgricolaDH_GApp.Models;

namespace AgricolaDH_GApp.ViewModels
{
    public class EtapasVM
    {
        public List<Etapa> etapaList { get; set; }
        public Etapa etapa { get; set; }

        public EtapasVM()
        {
            etapaList = new List<Etapa>();
            etapa = new Etapa();
        }        
    }
}
