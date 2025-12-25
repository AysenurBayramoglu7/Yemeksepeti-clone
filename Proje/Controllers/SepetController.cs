using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YemekSepeti.BLL.Abstract;
using YemekSepeti.WebUI.Models;
using YemekSepeti.Entities; 

namespace YemekSepeti.WebUI.Controllers
{
    // Sadece giriş yapmış kullanıcılar sepete erişebilir
    public class SepetController : Controller
    {
        private readonly IUrunService _urunService;
        private readonly ISiparisService _siparisService;

        public SepetController(IUrunService urunService, ISiparisService siparisService)
        {
            _urunService = urunService;
            _siparisService = siparisService;
        }

        // Sepet sayfasını gösterir
        public IActionResult Index()
        {
            // Session'dan sepeti oku, yoksa boş bir sepet oluştur
            var sepet = HttpContext.Session.GetJson<SepetViewModel>("Sepet") ?? new SepetViewModel();
            return View(sepet);
        }

        // Sepete ürün ekler (AJAX ile çağrılır)
        //[HttpPost]
        [HttpPost]
        public IActionResult Ekle(int urunId, int adet = 1)
        {
            var urun = _urunService.TGet(x => x.UrunId == urunId);
            if (urun == null)
            {
                return Json(new { success = false, message = "Ürün bulunamadı." });
            }

            // --- STOK KONTROLÜ (Ekleme Anında) ---
            if (urun.Stok <= 0)
            {
                return Json(new { success = false, message = "Üzgünüz, bu ürün stokta kalmadı." });
            }

            // Session'dan sepeti çek veya yeni oluştur
            var sepet = HttpContext.Session.GetJson<SepetViewModel>("Sepet") ?? new SepetViewModel();

            // Ürün zaten sepette var mı kontrol et
            var mevcutUrun = sepet.SepetItems.FirstOrDefault(x => x.UrunId == urunId);
            
            int mevcutAdet = mevcutUrun?.Adet ?? 0;
            int istenenToplamAdet = mevcutAdet + adet;

            // Eğer istenen adet stoktan fazlaysa
            if (istenenToplamAdet > urun.Stok)
            {
                 return Json(new { success = false, message = $"Stokta sadece {urun.Stok} adet var, daha fazla ekleyemezsiniz." });
            }

            // Stok yeterliyse normal ekle
            if (mevcutUrun != null)
            {
                mevcutUrun.Adet += adet;
            }
            else
            {
                sepet.SepetItems.Add(new SepetItem
                {
                    UrunId = urun.UrunId,
                    UrunAd = urun.UrunAd,
                    Fiyat = urun.Fiyat,
                    Adet = adet,
                    FotoUrl = urun.FotoUrl,
                    RestoranId = urun.RestoranID
                });
            }

            // Güncellenmiş sepeti tekrar Session'a kaydet
            HttpContext.Session.SetJson("Sepet", sepet);

            return Json(new { success = true, message = "Ürün sepete eklendi!", sepetCount = sepet.ToplamAdet });
        }

        // Sepet Miktarını Güncelle (+ / - Butonları için)
        public IActionResult Guncelle(int urunId, int adet)
        {
            var sepet = HttpContext.Session.GetJson<SepetViewModel>("Sepet");
            if (sepet != null)
            {
                var urun = sepet.SepetItems.FirstOrDefault(x => x.UrunId == urunId);
                if (urun != null)
                {
                    // Stok Kontrolü
                    var dbUrun = _urunService.TGet(x => x.UrunId == urunId);
                    int yeniAdet = urun.Adet + adet;

                    if (yeniAdet > dbUrun.Stok)
                    {
                        TempData["ErrorMessage"] = $"Stokta sadece {dbUrun.Stok} adet var, daha fazla ekleyemezsiniz.";
                    }
                    else if (yeniAdet > 0)
                    {
                        urun.Adet = yeniAdet;
                    }
                    else
                    {
                         // 0 veya altına düşerse silmeyelim, 1 de kalsın veya kullanıcı "Sil" butonunu kullansın
                         // İsteğe bağlı: 1'in altına inmesin
                         if(yeniAdet <= 0) urun.Adet = 1;
                    }
                    
                    HttpContext.Session.SetJson("Sepet", sepet);
                }
            }
            return RedirectToAction("Index");
        }

