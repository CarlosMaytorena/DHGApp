using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.Services.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AgricolaDH_GApp.Controllers
{
    [Authorize(AuthenticationSchemes = "CookieAuth")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        private readonly UsuarioService _usuarioService;

        public HomeController(ILogger<HomeController> logger, AppDbContext context, UsuarioService usuarioService)
        {
            _logger = logger;
            _context = context;
            _usuarioService = usuarioService;
        }

        public IActionResult Index()
        {
            // 🔹 Recuperar el ID del usuario desde la sesión
            var userId = HttpContext.Session.GetInt32("IdUsuario");

            if (userId == null)
            {
                // Si por alguna razón no hay sesión, redirigir al login
                return RedirectToAction("Index", "Login");
            }

            // 🔹 Obtener el usuario desde la base de datos
            var user = _usuarioService.GetUsuarioById(userId.Value);

            if (user == null)
            {
                // Si el usuario no existe o hubo error, redirigir al login
                return RedirectToAction("Index", "Login");
            }

            // 🔹 Pasar el modelo Usuario a la vista
            return View("~/Views/Home/Index.cshtml", user);
        }

        public IActionResult Login()
        {
            Login model = new Login();

            return View("~/Views/Login/Index.cshtml", model);

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
