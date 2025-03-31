using AgricolaDH_GApp.Controllers.Admin;
using AgricolaDH_GApp.Models;

namespace AgricolaDH_GApp.ViewModels
{
    public class EgresosVM
    {
        public List<Egreso> egresosLista { get; set; }
        public Egreso egreso { get; set; }
        public Almacen almacen { get; set; }
        public List<Almacen> almacenLista { get; set; }


        public List<Producto> productosList { get; set; }
        public List<Usuario> usuariosList { get; set; }
        public Producto producto { get; set; }

        public EgresosVM()
        {
            egresosLista = new List<Egreso>();
            egreso = new Egreso();
            productosList = new List<Producto>();
            usuariosList = new List<Usuario>();
            almacenLista = new List<Almacen>();
            producto = new Producto();

        }        
    }
}