        // Siparişi Tamamla (Eski: Direkt oluşturuyordu, Yeni: Ödeme sayfasına yönlendiriyor)
        [HttpGet]
        public IActionResult Tamamla()
        {
            // Kullanıcı giriş yapmamışsa
            if (!User.Identity.IsAuthenticated)
            {
                // Giriş sayfasına yönlendir, dönüş adresi olarak bu sayfayı ver
                return RedirectToAction("GirisYap", "Kullanici", new { returnUrl = "/Sepet/Tamamla" });
            }

            // Sepet Dolu mu?
            var sepet = HttpContext.Session.GetJson<SepetViewModel>("Sepet");
            if (sepet == null || sepet.SepetItems.Count == 0)
            {
                TempData["ErrorMessage"] = "Sepetiniz boş!";
                return RedirectToAction("Index");
            }

            // Ödeme Sayfasına Yönlendir
            return RedirectToAction("Odeme");
        }

        // Ödeme Sayfası (GET)
        [HttpGet]
        [Authorize] // Sadece giriş yapmış kullanıcılar
        public IActionResult Odeme()
        {
            var sepet = HttpContext.Session.GetJson<SepetViewModel>("Sepet");
            if (sepet == null || sepet.SepetItems.Count == 0)
            {
                return RedirectToAction("Index");
            }

            var model = new OdemeViewModel
            {
                Sepet = sepet
            };

            return View(model);
        }

        // Ödeme ve Sipariş Onayı (POST)
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult OdemeOnayla(OdemeViewModel model)
        {
            // Session'dan sepeti tekrar al (Güvenlik için, modelden almıyoruz)
            var sepet = HttpContext.Session.GetJson<SepetViewModel>("Sepet");
            if (sepet == null || sepet.SepetItems.Count == 0)
            {
                return RedirectToAction("Index");
            }
            model.Sepet = sepet; // View'a geri dönerse sepet görünsün diye

            if (!ModelState.IsValid)
            {
                return View("Odeme", model);
            }

            // --- STOK KONTROLÜ (MANUEL DÜZELTME İÇİN UYARI) ---
            bool stokSorunuVar = false;
            List<string> uyariMesajlari = new List<string>();

            foreach (var item in sepet.SepetItems)
            {
                var urun = _urunService.TGet(x => x.UrunId == item.UrunId);
                int gercekStok = urun?.Stok ?? 0;

                if (gercekStok < item.Adet)
                {
                    stokSorunuVar = true;
                    uyariMesajlari.Add($"'{item.UrunAd}' için stok yetersiz! (Stok: {gercekStok}, Sepetiniz: {item.Adet}). Lütfen sepetinize dönüp adedi azaltın.");
                }
            }

            if (stokSorunuVar)
            {
                ViewData["Hata"] = string.Join("<br/>", uyariMesajlari);
                return View("Odeme", model);
            }

            // Siparişi Oluştur
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdStr, out int userId))
            {
                var siparis = new YemekSepeti.Entities.Siparis
                {
                    KullaniciID = userId,
                    RestoranID = sepet.SepetItems.First().RestoranId,
                    ToplamTutar = sepet.GenelToplam,
                    Tarih = DateTime.Now,
                    TeslimatAdresi = model.TeslimatAdresi, // Kullanıcının girdiği adres
                    Durum = YemekSepeti.Entities.SiparisDurumu.OnayBekliyor,
                    TakipKodu = Guid.NewGuid().ToString().Substring(0, 8).ToUpper()
                };

                foreach (var item in sepet.SepetItems)
                {
                    siparis.SiparisDetaylar.Add(new YemekSepeti.Entities.SiparisDetay
                    {
                        UrunID = item.UrunId,
                        Adet = item.Adet,
                        Fiyat = item.Fiyat
                    });
                }

                // Veritabanına Kaydet 
                // Trigger devreye girecek ve stoğu düşecek.
                _siparisService.TInsert(siparis);

                // Sepeti Temizle
                HttpContext.Session.Remove("Sepet");

                TempData["SuccessMessage"] = $"Siparişiniz alındı! Takip Kodu: {siparis.TakipKodu}";
                return RedirectToAction("Index", "Siparis");
            }

            return RedirectToAction("Index");
        }

        // Sepetten sil
        [HttpPost]
        public IActionResult Sil(int urunId)
        {
            var sepet = HttpContext.Session.GetJson<SepetViewModel>("Sepet");
            if (sepet != null)
            {
                var urun = sepet.SepetItems.FirstOrDefault(x => x.UrunId == urunId);
                if (urun != null)
                {
                    sepet.SepetItems.Remove(urun);
                    HttpContext.Session.SetJson("Sepet", sepet);
                }
            }
            return RedirectToAction("Index");
        }

        // Sepeti tamamen boşaltır
        public IActionResult Temizle()
        {
            HttpContext.Session.Remove("Sepet");
            return RedirectToAction("Index");
        }
    }
}
