using System.Diagnostics;
using AnatoliaRoot_V2.Models;
using AnatoliaRoot_V2.Models.ViewModels;
using AnatoliaRoot_V2.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using AnatoliaRoot_V2.Data;
using System.Threading.Tasks;
using System.Linq;

namespace AnatoliaRoot_V2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        private readonly ExchangeRateService _exchangeRateService;
        private readonly GoldPriceService _goldPriceService;

        public HomeController(ILogger<HomeController> logger, AppDbContext context, ExchangeRateService exchangeRateService, GoldPriceService goldPriceService)
        {
            _logger = logger;
            _context = context;
            _exchangeRateService = exchangeRateService;
            _goldPriceService = goldPriceService;
        }

        public async Task<IActionResult> Index()
        {
            var lastRate = await _context.ExchangeRates
                .OrderByDescending(x => x.Date)
                .FirstOrDefaultAsync();
            ExchangeRateViewModel kurVm = null;
            if (lastRate != null)
            {
                kurVm = new ExchangeRateViewModel
                {
                    UsdTry = lastRate.UsdTry,
                    EurTry = lastRate.EurTry,
                    Date = lastRate.Date,
                    Source = lastRate.Source
                };
            }
            ViewBag.ExchangeRate = kurVm;

            var lastGoldPrice = await _context.GoldPrices
                .OrderByDescending(x => x.Date)
                .FirstOrDefaultAsync();
            GoldPriceViewModel altinVm = null;
            if (lastGoldPrice != null)
            {
                altinVm = new GoldPriceViewModel
                {
                    GramGold = lastGoldPrice.GramGold,
                    QuarterGold = lastGoldPrice.QuarterGold,
                    HalfGold = lastGoldPrice.HalfGold,
                    ChangeRate = lastGoldPrice.ChangeRate,
                    DayHigh = lastGoldPrice.DayHigh,
                    DayLow = lastGoldPrice.DayLow,
                    PrevClose = lastGoldPrice.PrevClose,
                    Date = lastGoldPrice.Date,
                    Source = lastGoldPrice.Source,
                    Currency = lastGoldPrice.Currency,
                    Exchange = lastGoldPrice.Exchange
                };
            }
            ViewBag.GoldPrice = altinVm;

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
