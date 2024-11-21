using AgricolaDH_GApp.Controllers.Admin;
using AgricolaDH_GApp.Models;

namespace AgricolaDH_GApp.ViewModels
{
    public class UsuariosVM
    {
        public List<Usuario> usuarioList { get; set; }
        public Usuario usuario { get; set; }

        public List<Rol> rolesList { get; set; }

        public UsuariosVM()
        {
            usuarioList = new List<Usuario>();
            usuario = new Usuario();
        }        
    }
}
