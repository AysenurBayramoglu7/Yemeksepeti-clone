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
        private readonly ILogger<HomeController> _logger; //logger ekledik. Hata gibi şeylerde kullanabiliriz
        private readonly IKategoriService _kategoriService;
        private readonly IRestoranService _restoranService;
        private readonly IFavoriRestoranlarService _favoriService;
        private readonly YemekSepetiDbContext _context; // DbContext ekledik.Doğrudan SP çağrısı için



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

            if (!string.IsNullOrEmpty(text))// Arama yapıldıysa
            {
                ViewData["SearchText"] = text; // View'e gönderilir 

                //Burada sql enjeksiyon saldırılarını önlemek için parametre kullanıyoruz
                var pText = new Microsoft.Data.SqlClient.SqlParameter("@text", text);

                // SP'den sadece filtreleme sonucunu (ID'leri) alıyoruz
                var aramaSonuclari = _context.RestoranSonuc // RestoranSonuc, SP sonucu için kullanılan Dto sınıfıdır. 
                    .FromSqlRaw("EXEC up_Arama @text", pText)
                    .ToList();

                // Sp den dönen ID'leri listeye alıyoruz
                var ids = aramaSonuclari.Select(x => x.RestoranID).ToList();

                //Gerçek verileri BLL üzerinden çekiyoruz. yani Puan, Tutar gibi kısımlar dolu geliyor
                restoranlar = _restoranService
                                .TGetList(r => ids.Contains(r.RestoranID));//asıl veri restoranlar tablosunda sp ile filtreleme yapıyoruz
            }
            else
            {
                restoranlar = _restoranService.TGetList();//arama yapılmadıysa tüm restoranları getir
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
                    int userId = int.Parse(userIdClaim.Value);// Kullanıcı ID'sini alıyoruz
                    var favoriRestoranlar = _favoriService.FavorileriGetir(userId);// Kullanıcının favori restoranlarını alıyoruz
                    // Favori restoran ID'lerini ViewModel'e atıyoruz. kalpli olanları dolu göstermek için.
                    model.FavoriRestoranIdleri = favoriRestoranlar.Select(x => x.RestoranID).ToList();

                    // Partial View için ViewBag'e de atıyoruz
                    ViewBag.FavoriRestoranIdleri = model.FavoriRestoranIdleri;
                }
            }

            return View(model); // Tüm verileri ViewModel ile birlikte View'a gönder
        }

        public IActionResult Privacy()
        {
            return View();
        }
        // Hata sayfası için
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
