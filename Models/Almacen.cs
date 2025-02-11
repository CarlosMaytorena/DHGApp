﻿using AgricolaDH_GApp.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace AgricolaDH_GApp.Models
{
    /// <summary>
    /// Estructura para mapear en db
    /// </summary>
    public class Almacen
    {
        public int IdAlmacen { get; set; }
        public int IdProducto { get; set; }
        public int Disponible { get; set; }
        public int EnUso { get; set; }
        public int Terminados { get; set; }


    }
    /// <summary>
    /// Estructura para mostrar en vista
    /// </summary>
    public class AlmacenView
    {
        public int IdAlmacen { get; set; }
        public int IdProducto { get; set; }
        public string? NombreProducto { get; set; }
        public string? Descripcion { get; set; }
        public int Disponible { get; set; }
        public int EnUso { get; set; }
        public int Terminados { get; set; }
        public string? ProductBarcodeID { get; set; }
    }

    public class AlmacenDTO
    {
        public Almacen Almacen { get; set; }
        public int Motivo { get; set; }
        public Movimiento Movimiento { get; set; }
        public Producto Producto { get; set; }
    }
}
