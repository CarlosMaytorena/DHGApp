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
            modelBuilder.Entity<Producto>().HasKey(m => new { m.IdProducto });
            modelBuilder.Entity<Proveedor>().HasKey(m => new { m.IdProveedor });
            modelBuilder.Entity<Rol>().HasKey(m => new { m.IdRol });

        }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }

        public DbSet<Rol> Roles { get; set; }

    }
}
