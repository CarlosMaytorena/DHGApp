using AgricolaDH_GApp.Controllers.Admin;
using AgricolaDH_GApp.Models;

namespace AgricolaDH_GApp.ViewModels
{
    public class AlmacenVM
    {
        public List<Almacen> almacenList { get; set; }
        public Almacen almacen { get; set; }

        public AlmacenVM()
        {
            almacenList = new List<Almacen>();
            almacen = new Almacen();
        }        
    }
}
