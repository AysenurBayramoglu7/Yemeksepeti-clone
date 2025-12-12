
using Microsoft.AspNetCore.Mvc;
using Proje.Models;
using System.Diagnostics;
using YemekSepeti.BLL.Abstract;
using YemekSepeti.WebUI.Models;
using System.Security.Claims;

namespace Proje.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IKategoriService _kategoriService;
        private readonly IRestoranService _restoranService;
        private readonly IFavoriRestoranlarService _favoriService;

        // Constructor
        public HomeController(
            ILogger<HomeController> logger,
            IKategoriService kategoriService,
            IRestoranService restoranService,
            IFavoriRestoranlarService favoriService)
        {
            _logger = logger;
            _kategoriService = kategoriService;
            _restoranService = restoranService;
            _favoriService = favoriService;
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

            // Eğer kullanıcı giriş yapmışsa, favori restoranlarını çek
            if (User.Identity.IsAuthenticated)
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null)
                {
                    int userId = int.Parse(userIdClaim.Value);
                    var favoriRestoranlar = _favoriService.FavorileriGetir(userId);
                    // Sadece ID'leri listeye alıyoruz
                    model.FavoriRestoranIdleri = favoriRestoranlar.Select(x => x.RestoranID).ToList();
                }
            }

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
