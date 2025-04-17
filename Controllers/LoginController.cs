using Microsoft.AspNetCore.Mvc;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Services.Admin;
using Microsoft.Ajax.Utilities;
using Microsoft.EntityFrameworkCore;

namespace AgricolaDH_GApp.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private readonly AppDbContext _context;
        private UsuarioService usuarioService;


        public LoginController(ILogger<LoginController> logger, AppDbContext _ctx, UsuarioService _usuarioService)
        {
            _logger = logger;
            _context = _ctx;
            usuarioService = _usuarioService;
        }

        // Show the login form (GET)
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // Handle form submission (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(Login model)
        {
            if (ModelState.IsValid)
            {
                // TODO: Add login logic here (e.g., check credentials)

                // Redirect to the home page after successful login
                return RedirectToAction("Index", "Dashboard");  // This sends the user to '/'
            }

            // If model is invalid, re-show the form with validation messages
            return RedirectToAction("Index", "Home");  // This sends the user to '/'
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Authenticate(Login model)
        {
            if (ModelState.IsValid)
            {
                // Use the model.Username and model.Password values from the form
                bool isValid = _context.Usuarios
                    .Any(u => u.Username == model.Username && u.Password == model.Password);

                if (isValid)
                {
                    // TODO: Set session or auth cookie
                    Usuario user = new Usuario();
                    user = usuarioService.UsuarioLogin(model.Username, model.Password);

                    return View("~/Views/Dashboard/Index.cshtml", user);
                }
                else
                {
                    TempData["LoginError"] = "Invalid login.";
                    return RedirectToAction("Index", "Home");  // This sends the user to '/'
                }
            }

            // If we got this far, something failed; redisplay form
            return RedirectToAction("Index", "Home");  // This sends the user to '/'
        }


    }
}

