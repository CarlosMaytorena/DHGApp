using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AgricolaDH_GApp.Controllers.Admin
{
    public class UsuariosController : Controller
    {
        private readonly ILogger<UsuariosController> _logger;

        private readonly AppDbContext context;

        public UsuariosController(ILogger<UsuariosController> logger, AppDbContext _ctx)
        {
            _logger = logger;
            context = _ctx;
        }

        [HttpGet]
        public IActionResult Index()
        {

            UsuariosVM model = new UsuariosVM();

            model.usuarioList = context.Usuarios.ToList();

            return PartialView("~/Views/Admin/Usuarios/Index.cshtml", model);
        }

        [HttpPost]
        public IActionResult CrearUsuario()
        {
            UsuariosVM model = new UsuariosVM();

            return PartialView("~/Views/Admin/Usuarios/Usuario.cshtml", model);
        }

        [HttpPost]
        public IActionResult EditarUsuario(int IdUsuario)
        {
            UsuariosVM model = new UsuariosVM();

            model.usuario = context.Usuarios.Find(IdUsuario);

            return PartialView("~/Views/Admin/Usuarios/Usuario.cshtml", model);
        }

        [HttpPost]
        public IActionResult InsertUsuario(UsuariosVM model)
        {
            context.Usuarios.Add(model.usuario);
            context.SaveChanges();

            model = new UsuariosVM();
            model.usuarioList = context.Usuarios.ToList();

            return PartialView("~/Views/Admin/Usuarios/Index.cshtml", model);
        }

        [HttpPost]
        public IActionResult UpdateUsuario(UsuariosVM model)
        {
            context.Usuarios.Update(model.usuario);
            context.SaveChanges();

            model = new UsuariosVM();
            model.usuarioList = context.Usuarios.ToList();

            return PartialView("~/Views/Admin/Usuarios/Index.cshtml", model);
        }

        [HttpPost]
        public IActionResult BorrarUsuario(int IdUsuario)
        {

            UsuariosVM model = new UsuariosVM();
            model.usuario = context.Usuarios.Find(IdUsuario);

            context.Usuarios.Remove(model.usuario);
            context.SaveChanges();

            model = new UsuariosVM();
            model.usuarioList = context.Usuarios.ToList();

            return PartialView("~/Views/Admin/Usuarios/Index.cshtml", model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}