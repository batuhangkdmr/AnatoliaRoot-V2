using System.Diagnostics;
using AnatoliaRoot_V2.Models;
using Microsoft.AspNetCore.Mvc;

namespace AnatoliaRoot_V2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // Kurumsal Sayfaları
        public IActionResult About()
        {
            ViewData["Title"] = "Hakkımızda";
            return View();
        }

        public IActionResult Mission()
        {
            ViewData["Title"] = "Misyonumuz";
            return View();
        }

        public IActionResult Vision()
        {
            ViewData["Title"] = "Vizyonumuz";
            return View();
        }

        public IActionResult Quality()
        {
            ViewData["Title"] = "Kalite Politikamız";
            return View();
        }

        // Hizmetler Sayfası
        public IActionResult Services()
        {
            ViewData["Title"] = "Hizmetlerimiz";
            return View();
        }

        // İletişim Sayfası
        public IActionResult Contact()
        {
            ViewData["Title"] = "İletişim";
            return View();
        }

    }
}
