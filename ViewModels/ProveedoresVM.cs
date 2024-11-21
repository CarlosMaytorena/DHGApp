using AgricolaDH_GApp.Controllers.Admin;
using AgricolaDH_GApp.Models;

namespace AgricolaDH_GApp.ViewModels
{
    public class ProveedoresVM
    {
        public List<Proveedor> proveedorList { get; set; }
        public Proveedor proveedor { get; set; }

        public ProveedoresVM()
        {
            proveedorList = new List<Proveedor>();
            proveedor = new Proveedor();
        }        
    }
}
