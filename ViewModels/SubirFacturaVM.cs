using AgricolaDH_GApp.Controllers.Admin;
using AgricolaDH_GApp.Models;

namespace AgricolaDH_GApp.ViewModels
{
    public class SubirFacturaVM
    {
        public List<OrdenDeCompraTable> subirFacturaList { get; set; }
        public OrdenDeCompraTable ordenDeCompra { get; set; }
        public List<ProductoOrdenarSelected> productosOrdenar { get; set; }
        public List<OrdenDeCompraTable> ordenesCerradas { get; set; }  // Closed (status 5)



        public SubirFacturaVM()
        {
            subirFacturaList = new List<OrdenDeCompraTable>();
            ordenDeCompra = new OrdenDeCompraTable();
            productosOrdenar = new List<ProductoOrdenarSelected>();
        }        
    }
}
