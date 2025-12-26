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
        private readonly IUrunService _urunService; 

        public SiparisController(ISiparisService siparisService, IUrunService urunService) 
        {
            _siparisService = siparisService;
            _urunService = urunService;
        }

        public IActionResult Index()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdStr, out int userId))
                return RedirectToAction("Index", "Home");

            //Kullanıcının tüm siparişlerini çek
            var siparisler = _siparisService.KullaniciSiparisGecmisiGetir(userId)
                                            .OrderByDescending(x => x.Tarih)
                                            .ToList();

            
            var modelListesi = new List<SiparisListesiViewModel>();

            foreach (var s in siparisler)
            {
                //Her sipariş için detayları al
                var spDenGelenDetay = _siparisService.SiparisDetayGetir(s.SiparisID);

                modelListesi.Add(new SiparisListesiViewModel
                {
                    Siparis = s,
                    Detaylar = spDenGelenDetay
                });
            }

            return View(modelListesi);
        }

        // Kullanıcılar için sipariş detay sayfası
        [HttpGet]
        public IActionResult Detay(int id)
        {
            //Giriş yapan kullanıcıyı bul
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("GirisYap", "Kullanici");

            //Kullanıcının tüm siparişlerini çek
            var siparisler = _siparisService.KullaniciSiparisGecmisiGetir(userId)
                                            .OrderByDescending(x => x.Tarih)
                                            .ToList();

            //SP ile detayları çekerek modeli doldur
            var modelListesi = new List<SiparisListesiViewModel>();

            //burada sadece id'si verilen siparişi bulup detaylarını alıyoruz.Sp ile
            foreach (var s in siparisler)
            {
                //Her sipariş için SP'ye gidip detayları alıyoruz
                var spDenGelenDetay = _siparisService.SiparisDetayGetir(s.SiparisID);

                //View de hwm sipariş ve detayları birlikte göstermek için modeli doldur
                modelListesi.Add(new SiparisListesiViewModel
                {
                    Siparis = s,       // Ana veri
                    Detaylar = spDenGelenDetay // SP'den gelen detay verisi
                });
            }

            // Dolu paketi View'a gönder
            return View(modelListesi);
        }
        // SİPARİŞ İPTAL ET KULLANICI TARAFI İÇİN 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult IptalEt(int siparisId)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("GirisYap", "Kullanici");

            var siparis = _siparisService.TGet(x => x.SiparisID == siparisId);

            // Sipariş bu kullanıcıya mı ait bakılır
            if (siparis == null || siparis.KullaniciID != userId)
            {
                TempData["Hata"] = "Sipariş bulunamadı veya yetkiniz yok.";
                return RedirectToAction("Index");
            }

            //Sadece "Onay Bekliyor" aşamasındaysa iptal edilebilir.
            // (Hazırlanıyor ve sonrası iptal edilemez)
            if ((int)siparis.Durum >= (int)SiparisDurumu.Hazirlaniyor)
            {
                TempData["Hata"] = "Sipariş hazırlanmaya başlandığı için iptal edilemez.";
                return RedirectToAction("Index");
            }

            // --- STOK GERİ YÜKLEME ---
            // detayları al
            var detaylar = _siparisService.GetSiparisDetaylariEntity(siparisId);
            
            //Her bir ürün için stoğu artır
            foreach (var item in detaylar)
            {
                 var urun = _urunService.TGet(u => u.UrunId == item.UrunID); 
                 if (urun != null)
                 {
                     urun.Stok += item.Adet;
                     _urunService.TUpdate(urun);
                 }
            }
            //kullanıcı sipariş iptal işlemi
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
