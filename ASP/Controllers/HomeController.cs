﻿using System.Diagnostics;
using ASP.Models;
using ASP.Models.Home;
using Microsoft.AspNetCore.Mvc;
using ASP.Models.Entities;
using ASP.Services.Time;
using ASP.Services.Random;
using ASP.Services.Identity;

namespace ASP.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITimeService _timeService;
        private readonly IRandomService _randomService;
        private readonly IIdentityService _identityService;

        public HomeController(ILogger<HomeController> logger, ITimeService timeService, IRandomService randomService, IIdentityService identityService)
        {
            _logger = logger;
            _timeService = timeService;
            _randomService = randomService;
            _identityService = identityService;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Razor()
        {
            HomeRazorPageModel model = new()
            {
                Arr = [ "Item I", "Item II", "Item III", "Item IV", "Item V" ]
            };
            return View(model);
        }
        public IActionResult Demo()
        {
            HomeDemoViewModel model = new()
            {
                Items = [new Item() { Name="Ландау, Лифшиц Курс теоретической физики Том 1 Механика", Number = 1, Price = 25 },
                        new Item() { Name="Ландау, Лифшиц Курс теоретической физики Том 2 Теория поля", Number = 1, Price = 25 },
                        new Item() { Name="Ландау, Лифшиц Курс теоретической физики Том 3 Квантовая механика", Number = 1, Price = 25 },
                        new Item() { Name="Ландау, Лифшиц Курс теоретической физики Том 4 Квантовая электродинамика", Number = 1, Price = 25 },
                        new Item() { Name="Ландау, Лифшиц Курс теоретической физики Том 5 Статистическая физика часть 1", Number = 1, Price = 25 },
                        new Item() { Name="Ландау, Лифшиц Курс теоретической физики Том 6 Гидродинамика", Number = 1, Price = 25 },
                        new Item() { Name="Ландау, Лифшиц Курс теоретической физики Том 7 Теория упругости", Number = 1, Price = 25 },
                        new Item() { Name="Ландау, Лифшиц Курс теоретической физики Том 8 Электродинамика сплошных сред", Number = 1, Price = 25 },
                        new Item() { Name="Ландау, Лифшиц Курс теоретической физики Том 9 Квантовая электродинамика", Number = 1, Price = 25 },
                        new Item() { Name="Ландау, Лифшиц Курс теоретической физики Том 10 Статистическая физика часть 2", Number = 1, Price = 25 },
                        new Item() { Name="Ландау, Лифшиц Курс теоретической физики Том 11 Физическая кинетика", Number = 1, Price = 25 }]
            };
            return View(model);
        }

        public IActionResult Ioc() // Inversion of control
        {
            //ViewData["timestamp"] = _timeService.Timestamp() + " -- Random: " + _randomService.Otp();
            ViewData["identity"] = "Id 1: " + _identityService.GenerateId() + " -- Id 2: " + _identityService.GenerateId() + " -- Id 3: " + _identityService.GenerateId();
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
