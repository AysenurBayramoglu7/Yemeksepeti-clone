using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YemekSepeti.BLL.Abstract;
using YemekSepeti.WebUI.Models;
// SessionExtensions Models altında olduğu için, o namespace'i kullanıyoruz.
// Ayrıca YemekSepeti.Entities namespace'i SepetItem için gerekli.
using YemekSepeti.Entities; 

namespace YemekSepeti.WebUI.Controllers
{
    // Sadece giriş yapmış kullanıcılar sepete erişebilir
    [Authorize]
    public class SepetController : Controller
    {
        private readonly IUrunService _urunService;

        public SepetController(IUrunService urunService)
        {
            _urunService = urunService;
        }

        // Sepet sayfasını gösterir
        public IActionResult Index()
        {
            // Session'dan sepeti oku, yoksa boş bir sepet oluştur
            var sepet = HttpContext.Session.GetJson<SepetViewModel>("Sepet") ?? new SepetViewModel();
            return View(sepet);
        }

        // Sepete ürün ekler (AJAX ile çağrılır)
        [HttpPost]
        public IActionResult Ekle(int urunId, int adet = 1)
        {
            var urun = _urunService.TGet(x => x.UrunId == urunId);
            if (urun == null)
            {
                return Json(new { success = false, message = "Ürün bulunamadı." });
            }

            // Session'dan sepeti çek veya yeni oluştur
            var sepet = HttpContext.Session.GetJson<SepetViewModel>("Sepet") ?? new SepetViewModel();

            // Ürün zaten sepette var mı kontrol et
            var mevcutUrun = sepet.SepetItems.FirstOrDefault(x => x.UrunId == urunId);
            if (mevcutUrun != null)
            {
                // Varsa adedini artır
                mevcutUrun.Adet += adet;
            }
            else
            {
                // Yoksa yeni kalem olarak ekle
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
