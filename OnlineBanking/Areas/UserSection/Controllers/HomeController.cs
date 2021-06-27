﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OnlineBanking.Data;
using OnlineBanking.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;


namespace OnlineBanking.Areas.UserSection.Controllers
{
    [Area("UserSection")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        //private readonly OnlineBankingContext _context;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        //public HomeController(OnlineBankingContext context)
        //{
        //    _context = context;
        //}

        [Route("PayyedDigibank/Dashboard")]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("NameCurrentUser") == null)
            {
                return RedirectToAction("Login", "Users");

            }
            ViewBag.Current = "Dashboard";
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
