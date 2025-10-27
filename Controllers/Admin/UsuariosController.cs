using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.Services.Admin;
using AgricolaDH_GApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace AgricolaDH_GApp.Controllers.Admin
{
    public class UsuariosController : Controller
    {
        private readonly ILogger<UsuariosController> _logger;

        private readonly AppDbContext context;
        private UsuarioService usuarioService;
        private RolService rolService;
        private ViewRenderService renderService;


        public UsuariosController(ILogger<UsuariosController> logger, AppDbContext _ctx,
            UsuarioService _usuarioService, RolService _rolService, ViewRenderService _renderService)
        {
            _logger = logger;
            context = _ctx;
            usuarioService = _usuarioService;
            rolService = _rolService;
            renderService = _renderService;
        }

        [HttpGet]
        public IActionResult Index()
        {

            UsuariosVM model = new UsuariosVM();
            model.usuarioList = usuarioService.SelectUsuarios();
            model.usuarioList = GetRolDescripcion(model.usuarioList);

            return PartialView("~/Views/Admin/Usuarios/Index.cshtml", model);
        }

        [HttpPost]
        public IActionResult CrearUsuario()
        {
            UsuariosVM model = new UsuariosVM();
            model.rolesList = rolService.SelectRoles();

            return PartialView("~/Views/Admin/Usuarios/Usuario.cshtml", model);
        }

        [HttpPost]
        public IActionResult EditarUsuario(int IdUsuario)
        {
            UsuariosVM model = new UsuariosVM();

            model.usuario = usuarioService.SelectUsuario(IdUsuario);
            model.rolesList = rolService.SelectRoles();

            return PartialView("~/Views/Admin/Usuarios/Usuario.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> InsertUsuario(UsuariosVM model)
        {
            _logger.LogInformation("Hola InsertarUsuario");

            int res = usuarioService.InsertUsuario(model.usuario);
            // Access to Active Directory
            //res += await usuarioService.AddToActiveDirectory(model.usuario);
            model = new UsuariosVM();
            model.usuarioList = usuarioService.SelectUsuarios();
            model.usuarioList = GetRolDescripcion(model.usuarioList);

            return Json(new { res, url = await renderService.RenderViewToStringAsync("~/Views/Admin/Usuarios/Index.cshtml", model) });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUsuario(UsuariosVM model)
        {
            Usuario old = context.Usuarios.AsNoTracking().FirstOrDefault(u => u.IdUsuario == model.usuario.IdUsuario);
            // Access to Active Directory
            //int res = await usuarioService.UpdateActiveDirectory(old, model.usuario);
            int res = usuarioService.UpdateUsuario(model.usuario);
            model = new UsuariosVM();
            model.usuarioList = usuarioService.SelectUsuarios();
            model.usuarioList = GetRolDescripcion(model.usuarioList);

            return Json(new { res, url = await renderService.RenderViewToStringAsync("~/Views/Admin/Usuarios/Index.cshtml", model) });
        }


        [HttpPost]
        public async Task<IActionResult> BorrarUsuario(int IdUsuario)
        {
            _logger.LogInformation("Hola Borrar Usuario");
            // Access to Active Directory
            //int res = await usuarioService.DropActiveDirectory(u);
            //res += usuarioService.DeleteUsuario(IdUsuario);
            Usuario u = context.Usuarios.AsNoTracking().FirstOrDefault(a => a.IdUsuario == IdUsuario);
            int res = 0;
            UsuariosVM model = new UsuariosVM();
            model.usuarioList = usuarioService.SelectUsuarios();
            model.usuarioList = GetRolDescripcion(model.usuarioList);

            return Json(new {res, url = await renderService.RenderViewToStringAsync("~/Views/Admin/Usuarios/Index.cshtml", model) });
        }

        public List<Usuario> GetRolDescripcion(List<Usuario> usuarioList)
        {
            foreach (var usuario in usuarioList)
            {
                usuario.RolDescripcion = rolService.SelectRol(usuario.IdRol).Descripcion;
            }

            return usuarioList;
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