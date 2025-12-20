using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using YemekSepeti.BLL.Abstract;
using YemekSepeti.Entities.Dtos;
using YemekSepeti.Entities;
using YemekSepeti.WebUI.Models;

namespace YemekSepeti.WebUI.Controllers
{
    [Authorize]
    public class SiparisController : Controller
    {
        private readonly ISiparisService _siparisService;
        private readonly IUrunService _urunService; // EKLENDİ

        public SiparisController(ISiparisService siparisService, IUrunService urunService) // GÜNCELLENDİ
        {
            _siparisService = siparisService;
            _urunService = urunService;
        }

        public IActionResult Index()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdStr, out int userId))
                return RedirectToAction("Index", "Home");

            // 1. Kullanıcının tüm siparişlerini çek
            var siparisler = _siparisService.KullaniciSiparisGecmisiGetir(userId)
                                            .OrderByDescending(x => x.Tarih)
                                            .ToList();

            // 2. Modeli doldur
            var modelListesi = new List<SiparisListesiViewModel>();

            foreach (var s in siparisler)
            {
                // Her sipariş için detayları al
                var spDenGelenDetay = _siparisService.SiparisDetayGetir(s.SiparisID);

                modelListesi.Add(new SiparisListesiViewModel
                {
                    Siparis = s,
                    Detaylar = spDenGelenDetay
                });
            }

            return View(modelListesi);
        }

        // MÜŞTERİ İÇİN SİPARİŞ DETAY SAYFASI
        [HttpGet]
        public IActionResult Detay(int id)
        {
            // 1. Giriş yapan kullanıcıyı bul
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("GirisYap", "Kullanici");

            // 2. Kullanıcının tüm siparişlerini çek
            var siparisler = _siparisService.KullaniciSiparisGecmisiGetir(userId)
                                            .OrderByDescending(x => x.Tarih)
                                            .ToList();

            // 3. Modeli doldur (SP ile detayları çekerek)
            var modelListesi = new List<SiparisListesiViewModel>();

            foreach (var s in siparisler)
            {
                // BURASI KRİTİK: Her sipariş için SP'ye gidip detayları alıyoruz
                var spDenGelenDetay = _siparisService.SiparisDetayGetir(s.SiparisID);

                modelListesi.Add(new SiparisListesiViewModel
                {
                    Siparis = s,       // Ana veri
                    Detaylar = spDenGelenDetay // SP'den gelen detay verisi
                });
            }

            // 4. Dolu paketi View'a gönder
            return View(modelListesi);
        }
        // SİPARİŞ İPTAL ET (KULLANICI)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult IptalEt(int siparisId)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("GirisYap", "Kullanici");

            var siparis = _siparisService.TGet(x => x.SiparisID == siparisId);

            // Güvenlik: Sipariş bu kullanıcıya mı ait?
            if (siparis == null || siparis.KullaniciID != userId)
            {
                TempData["Hata"] = "Sipariş bulunamadı veya yetkiniz yok.";
                return RedirectToAction("Index");
            }

            // Kural: Sadece "Onay Bekliyor" aşamasındaysa iptal edilebilir.
            // (Hazırlanıyor ve sonrası iptal edilemez)
            if ((int)siparis.Durum >= (int)SiparisDurumu.Hazirlaniyor)
            {
                TempData["Hata"] = "Sipariş hazırlanmaya başlandığı için iptal edilemez.";
                return RedirectToAction("Index");
            }

            // --- STOK GERİ YÜKLEME ---
            // 1. Sipariş detaylarını çek (Entity üzerinden)
            var detaylar = _siparisService.GetSiparisDetaylariEntity(siparisId);
            
            // 2. Her bir ürün için stoğu artır
            foreach (var item in detaylar)
            {
                 var urun = _urunService.TGet(u => u.UrunId == item.UrunID); 
                 if (urun != null)
                 {
                     urun.Stok += item.Adet;
                     _urunService.TUpdate(urun);
                 }
            }
            // -------------------------

            // İptal işlemini yap
            try
            {
               _siparisService.KullaniciSiparisIptal(siparisId, userId);
            }
            catch (Exception ex)
            {
                TempData["Hata"] = ex.Message;
                return RedirectToAction("Index");
            }
            
            TempData["Basari"] = "Siparişiniz başarıyla iptal edildi ve tutar iade edildi.";
            return RedirectToAction("Index");
        }
    }
}
