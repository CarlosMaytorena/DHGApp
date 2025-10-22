using Microsoft.AspNetCore.Mvc;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Services.Admin;
using Microsoft.Ajax.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using AgricolaDH_GApp.ViewModels;
using System.Text.Json;
using AgricolaDH_GApp.Helper;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;



namespace AgricolaDH_GApp.Controllers
{
    public class LoginController : Controller
    {

        private readonly ILogger<LoginController> _logger;
        private readonly AppDbContext _context;
        private UsuarioService usuarioService;
        private readonly OrdenDeCompraService _ordenDeCompraService;
        private ConstanteService constanteService;
        private Email email;

        public LoginController(
            ILogger<LoginController> logger,
            AppDbContext _ctx,
            UsuarioService _usuarioService,
            OrdenDeCompraService ordenDeCompraService,
            ConstanteService _constanteService,
            Email _email)
        {
            _logger = logger;
            _context = _ctx;
            usuarioService = _usuarioService;
            _ordenDeCompraService = ordenDeCompraService;
            constanteService = _constanteService;
            email = _email;

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
                bool isValid = _context.Usuarios
                    .Any(u => u.Username == model.Username && u.Password == model.Password);

                if (isValid)
                {
                    Usuario user = usuarioService.UsuarioLogin(model.Username, model.Password);

                    // Crear claims para autenticación
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("UserId", user.IdUsuario.ToString()),
                new Claim(ClaimTypes.Role, user.IdRol.ToString())
            };

                    var identity = new ClaimsIdentity(claims, "CookieAuth");
                    var principal = new ClaimsPrincipal(identity);

                    HttpContext.SignInAsync("CookieAuth", principal);

                    // Guardar datos mínimos en sesión
                    HttpContext.Session.SetString("Correo", user.Correo ?? "");
                    HttpContext.Session.SetInt32("IdUsuario", user.IdUsuario);
                    HttpContext.Session.SetInt32("IdRol", user.IdRol);

                    // ✅ En lugar de renderizar la vista, redirigimos (PRG pattern)
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["LoginError"] = "Usuario o contraseña incorrectos.";
                    return View("Index");
                }
            }

            // Si el modelo no es válido, regresar al login
            return View("Index", model);
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Login", "Home");  // This sends the user to '/'
        }

        [HttpPost]
        public ActionResult EnviarLinkRecuperacion(string correo)
        {
            // Solo imprimir el correo recibido en consola
            System.Diagnostics.Debug.WriteLine("Correo recibido: " + correo);

            // Puedes usar esto para dar feedback en pantalla si quieres
            TempData["LoginError"] = "Correo recibido, revisa la consola del servidor.";

            return View("Login");
        }

        [HttpPost]
        public JsonResult VerificarCorreo([FromBody] JsonElement data)
        {
            string correo = data.GetProperty("correo").GetString();
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Correo == correo);

            if (usuario != null)
            {
                // Generar código de 4 dígitos (ej. "0042", "9999")
                Random random = new Random();
                int numero = random.Next(1, 10000);
                string codigo = numero.ToString("D4");

                // Guardar en la base de datos
                usuario.ResetPasswordToken = codigo;
                _context.SaveChanges();

                // Enviar correo
                string sendgridKey = constanteService.SelectConstante("SendgridKey").Valor;
                string defaultEmailFrom = constanteService.SelectConstante("DefaultEmailFrom").Valor;
                email.SendForgotPasswordMail(sendgridKey, defaultEmailFrom, correo, codigo);


                return Json(new { existe = true, codigo });
            }

            return Json(new { existe = false });
        }

        [HttpPost]
        public JsonResult CambiarPassword([FromBody] JsonElement data)
        {
            try
            {
                string correo = data.GetProperty("correo").GetString();
                string nuevaPassword = data.GetProperty("nuevaPassword").GetString();

                var usuario = _context.Usuarios.FirstOrDefault(u => u.Correo == correo);

                if (usuario == null)
                {
                    return Json(new { success = false, message = "Usuario no encontrado" });
                }

                // Aquí podrías hashear la contraseña si tu app lo requiere.
                usuario.Password = nuevaPassword;
                usuario.ResetPasswordToken = null;

                _context.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


    }
}

