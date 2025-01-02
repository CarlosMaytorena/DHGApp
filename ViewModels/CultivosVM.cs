using AgricolaDH_GApp.Controllers.Admin;
using AgricolaDH_GApp.Models;

namespace AgricolaDH_GApp.ViewModels
{
    public class CultivosVM
    {
        public List<Cultivo> cultivoList { get; set; }
        public Cultivo cultivo { get; set; }

        public CultivosVM()
        {
            cultivoList = new List<Cultivo>();
            cultivo = new Cultivo();
        }        
    }
}
