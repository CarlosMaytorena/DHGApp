using AgricolaDH_GApp.Controllers.Admin;
using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.Services.Admin;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace AgricolaDH_GApp.Controllers
{
	public class HomeController : Controller
	{
        private readonly ILogger<HomeController> _logger;

        private readonly AppDbContext context;
        private UsuarioService usuarioService;
        private ViewRenderService renderService;

		public HomeController(ILogger<HomeController> logger, AppDbContext _ctx,
            UsuarioService _usuarioService, ViewRenderService _renderService)
		{
			_logger = logger;
            context = _ctx;
            usuarioService = _usuarioService;
            renderService = _renderService;
        }

		public IActionResult Index()
		{
			Login model = new Login();

            return View("~/Views/Login/Index.cshtml", model);

		}

        public IActionResult Login()
        {
            Login model = new Login();

            return View("~/Views/Login/Index.cshtml", model);

        }

        //     public IActionResult Login(Login model)
        //     {
        //Usuario user = new Usuario();
        //user = usuarioService.UsuarioLogin(model.Username, model.Password);

        //if (user == null) 
        //{
        //             return View("~/Views/Login/Index.cshtml", model);
        //         }

        //HttpContext.Session.SetString("s_SessionUser", JsonConvert.SerializeObject(user));

        //         return View("~/Views/Dashboard/Index.cshtml", user);
        //     }

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