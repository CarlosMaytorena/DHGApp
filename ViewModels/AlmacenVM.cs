using AgricolaDH_GApp.Controllers.Admin;
using AgricolaDH_GApp.Models;

namespace AgricolaDH_GApp.ViewModels
{
    public class AlmacenVM
    {
        public List<Almacen> almacenLista { get; set; }
        public Almacen almacen { get; set; }



        public List<Usuario> usuariosList { get; set; }
        public Producto producto { get; set; }

        public AlmacenVM()
        {
            almacenLista = new List<Almacen>();
            almacen = new Almacen();

            usuariosList = new List<Usuario>();

            producto = new Producto();
        }        
    }
}
