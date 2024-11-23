using AgricolaDH_GApp.Controllers.Admin;
using AgricolaDH_GApp.Models;

namespace AgricolaDH_GApp.ViewModels
{
    public class AreasVM
    {
        public List<Area> areaList { get; set; }
        public Area area { get; set; }

        public AreasVM()
        {
            areaList = new List<Area>();
            area = new Area();
        }        
    }
}
