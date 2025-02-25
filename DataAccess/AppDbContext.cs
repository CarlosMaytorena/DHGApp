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

            modelBuilder.Entity<OrdenDeCompra>().HasKey(m => new { m.IdOrdenDeCompra});
            modelBuilder.Entity<ProductoOrdenar>().HasKey(m => new { m.IdProductoOrdenar});
            modelBuilder.Entity<ProductoOrdenarSelected>().HasKey(m => new { m.IdProductoOrdenar});
            modelBuilder.Entity<OrdenDeCompraTable>().HasKey(m => new { m.IdOrdenDeCompra});

        }

        public DbSet<OrdenDeCompra> OrdenesDeCompra { get; set; }
        public DbSet<OrdenDeCompraTable> OrdenDeCompraTable { get; set; }
        public DbSet<ProductoOrdenar> ProductosOrdenar { get; set; }
        public DbSet<ProductoOrdenarSelected> ProductoOrdenarSelected { get; set; }

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

    }
}
