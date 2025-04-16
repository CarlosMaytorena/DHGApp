using AgricolaDH_GApp.Models;
using AgricolaDH_GApp.Services.Admin;
using AgricolaDH_GApp.ViewModels;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AgricolaDH_GApp.Controllers
{

    public class ReimpresionController : Controller
    {
        private readonly AlmacenService _almacenService;


        public ReimpresionController(AlmacenService almacenService)
        {
            _almacenService = almacenService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return PartialView("~/Views/Reimpresion/Index.cshtml");
        }

        [HttpPost]
        public IActionResult ValidarSeriales([FromBody] List<string> seriales)
        {
            try
            {
                var serialesValidos = _almacenService.ObtenerSerialesValidos(seriales);

                return Json(new
                {
                    success = true,
                    seriales = serialesValidos
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Error interno: " + ex.Message
                });
            }
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