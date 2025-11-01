using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgricolaDH_GApp.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string? SegundoNombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string? ApellidoMaterno { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Correo { get; set; }
        public int IdRol { get; set; }
        public string? ResetPasswordToken { get; set; }


        [NotMapped]
        public string RolDescripcion { get; set; }

        [NotMapped]
        public string NombreCompleto
        {
            get
            {
                return $"{Nombre} {ApellidoPaterno}";
            }
        }


    }

    public class UsuarioDropdown
    {
        public int IdUsuario { get; set; }
        public string NombreCompleto { get; set; }

    }
}
