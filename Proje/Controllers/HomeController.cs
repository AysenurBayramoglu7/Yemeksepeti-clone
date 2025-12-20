using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proje.Models;
using System.Diagnostics;
using System.Security.Claims;
using YemekSepeti.BLL.Abstract;
using YemekSepeti.DAL;
using YemekSepeti.WebUI.Models;

namespace Proje.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IKategoriService _kategoriService;
        private readonly IRestoranService _restoranService;
        private readonly IFavoriRestoranlarService _favoriService;
        private readonly YemekSepetiDbContext _context;

       

        // Constructor
        public HomeController(
            ILogger<HomeController> logger,
            IKategoriService kategoriService,
            IRestoranService restoranService,
            IFavoriRestoranlarService favoriService,
            YemekSepetiDbContext context)
        {
            _logger = logger;
            _kategoriService = kategoriService;
            _restoranService = restoranService;
            _favoriService = favoriService;
            _context = context;
        }

        public IActionResult Index(string text)
        {
            var kategoriler = _kategoriService.TGetList();
            List<YemekSepeti.Entities.Restoran> restoranlar;

            if (!string.IsNullOrEmpty(text))
            {
                ViewData["SearchText"] = text; // View'da göstermek için
                
                // Parametreyi güvenli bir şekilde oluşturuyoruz
                var pText = new Microsoft.Data.SqlClient.SqlParameter("@text", text);

                // 1. ADIM: SP'den sadece filtreleme sonucunu (ID'leri) alıyoruz
                var aramaSonuclari = _context.RestoranSonuc
                    .FromSqlRaw("EXEC up_Arama @text", pText)
                    .ToList();

                // 2. ADIM: ID listesini çıkarıyoruz
                var ids = aramaSonuclari.Select(x => x.RestoranID).ToList();

                // 3. ADIM: Gerçek verileri BLL üzerinden çekiyoruz (Böylece Puan, Tutar vb. dolu geliyor)
                restoranlar = _restoranService
                                .TGetList(r => ids.Contains(r.RestoranID));
            }
            else
            {
                restoranlar = _restoranService.TGetList();
            }

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
                    var favoriRestoranlar = _favoriService.FavorileriGetir(userId);// Kullanıcının favori restoranlarını alıyoruz
                    // Sadece ID'leri listeye alıyoruz
                    model.FavoriRestoranIdleri = favoriRestoranlar.Select(x => x.RestoranID).ToList();
                }
            }

            return View(model); // Tüm verileri ViewModel ile birlikte View'a gönder
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
