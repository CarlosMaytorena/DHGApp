namespace AgricolaDH_GApp.Models
{
    public class Factura
    {
        public int IdFactura { get; set; }
        public int IdOrdenDeCompra { get; set; }
        public bool? MonedaNacional { get; set; }
        public DateTime FechaCarga { get; set; }
        public int IdUsuarioCarga { get; set; }
    }

    public class FacturaDetalle
    {
        public int IdFacturaDetalle { get; set; }
        public int IdFactura { get; set; }
        public int? IdProductoOrdenar { get; set; }
        public string? DescripcionExtra { get; set; }
        public int CantidadFacturada { get; set; }
        public decimal? PrecioUnitario { get; set; }
        public decimal? PrecioTotal { get; set; }
    }

    public class FacturaHistorial
    {
        public int IdFactura { get; set; }
        public int IdOrdenDeCompra { get; set; }
        public bool? MonedaNacional { get; set; }
        public DateTime FechaCarga { get; set; }
        public string UsuarioCarga { get; set; }
    }

    public class ResumenFacturacionProducto
    {
        public int IdResumen { get; set; }
        public int? IdProductoOrdenar { get; set; }
        public string Producto { get; set; }
        public int? Cantidad { get; set; }
        public int? PorRecibir { get; set; }
        public int? CantidadIngresada { get; set; }
        public int CantidadFacturada { get; set; }
        public decimal? PrecioUnitario { get; set; }
        public bool? MonedaNacional { get; set; }
        public decimal? CostoTotal { get; set; }
        public bool EsExtra { get; set; }
    }
}
