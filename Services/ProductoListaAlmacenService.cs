using AgricolaDH_GApp.Controllers.Admin;
using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.ViewModels;
using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;

namespace AgricolaDH_GApp.Services
{
    public class ProductoListaAlmacenService
    {
        private readonly AppDbContext context;

        public ProductoListaAlmacenService(AppDbContext _ctx)
        {
            context = _ctx;
        }
    }
}
