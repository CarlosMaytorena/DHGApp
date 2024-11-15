using Microsoft.EntityFrameworkCore;

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

    }
}
