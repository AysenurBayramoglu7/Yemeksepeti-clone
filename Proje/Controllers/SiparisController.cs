using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using YemekSepeti.BLL.Abstract;
using YemekSepeti.Entities.Dtos;
using YemekSepeti.WebUI.Models;

namespace YemekSepeti.WebUI.Controllers
{
    [Authorize]
    public class SiparisController : Controller
    {
        private readonly ISiparisService _siparisService;

        public SiparisController(ISiparisService siparisService)
        {
            _siparisService = siparisService;
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
    }
}
