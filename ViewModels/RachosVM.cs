using AgricolaDH_GApp.Controllers.Admin;
using AgricolaDH_GApp.Models;

namespace AgricolaDH_GApp.ViewModels
{
    public class RanchosVM
    {
        public List<Rancho> ranchoList { get; set; }
        public Rancho rancho { get; set; }
        public List<Area> areaList { get; set; }

        public RanchosVM()
        {
            ranchoList = new List<Rancho>();
            areaList = new List<Area>();
            rancho = new Rancho();
        }        
    }
}
