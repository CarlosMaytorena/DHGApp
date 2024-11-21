using Microsoft.EntityFrameworkCore;

namespace AgricolaDH_GApp.Models
{
    public class Proveedor
    {
        public int IdProveedor { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }

    }
}
