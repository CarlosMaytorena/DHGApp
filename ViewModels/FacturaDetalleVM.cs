namespace AgricolaDH_GApp.ViewModels
{
    public class FacturaDetalleVM
    {
        public int? IdProductoOrdenar { get; set; }
        public string Producto { get; set; }
        public bool EsExtra { get; set; }
        public int CantidadFacturada { get; set; }
        public decimal? PrecioUnitario { get; set; }
        public decimal? PrecioTotal { get; set; }
    }
}
