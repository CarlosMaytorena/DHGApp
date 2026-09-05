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

        public List<FacturaHistorial> historialFacturas { get; set; }
        public List<ResumenFacturacionProducto> resumenFacturacion { get; set; }
        public List<FacturaDetalleVM> facturaDetallePreview { get; set; }
        public bool? monedaNacionalPreview { get; set; }

        public SubirFacturaVM()
        {
            subirFacturaList = new List<OrdenDeCompraTable>();
            ordenDeCompra = new OrdenDeCompraTable();
            productosOrdenar = new List<ProductoOrdenarSelected>();
            ordenesCerradas = new List<OrdenDeCompraTable>();
            historialFacturas = new List<FacturaHistorial>();
            resumenFacturacion = new List<ResumenFacturacionProducto>();
            facturaDetallePreview = new List<FacturaDetalleVM>();
        }
    }
}
