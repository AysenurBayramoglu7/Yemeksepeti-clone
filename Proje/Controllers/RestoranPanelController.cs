using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Security.Claims;
using YemekSepeti.BLL.Abstract;
using YemekSepeti.Entities;
using YemekSepeti.WebUI.Models;

namespace YemekSepeti.WebUI.Controllers
{
    // Admin panelinde [Authorize(Roles = "Admin")] vardı.
    // Burada giriş yapmış olması yeterli, içeride "Restoran Sahibi mi?" kontrolü yapacağız.
    // Eğer Rol sistemin oturmuşsa [Authorize(Roles = "RestoranSahibi")] de diyebilirsin.
    [Authorize]
    public class RestoranPanelController : Controller
    {
        private readonly ISiparisService _siparisService;
        private readonly IRestoranService _restoranService;
        private readonly IKullaniciService _kullaniciService; // Kullanıcı bilgilerini teyit için

        public RestoranPanelController(
            ISiparisService siparisService,
            IRestoranService restoranService,
            IKullaniciService kullaniciService)
        {
            _siparisService = siparisService;
            _restoranService = restoranService;
            _kullaniciService = kullaniciService;
        }

        // ----------------- PANEL ANASAYFA (SİPARİŞ LİSTESİ) -----------------
        // AdminController'daki "RestoranListesi" mantığıyla aynı.
        // Ama burada TÜM restoranları değil, sadece KENDİ siparişlerini listeliyor.
        public IActionResult Index()
        {
            // 1. Giriş yapan restoran sahibini bul
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("GirisYap", "Kullanici");

            var restoran = _restoranService.TGet(x => x.KullaniciID == userId);
            if (restoran == null) return Content("HATA: Kayıtlı bir restoranınız yok.");

            // 2. Siparişleri çek (RestoranID filtresiyle)
            var siparisler = _siparisService.TGetList(x => x.RestoranID == restoran.RestoranID)
                                            .OrderByDescending(x => x.Tarih) // En yeni en üstte
                                            .ToList();

            // 3. Admin panelindeki gibi ilişkili verileri doldurma (gerekirse)
            // Örn: Kullanıcı adını görmek istersen burada siparis.Kullanici = ... diyebilirsin.

            ViewBag.RestoranAd = restoran.RestoranAd; // Başlıkta göstermek için

            return View(siparisler);
        }

        // ----------------- DURUM GÜNCELLEME (POST) -----------------
        // AdminController'daki "RestoranOnayla" veya "RestoranDuzenle" mantığıyla aynı.
        [HttpPost]
        [ValidateAntiForgeryToken] // Admin panelindeki güvenlik
        public IActionResult DurumGuncelle(int siparisId, int yeniDurum)
        {
            // A) Enum Güvenlik Kontrolü
            if (!Enum.IsDefined(typeof(SiparisDurumu), yeniDurum))
            {
                TempData["Hata"] = "Geçersiz sipariş durumu.";
                return RedirectToAction(nameof(Index));
            }

            // B) Kullanıcı Kontrolü
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("GirisYap", "Kullanici");

            try
            {
                // C) Servis Çağrısı (Admin panelinde _service.TUpdate çağırıyordun, burada özel metodumuzu çağırıyoruz)
                _siparisService.SiparisDurumGuncelle(
                    siparisId,
                    (SiparisDurumu)yeniDurum,
                    userId
                );

                TempData["Basarili"] = "Sipariş durumu güncellendi.";
            }
            catch (Exception ex)
            {
                // Admin panelindeki gibi hata yakalama
                TempData["Hata"] = ex.Message;
            }

            return RedirectToAction(nameof(Index)); // Admin'deki "nameof" kullanımı
        }

        // ----------------- RESTORAN BİLGİLERİM (GET) -----------------
        // AdminController'daki "RestoranDuzenle" ile AYNI yapı.
        // Restoran sahibi kendi bilgilerini görüp düzenleyebilsin.
        [HttpGet]
        public IActionResult Bilgilerim()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int.TryParse(userIdStr, out int userId);

            var restoran = _restoranService.TGet(x => x.KullaniciID == userId);
            if (restoran == null) return NotFound();

            return View(restoran);
        }

        // ----------------- RESTORAN BİLGİLERİM (POST) -----------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Bilgilerim(Restoran model)
        {
            // Admin panelindeki Validasyon mantığı
            // Not: Restoran sahibi onay durumunu veya ID'sini değiştiremez, o yüzden dikkatli mapping yapıyoruz.

            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int.TryParse(userIdStr, out int userId);

            var existing = _restoranService.TGet(x => x.KullaniciID == userId);
            if (existing == null) return NotFound();

            // Sadece izin verilen alanları güncelliyoruz (Admin panelindeki mantık)
            existing.RestoranAd = model.RestoranAd;
            existing.Adres = model.Adres;
            existing.Telefon = model.Telefon;
            existing.MinSiparisTutar = model.MinSiparisTutar;
            // existing.OnayliMi = model.OnayliMi; //BUNU YAPMIYORUZ! Onayı sadece Admin verir.

            _restoranService.TUpdate(existing);

            TempData["Basarili"] = "Bilgileriniz güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        // ----------------- SİPARİŞ DETAY SAYFASI (GET) -----------------
        [HttpGet]
        public IActionResult SiparisDetay(int id)
        {
            // 1. Güvenlik: Giriş yapan restoran sahibini bul
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("GirisYap", "Kullanici");

            var restoran = _restoranService.TGet(x => x.KullaniciID == userId);
            if (restoran == null) return RedirectToAction("Index");

            // 2. Siparişin genel bilgilerini çek (Siparis Tablosu)
            var siparis = _siparisService.TGet(x => x.SiparisID == id);

            // 3. GÜVENLİK KONTROLÜ: Sipariş bu restorana mı ait?
            if (siparis == null || siparis.RestoranID != restoran.RestoranID)
            {
                TempData["Hata"] = "Sipariş bulunamadı veya görüntüleme yetkiniz yok.";
                return RedirectToAction("Index");
            }

            // 4. Siparişin İÇERİĞİNİ çek (SiparisDetaylari Tablosu)
            // İşte burası senin veritabanındaki o tabloyu okuyan yer!
            var urunDetaylari = _siparisService.SiparisDetayGetir(id);

            // 5. Kutuyu (ViewModel) doldur
            var model = new RestoranSiparisDetayViewModel
            {
                Siparis = siparis,
                Urunler = urunDetaylari
            };

            // 6. Sayfayı aç
            return View(model);
        }
    }
}