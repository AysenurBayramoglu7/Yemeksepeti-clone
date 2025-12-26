using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YemekSepeti.BLL.Abstract;
using YemekSepeti.Entities;

namespace YemekSepeti.WebUI.Controllers
{
    [Authorize] // Sadece giriş yapmış kullanıcılar
    public class YorumController : Controller
    {
        private readonly IYorumService _yorumService;
        private readonly ISiparisService _siparisService;

        public YorumController(IYorumService yorumService, ISiparisService siparisService)
        {
            _yorumService = yorumService;
            _siparisService = siparisService;
        }

        // Post ile yorum ekleme
        [HttpPost]
        public IActionResult Ekle(int siparisId, int puan, string yorumMetni)
        {
            //Giriş yapan kullanıcıyı bul
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId))
            {
                TempData["Hata"] = "Giriş yapmanız gerekiyor.";
                return RedirectToAction("Index", "Siparis");
            }

            //Siparişin bu kullanıcıya ait olduğunu ve teslim edildi mi bak 
            var siparis = _siparisService.TGet(s => s.SiparisID == siparisId);
            
            if (siparis == null)
            {
                TempData["Hata"] = "Sipariş bulunamadı.";
                return RedirectToAction("Index", "Siparis");
            }

            if (siparis.KullaniciID != userId)
            {
                TempData["Hata"] = "Bu sipariş size ait değil.";
                return RedirectToAction("Index", "Siparis");
            }

            if (siparis.Durum != SiparisDurumu.TeslimEdildi)
            {
                TempData["Hata"] = "Sadece teslim edilmiş siparişlere yorum yapabilirsiniz.";
                return RedirectToAction("Index", "Siparis");
            }

            //Yorumu oluştur ve kaydet
            try
            {
                // Mevcut yorumu kontrol et yorum var mı yok mu eğer varsa güncelle yoksa ekle
                var existingYorum = _yorumService.TGet(x => x.SiparisID == siparisId);

                if (existingYorum != null)
                {
                    // Yorum varsa güncelleme
                    existingYorum.Puan = puan;
                    existingYorum.YorumMetni = yorumMetni;
                    existingYorum.RestoranID = siparis.RestoranID;
                    existingYorum.KullaniciID = userId; 

                    _yorumService.TYorumGuncelleSP(existingYorum);
                    TempData["Basari"] = "Yorumunuz başarıyla güncellendi.";
                }
                else
                {
                    // Yorum yoksa ekleme
                    var yorum = new Yorum
                    {
                        RestoranID = siparis.RestoranID,
                        KullaniciID = userId,
                        Puan = puan,
                        YorumMetni = yorumMetni,
                        CreatedAt = DateTime.Now,
                        AktifMi = true,
                        SiparisID = siparisId
                    };

                    _yorumService.TYorumEkleSP(yorum);
                    TempData["Basari"] = "Yorumunuz başarıyla kaydedildi. Teşekkür ederiz!";
                }
            }
            catch (Exception ex)
            {
                TempData["Hata"] = ex.Message;
            }

            return RedirectToAction("Index", "Siparis");
        }

        [HttpGet]
        public IActionResult GetYorum(int siparisId)
        {
            var yorum = _yorumService.TGet(x => x.SiparisID == siparisId);
            if (yorum != null)
            {
                return Json(new { exists = true, puan = yorum.Puan, yorum = yorum.YorumMetni });
            }
            return Json(new { exists = false });
        }

        //Kullanıcı Yorum Silme İşlemi de yapalabilir.
        [HttpPost]
        public IActionResult Sil(int siparisId)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId))
            {
                return Json(new { success = false, message = "Oturum açmanız gerekiyor." });
            }

            try
            {
                var yorum = _yorumService.TGet(x => x.SiparisID == siparisId);
                if (yorum == null)
                {
                    return Json(new { success = false, message = "Yorum bulunamadı." });
                }

                // Yorumu silmeye çalışan kişi, yorumun sahibi mi kontrol edilmeli.
                if (yorum.KullaniciID != userId)
                {
                    return Json(new { success = false, message = "Bu yorumu silmeye yetkiniz yok." });
                }

                _yorumService.TDelete(yorum);
                
                // Trigger veritabanında tanımlı olduğu için, silme işlemi sonrası 
                // otomatik olarak Restoran puanını güncelleyecektir.
                
                return Json(new { success = true, message = "Yorum başarıyla silindi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata oluştu: " + ex.Message });
            }
        }
    }
}
