using AgricolaDH_GApp.Controllers.Admin;
using AgricolaDH_GApp.Models;

namespace AgricolaDH_GApp.ViewModels
{
    public class AlmacenVM
    {
        public List<Almacen> almacenLista { get; set; }


        public List<AlmacenView> almacenList { get; set; }
        public Almacen almacen { get; set; }
        public AlmacenView almacenView { get; set; }
        public List<Movimiento> movimientosList { get; set; }
        public Movimiento movimiento { get; set; }
        public List<Usuario> usuariosList { get; set; }
        public Producto producto { get; set; }

        public AlmacenVM()
        {
            almacenLista = new List<Almacen>();


            almacenList = new List<AlmacenView>();
            almacenView = new AlmacenView();
            almacen = new Almacen();

            movimiento = new Movimiento();
            movimientosList = new List<Movimiento>();

            usuariosList = new List<Usuario>();

            producto = new Producto();
        }        
    }
}
