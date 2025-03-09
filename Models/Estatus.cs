using AgricolaDH_GApp.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgricolaDH_GApp.Models
{
    public class Estatus
    {
        public int IdEstatus { get; set; }
        public string? NombreEstatus { get; set; }
    } 
}
