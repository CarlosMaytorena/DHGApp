﻿using AgricolaDH_GApp.DataAccess;
using AgricolaDH_GApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AgricolaDH_GApp.Controllers
{
    public class RequisicionController : Controller
	{
		private readonly ILogger<RequisicionController> _logger;
		
		private readonly AppDbContext context;

		public RequisicionController(ILogger<RequisicionController> logger, AppDbContext _ctx)
		{
			_logger = logger;
			context = _ctx;
		}

		[HttpGet]
		public IActionResult Index()
		{

			return PartialView("~/Views/Requisicion/Index.cshtml");
		}

        [HttpPost]
        public IActionResult CrearRequisicion()
        {
            return PartialView("~/Views/Requisicion/RequisicionForm.cshtml");
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