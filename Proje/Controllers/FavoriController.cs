using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YemekSepeti.BLL.Abstract;
using YemekSepeti.Entities;
using YemekSepeti.WebUI.Models;

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
        //Bu sayfa yenilenmeden, AJAX ile favori ekleme/silme işlemi yapıyor.
        // return olarak view değil json döndürüyoruz.Yeni sayfa açılmasın diye.
        [HttpPost]
        public IActionResult Ekle(int restoranId)
        {
            // Kullanıcı giriş yapmış mı kontrol et
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { success = false, message = "Lütfen önce giriş yapınız." });
            }
            //ClaimTypes.NameIdentifier= kullanıcının ıd sini tutan yer
            // Claims üzerinden Kullanıcı ID'sini al
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);//kullanıcının ID’sini cookie içine claim olarak saklar.
            if (userIdClaim == null)
            {
                return Json(new { success = false, message = "Kullanıcı kimliği doğrulanamadı." });
            }

            int kullaniciId = int.Parse(userIdClaim.Value);//ID yi string den int e çeviriyoruz.

            try
            {
                _favoriService.FavoriEkle(kullaniciId, restoranId); // Favori eklemek için metodu çağır.
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
