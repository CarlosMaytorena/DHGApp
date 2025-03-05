using AgricolaDH_GApp.Controllers.Admin;
using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.ViewModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace AgricolaDH_GApp.Services.Admin
{
    public class UsuarioService
    {
        private readonly AppDbContext context;

        public UsuarioService(AppDbContext _ctx)
        {
            context = _ctx;
        }

        public Usuario UsuarioLogin(string username, string password)
        {
            Usuario usuario;

            try
            {
                usuario = context.Usuarios.FromSqlRaw("EXEC SP_SelectUsuarioLogin @Username, @Password",
                    new SqlParameter("@Username", username),
                    new SqlParameter("@Password", password)).ToList().FirstOrDefault();

            }
            catch
            {
                usuario = new Usuario();
            }

            return usuario;
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

        public List<UsuarioDropdown> SelectUsuariosByIdRol(int IdRol)
        {

            List<UsuarioDropdown> usuarioDropdown;

            try
            {
                usuarioDropdown = context.UsuariosDropdown.FromSqlRaw("exec SP_SelectIngenierosDropdown @IdRol",
                    new SqlParameter("@IdRol", IdRol)).ToList();
            }
            catch
            {
                usuarioDropdown = new List<UsuarioDropdown>();
            }

            return usuarioDropdown;
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
