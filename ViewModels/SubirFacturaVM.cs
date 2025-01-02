using AgricolaDH_GApp.Controllers.Admin;
using AgricolaDH_GApp.Models;

namespace AgricolaDH_GApp.ViewModels
{
    public class SubirFacturaVM
    {
        public List<OrdenDeCompraTable> subirFacturaList { get; set; }
        public OrdenDeCompraTable ordenDeCompra { get; set; }
        public List<ProductoOrdenarSelected> productosOrdenar { get; set; }

        //Dropdowns
        public List<Usuario> solicitanteList { get; set; }
        public List<Proveedor> proveedorList { get; set; }
        public List<Area> areaList { get; set; }
        public List<Cultivo> cultivoList { get; set; }
        public List<Rancho> ranchoList { get; set; }
        public List<Etapa> etapaList { get; set; }
        public List<Temporada> temporadaList { get; set; }
        public List<Producto> productoList { get; set; }

        public SubirFacturaVM()
        {
            ordenDeCompra = new OrdenDeCompraTable();

            //Dropdowns
            solicitanteList = new List<Usuario>();
            proveedorList = new List<Proveedor>();
            areaList = new List<Area>();
            cultivoList = new List<Cultivo>();
            ranchoList = new List<Rancho>();
            etapaList = new List<Etapa>();
            temporadaList = new List<Temporada>();
            productoList = new List<Producto>();
        }        
    }
}
