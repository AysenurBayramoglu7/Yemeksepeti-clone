using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YemekSepeti.BLL.Abstract;

namespace Proje.Controllers
{
    public class FavoriController : Controller
    {
        private readonly IFavoriRestoranlarService _favoriService;

        public FavoriController(IFavoriRestoranlarService favoriService)
        {
            _favoriService = favoriService;
        }

        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("GirisYap", "Kullanici");
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return RedirectToAction("GirisYap", "Kullanici");

            int kullaniciId = int.Parse(userIdClaim.Value);
            var favoriler = _favoriService.FavorileriGetir(kullaniciId);

            return View(favoriler);
        }

        [HttpPost]
        public IActionResult Ekle(int restoranId)
        {
            // Kullanıcı giriş yapmış mı kontrol et
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { success = false, message = "Lütfen önce giriş yapınız." });
            }

            // Claims üzerinden Kullanıcı ID'sini al
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Json(new { success = false, message = "Kullanıcı kimliği doğrulanamadı." });
            }

            int kullaniciId = int.Parse(userIdClaim.Value);

            try
            {
                _favoriService.FavoriEkle(kullaniciId, restoranId);
                return Json(new { success = true, message = "Restoran favorilere eklendi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Bir hata oluştu: " + ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Sil(int restoranId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { success = false, message = "Lütfen önce giriş yapınız." });
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Json(new { success = false, message = "Kullanıcı kimliği doğrulanamadı." });
            }

            int kullaniciId = int.Parse(userIdClaim.Value);

            try
            {
                _favoriService.FavoriSil(kullaniciId, restoranId);
                return Json(new { success = true, message = "Restoran favorilerden çıkarıldı." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Bir hata oluştu: " + ex.Message });
            }
        }
    }
}
