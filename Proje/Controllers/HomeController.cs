
using Microsoft.AspNetCore.Mvc;
using Proje.Models;
using System.Diagnostics;
using YemekSepeti.BLL.Abstract;
using YemekSepeti.WebUI.Models;

namespace Proje.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IKategoriService _kategoriService;
        private readonly IRestoranService _restoranService;

        // Constructor
        public HomeController(
            ILogger<HomeController> logger,
            IKategoriService kategoriService,
            IRestoranService restoranService)
        {
            _logger = logger;
            _kategoriService = kategoriService;
            _restoranService = restoranService;
        }

        public IActionResult Index()
        {
            var kategoriler = _kategoriService.TGetList();
            var restoranlar = _restoranService.TGetList();

            var model = new HomeViewModel
            {
                Kategoriler = kategoriler,
                Restoranlar = restoranlar
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
