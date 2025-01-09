using AgricolaDH_GApp.Controllers.Admin;
using AgricolaDH_GApp.Models;

namespace AgricolaDH_GApp.ViewModels
{
    public class AlmacenVM
    {
        public List<AlmacenView> almacenList { get; set; }
        public Almacen almacen { get; set; }
        public AlmacenView almacenView { get; set; }
        public List<Producto> productoList { get; set; }


        public AlmacenVM()
        {
            almacenList = new List<AlmacenView>();
            almacenView = new AlmacenView();
            almacen = new Almacen();

            //Dropdown list
            productoList = new List<Producto>();


        }        
    }
}
