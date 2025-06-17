using Microsoft.AspNetCore.Mvc;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Services.Admin;
using Microsoft.Ajax.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using AgricolaDH_GApp.ViewModels;

namespace AgricolaDH_GApp.Controllers
{
    public class LoginController : Controller
    {

        private readonly ILogger<LoginController> _logger;
        private readonly AppDbContext _context;
        private UsuarioService usuarioService;
        private readonly OrdenDeCompraService _ordenDeCompraService;




        public LoginController(
            ILogger<LoginController> logger,
            AppDbContext _ctx,
            UsuarioService _usuarioService,
            OrdenDeCompraService ordenDeCompraService)
        {
            _logger = logger;
            _context = _ctx;
            usuarioService = _usuarioService;
            _ordenDeCompraService = ordenDeCompraService;

        }

        // Show the login form (GET)
        [HttpGet]
        public IActionResult Index()
        {
            return View();
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

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim("UserId", user.IdUsuario.ToString()),
                        new Claim(ClaimTypes.Role, user.IdRol.ToString())
                    };

                    var identity = new ClaimsIdentity(claims, "CookieAuth");
                    var principal = new ClaimsPrincipal(identity);

                    HttpContext.SignInAsync("CookieAuth", principal);
                    return View("~/Views/Home/Index.cshtml", user);

                    //return View("~/Views/Home/Index.cshtml", user);
                }
                else
                {
                    TempData["LoginError"] = "Invalid login.";
                    return View("Index");
                }
            }

            // If we got this far, something failed; redisplay form
            Login loginModel = new Login();

            return View("~/Views/Login/Index.cshtml", loginModel);
            //return RedirectToAction("Login", "Home");  // This sends the user to '/'
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Login", "Home");  // This sends the user to '/'
        }



    }
}

