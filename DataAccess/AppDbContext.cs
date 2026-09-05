using AgricolaDH_GApp.Models;
using Microsoft.Ajax.Utilities;
using Microsoft.EntityFrameworkCore;

namespace AgricolaDH_GApp.DataAccess
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) :base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>().HasKey(m => new { m.IdUsuario });
            modelBuilder.Entity<UsuarioDropdown>().HasKey(m => new { m.IdUsuario });
            modelBuilder.Entity<Producto>().HasKey(m => new { m.IdProducto });
            modelBuilder.Entity<Proveedor>().HasKey(m => new { m.IdProveedor });           
            modelBuilder.Entity<Area>().HasKey(m => new { m.IdArea });
            modelBuilder.Entity<Rol>().HasKey(m => new { m.IdRol });
            modelBuilder.Entity<Cultivo>().HasKey(m => new { m.IdCultivo });
            modelBuilder.Entity<Rancho>().HasKey(m => new { m.IdRancho });
            modelBuilder.Entity<Etapa>().HasKey(m => new { m.IdEtapa });
            modelBuilder.Entity<Temporada>().HasKey(m => new { m.IdTemporada });
            modelBuilder.Entity<Constante>().HasKey(m => new { m.Descripcion });

            modelBuilder.Entity<OrdenDeCompraStatus>().HasKey(m => new { m.IdOrdenDeCompraStatus });
            modelBuilder.Entity<OrdenDeCompra>().HasKey(m => new { m.IdOrdenDeCompra});
            modelBuilder.Entity<ProductoOrdenar>().HasKey(m => new { m.IdProductoOrdenar});
            modelBuilder.Entity<ProductoOrdenarSelected>().HasKey(m => new { m.IdProductoOrdenar});
            modelBuilder.Entity<OrdenDeCompraTable>().HasKey(m => new { m.IdOrdenDeCompra});

            modelBuilder.Entity<Factura>().HasKey(m => new { m.IdFactura });
            modelBuilder.Entity<FacturaDetalle>().HasKey(m => new { m.IdFacturaDetalle });
            modelBuilder.Entity<FacturaHistorial>().HasKey(m => new { m.IdFactura });
            modelBuilder.Entity<ResumenFacturacionProducto>().HasKey(m => new { m.IdResumen });

            modelBuilder.Entity<Almacen>().HasKey(m => new { m.IdAlmacen });
            modelBuilder.Entity<Egreso>().HasKey(m => new { m.IdEgreso });
            modelBuilder.Entity<Estatus>().HasKey(m => new { m.IdEstatus });
            modelBuilder.Entity<Evidencia>().HasKey(m => new { m.IdEvidencia });
            modelBuilder.Entity<SerialMap>().HasKey(m => new { m.SerialKey });
            modelBuilder.Entity<LogsEgresos>().HasKey(m => new { m.IdLogsEgresos });
            modelBuilder.Entity<UltimoProductoID>().HasKey(m => new { m.ID });

            modelBuilder.Entity<LogsAlmacen>().HasKey(m => new { m.IdLogsAlmacen });
            modelBuilder.Entity<LogsAlmacenProductos>().HasKey(m => new { m.IdLogsAlmacenProducto });

            modelBuilder.Entity<Ingreso>().HasKey(m => new { m.IdIngreso });
            modelBuilder.Entity<Ingreso>().ToTable("Ingreso");
            modelBuilder.Entity<IngresoDetalle>().HasKey(m => new { m.IdIngresoDetalle });
        }
        public DbSet<OrdenDeCompraStatus> OrdenDeCompraStatus { get; set; }
        public DbSet<OrdenDeCompra> OrdenesDeCompra { get; set; }
        public DbSet<OrdenDeCompraTable> OrdenDeCompraTable { get; set; }
        public DbSet<ProductoOrdenar> ProductosOrdenar { get; set; }
        public DbSet<ProductoOrdenarSelected> ProductoOrdenarSelected { get; set; }
        public DbSet<Factura> Facturas { get; set; }
        public DbSet<FacturaDetalle> FacturaDetalle { get; set; }
        public DbSet<FacturaHistorial> FacturaHistorial { get; set; }
        public DbSet<ResumenFacturacionProducto> ResumenFacturacionProducto { get; set; }


        //Catalogos
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<UsuarioDropdown> UsuariosDropdown { get; set; }
        public DbSet<Cultivo> Cultivos { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Rancho> Ranchos { get; set; }
        public DbSet<Etapa> Etapas { get; set; }
        public DbSet<Temporada> Temporadas { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Constante> Constantes { get; set; }

        public DbSet<Almacen> Almacen { get; set; }
        public DbSet<Egreso> Egresos { get; set; }
        public DbSet<Estatus> Estatus { get; set; }
        public DbSet<Evidencia> Evidencia { get; set; }

        public DbSet<UltimoProductoID> UltimoProductoID { get; set; }

        public DbSet<SerialMap> SerialMap { get; set; }
        public DbSet<LogsAlmacen> LogsAlmacen { get; set; }
        public DbSet<LogsEgresos> LogsEgresos { get; set; }
        public DbSet<LogsAlmacenProductos> LogsAlmacenProductos { get; set; }

        public DbSet<Ingreso> Ingresos { get; set; }
        public DbSet<IngresoDetalle> IngresoDetalle { get; set; }

    }
}
