using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AgricolaDH_GApp.Models
{
    public class Login
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }

}
