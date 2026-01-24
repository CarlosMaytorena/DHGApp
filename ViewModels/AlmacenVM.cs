using AgricolaDH_GApp.Controllers.Admin;
using AgricolaDH_GApp.Models;

namespace AgricolaDH_GApp.ViewModels
{
    public class AlmacenVM
    {
        public List<Almacen> almacenLista { get; set; }
        public Almacen almacen { get; set; }

        public List<Usuario> usuariosList { get; set; }
        public List<UsuarioDropdown> ingenieroList { get; set; }
        public Producto producto { get; set; }
        public string sourceView { get; set; }
        public LogsAlmacen logsAlmacen { get; set; }
        public LogsAlmacenProductos logsAlmacenProductos { get; set; }


        public AlmacenVM()
        {
            almacenLista = new List<Almacen>();
            almacen = new Almacen();
            usuariosList = new List<Usuario>();
            ingenieroList = new List<UsuarioDropdown>();
            producto = new Producto();
            logsAlmacen = new LogsAlmacen();
            sourceView = string.Empty;
            logsAlmacenProductos = new LogsAlmacenProductos();
        }        
    }
}
