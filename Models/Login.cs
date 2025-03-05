using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AgricolaDH_GApp.Models
{
    public class Login
    {
        [Required]
        [Display(Name = "Usuario")]
        public string Username { get; set; }
        [Required]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }
        public bool HasError { get; set; }

    }
}
