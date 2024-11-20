using AgricolaDH_GApp.Controllers.Admin;
using AgricolaDH_GApp.Models;

namespace AgricolaDH_GApp.ViewModels
{
    public class ProductosVM
    {
        public List<Producto> productoList { get; set; }
        public Producto producto { get; set; }

        public ProductosVM()
        {
            productoList = new List<Producto>();
            producto = new Producto();
        }        
    }
}
