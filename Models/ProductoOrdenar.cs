using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgricolaDH_GApp.Models
{
    public class ProductoOrdenar
    {
        public int IdProductoOrdenar { get; set; }
        public int IdOrdenDeCompra { get; set; }
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal? Unidad { get; set; }
        public decimal? Total { get; set; }
        public decimal? Impuesto { get; set; }
        public int PorRecibir { get; set; }
        public bool? MonedaNacional { get; set; }
    }

    public class ProductoOrdenarSelected
    {
        public int IdProductoOrdenar { get; set; }
        public int IdOrdenDeCompra { get; set; }
        public string Producto { get; set; }
        public int Cantidad { get; set; }
        public decimal? Unidad { get; set; }
        public decimal? Total { get; set; }
        public decimal? Impuesto { get; set; }
        public decimal? Descuento { get; set; }
        public int PorRecibir { get; set; }
        public int ClaveProveedor { get; set; }
        public decimal Contenido { get; set; }
        public bool CalculoAlterno { get; set; }
        public string NombreInterno {  get; set; }
        public string Compania { get; set; }
        public string UnidadDeSKU { get; set; }
        public bool? MonedaNacional { get; set; }

        [NotMapped]
        public string NombreDropdown => $"{NombreInterno} - {Compania} ({Contenido} {UnidadDeSKU})";

        // Suma de IngresoDetalle.CantidadRecibida para esta línea, calculada en el controlador.
        // No representa "lo ordenado" (eso sigue siendo Cantidad) sino lo realmente recibido a la fecha.
        [NotMapped]
        public int RecibidoAcumulado { get; set; }

        // Valor que el usuario ya había escrito en "Recibir" antes de agregar otro producto
        // del catálogo; se reaplica al recargar el formulario para no perderlo.
        [NotMapped]
        public int CantidadCapturada { get; set; }

        // Precio unitario de la factura más reciente subida para esta línea (si alguna vez
        // se subió una). Null si el producto nunca se ha facturado.
        [NotMapped]
        public decimal? PrecioUnitarioFactura { get; set; }

    }

    public class ProductoRecibidoDTO
    {
        public int IdProductoOrdenar { get; set; }
        public int Recibida { get; set; }
        public List<string> Seriales { get; set; }
        public List<string>? SerialesCortos { get; set; } // NEW: the short serials to persist

    }

    public class CapturaPreviaDTO
    {
        public int IdProductoOrdenar { get; set; }
        public int Cantidad { get; set; }
    }

    public class AgregarProductoCatalogoRequest
    {
        public int IdOrdenDeCompra { get; set; }
        public int IdProducto { get; set; }
        public List<CapturaPreviaDTO>? CapturaPrevia { get; set; }
    }

    public class EliminarProductoCatalogoRequest
    {
        public int IdOrdenDeCompra { get; set; }
        public int IdProductoOrdenar { get; set; }
        public List<CapturaPreviaDTO>? CapturaPrevia { get; set; }
    }
}
