using AgricolaDH_GApp.Controllers.Admin;
using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.ViewModels;

namespace AgricolaDH_GApp.Services.Admin
{
    public class UsuarioService
    {
        private readonly AppDbContext context;

        public UsuarioService(AppDbContext _ctx)
        {
            context = _ctx;
        }

        public List<Usuario> SelectUsuarios()
        {
            List<Usuario> usuarioList;

            try
            {
                usuarioList = context.Usuarios.ToList();
            }
            catch
            {
                usuarioList = new List<Usuario>();
            }

            return usuarioList;
        }

        public Usuario SelectUsuario(int IdUsuario)
        {
            Usuario usuario;

            try
            {
                usuario = context.Usuarios.Find(IdUsuario);
            }
            catch
            {
                usuario = new Usuario();
            }

            return usuario;
        }

        public int InsertUsuario(Usuario usuario)
        {
            int res = 0;

            try
            {
                context.Usuarios.Add(usuario);
                context.SaveChanges();
            }
            catch(Exception ex)
            {
                res = -1;
            }

            return res;

        }

        public int UpdateUsuario(Usuario usuario)
        {
            int res = 0;

            try
            {
                context.Usuarios.Update(usuario);
                context.SaveChanges();
            }
            catch(Exception ex) 
            { 
                res = -1;
            }

            return res;
        }

        public int DeleteUsuario(int IdUsuario)
        {
            int res = 0;

            try
            {
                Usuario usuario = context.Usuarios.Find(IdUsuario);

                context.Usuarios.Remove(usuario);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                res = -1;
            }

            return res;
        }
    }
}
