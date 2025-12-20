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
        private readonly IKullaniciService _kullaniciService;
        private readonly IUrunService _urunService;
        private readonly IUrunKategoriService _urunKategoriService;
        private readonly IRaporService _raporService;
        private readonly IYorumService _yorumService; // EKLENDİ


        public RestoranPanelController(
            ISiparisService siparisService,
            IRestoranService restoranService,
            IKullaniciService kullaniciService,
            IUrunService urunService,
            IUrunKategoriService urunKategoriService,
            IRaporService raporService,
            IYorumService yorumService) // EKLENDİ
        {
            _siparisService = siparisService;
            _restoranService = restoranService;
            _kullaniciService = kullaniciService;
            _urunService = urunService;
            _urunKategoriService = urunKategoriService;
            _raporService = raporService;
            _yorumService = yorumService; // ATANDI
        }

        // Yardımcı metod: Giriş yapan kullanıcının restoranını bul
        private Restoran? GetCurrentRestoran()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId)) return null;
            return _restoranService.TGet(x => x.KullaniciID == userId);
        }

        // ----------------- PANEL ANASAYFA (SİPARİŞ LİSTESİ) -----------------
        public IActionResult Index()
        {
            var restoran = GetCurrentRestoran();
            if (restoran == null) return Content("HATA: Kayıtlı bir restoranınız yok.");

            var siparisler = _siparisService.TGetList(x => x.RestoranID == restoran.RestoranID)
                                            .OrderByDescending(x => x.Tarih)
                                            .ToList();

            // 3. Admin panelindeki gibi ilişkili verileri doldurma (gerekirse)
            // Örn: Kullanıcı adını görmek istersen burada siparis.Kullanici = ... diyebilirsin.

            ViewBag.RestoranAd = restoran.RestoranAd; // Başlıkta göstermek için

            return View(siparisler);
        }

        // ----------------- DURUM GÜNCELLEME (POST) -----------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DurumGuncelle(int siparisId, int yeniDurum)
        {
            if (!Enum.IsDefined(typeof(SiparisDurumu), yeniDurum))
            {
                TempData["Hata"] = "Geçersiz sipariş durumu.";
                return RedirectToAction(nameof(Index));
            }

            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("GirisYap", "Kullanici");

            // 1. Mevcut siparişi çekip durumunu kontrol etmeliyiz
            // (Concurrency/Eşzamanlılık durumunda veri tutarlılığı için tekrar DB'den çekmek iyidir)
            var mevcutSiparis = _siparisService.TGet(x => x.SiparisID == siparisId);
            if (mevcutSiparis == null)
            {
                TempData["Hata"] = "Sipariş bulunamadı.";
                return RedirectToAction(nameof(Index));
            }
            
            // KURAL 1: Teslim edilmiş sipariş iptal edilemez
            if (mevcutSiparis.Durum == SiparisDurumu.TeslimEdildi && (SiparisDurumu)yeniDurum == SiparisDurumu.IptalEdildi)
            {
                TempData["Hata"] = "Müşteriye teslim edilmiş sipariş iptal edilemez!";
                return RedirectToAction(nameof(Index));
            }

            // KURAL 2: Zaten iptal edilmişse tekrar işlem yapma (Stok şişmesini önle)
            if (mevcutSiparis.Durum == SiparisDurumu.IptalEdildi && (SiparisDurumu)yeniDurum == SiparisDurumu.IptalEdildi)
            {
                TempData["Bilgi"] = "Bu sipariş zaten iptal edilmiş, işlem yapılmadı.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // 3. TRANSACTION (ATOMİK İŞLEM) BAŞLANGICI
                // Sipariş durumu değişimi ve ürün stok iadesi "ya hep ya hiç" mantığıyla çalışmalı.
                // Eğer stok güncellenirken hata olursa, sipariş durumu da eski haline döner.
                using (var transaction = new System.Transactions.TransactionScope())
                {

                    // STOK İADE MANTIĞI (Eğer İptal Ediliyorsa ve eski durum İptal değilse)
                    if ((SiparisDurumu)yeniDurum == SiparisDurumu.IptalEdildi && mevcutSiparis.Durum != SiparisDurumu.IptalEdildi)
                    {
                        var detaylar = _siparisService.GetSiparisDetaylariEntity(siparisId);
                        foreach(var item in detaylar)
                        {
                            var urun = _urunService.TGet(u => u.UrunId == item.UrunID);
                            if(urun != null)
                            {
                                urun.Stok += item.Adet;
                                _urunService.TUpdate(urun);
                            }
                        }
                    }

                    // Sipariş durumunu güncelle
                    _siparisService.SiparisDurumGuncelle(siparisId, (SiparisDurumu)yeniDurum, userId);
                    
                    // İşlemi onayla (Commit)
                    transaction.Complete(); 
                }
                // TRANSACTION BİTİŞİ

                TempData["Basarili"] = "Sipariş durumu güncellendi.";
            }
            catch (Exception ex)
            {
                TempData["Hata"] = "İşlem sırasında hata oluştu: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // ----------------- RESTORAN BİLGİLERİM (GET) -----------------
        [HttpGet]
        public IActionResult Bilgilerim()
        {
            var restoran = GetCurrentRestoran();
            if (restoran == null) return NotFound();
            return View(restoran);
        }

        // ----------------- RESTORAN BİLGİLERİM (POST) -----------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Bilgilerim(Restoran model)
        {
            var existing = GetCurrentRestoran();
            if (existing == null) return NotFound();

            existing.RestoranAd = model.RestoranAd;
            existing.Adres = model.Adres;
            existing.Telefon = model.Telefon;
            existing.MinSiparisTutar = model.MinSiparisTutar;

            _restoranService.TUpdate(existing);

            TempData["Basarili"] = "Bilgileriniz güncellendi.";
            return RedirectToAction(nameof(Bilgilerim));
        }

        // ----------------- SİPARİŞ DETAY SAYFASI (GET) -----------------
        [HttpGet]
        public IActionResult SiparisDetay(int id)
        {
            var restoran = GetCurrentRestoran();
            if (restoran == null) return RedirectToAction("GirisYap", "Kullanici");

            var siparis = _siparisService.TGet(x => x.SiparisID == id);

            if (siparis == null || siparis.RestoranID != restoran.RestoranID)
            {
                TempData["Hata"] = "Sipariş bulunamadı veya görüntüleme yetkiniz yok.";
                return RedirectToAction("Index");
            }

            var urunDetaylari = _siparisService.SiparisDetayGetir(id);

            var model = new RestoranSiparisDetayViewModel
            {
                Siparis = siparis,
                Urunler = urunDetaylari
            };

            return View(model);
        }

        // =================== ÜRÜN YÖNETİMİ ===================

        // ----------------- ÜRÜN LİSTELEME -----------------
        public IActionResult Urunlerim()
        {
            var restoran = GetCurrentRestoran();
            if (restoran == null) return RedirectToAction("GirisYap", "Kullanici");

            // Sadece kendi restoranının ürünlerini getir
            var urunler = _urunService.TGetList(x => x.RestoranID == restoran.RestoranID);

            // Kategori bilgisini doldur
            foreach (var u in urunler)
            {
                u.UrunKategori = _urunKategoriService.TGet(k => k.UrunKategoriID == u.UrunKategoriID);
            }

            return View(urunler);
        }

        // ----------------- ÜRÜN EKLE (GET) -----------------
        [HttpGet]
        public IActionResult UrunEkle()
        {
            var restoran = GetCurrentRestoran();
            if (restoran == null) return RedirectToAction("GirisYap", "Kullanici");

            // Ürün kategorileri dropdown
            ViewBag.UrunKategorileri = new SelectList(
                _urunKategoriService.TGetList(),
                "UrunKategoriID",
                "UrunKategoriAd"
            );

            return View();
        }

        // ----------------- ÜRÜN EKLE (POST) -----------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UrunEkle(Urun urun)
        {
            var restoran = GetCurrentRestoran();
            if (restoran == null) return RedirectToAction("GirisYap", "Kullanici");

            // RestoranID'yi otomatik ata (kullanıcı değiştiremesin)
            urun.RestoranID = restoran.RestoranID;
            urun.CreatedAt = DateTime.Now;
            urun.AktifMi = true;

            // ModelState'den RestoranID hatasını kaldır (biz atadık)
            ModelState.Remove("RestoranID");

            // LİMİT KONTROLÜ: Max 27 Ürün
            var mevcutUrunSayisi = _urunService.TGetList(x => x.RestoranID == restoran.RestoranID && x.AktifMi == true).Count;
            if (mevcutUrunSayisi >= 27)
            {
                ViewBag.Hata = "Maksimum ürün sayısına ulaştınız (27). Daha fazla ürün ekleyemezsiniz.";
                
                ViewBag.UrunKategorileri = new SelectList(
                    _urunKategoriService.TGetList(),
                    "UrunKategoriID",
                    "UrunKategoriAd"
                );
                return View(urun);
            }

            if (!ModelState.IsValid)
            {
                ViewBag.UrunKategorileri = new SelectList(
                    _urunKategoriService.TGetList(),
                    "UrunKategoriID",
                    "UrunKategoriAd"
                );
                return View(urun);
            }

            _urunService.TInsert(urun);
            TempData["Basarili"] = "Ürün başarıyla eklendi.";
            return RedirectToAction(nameof(Urunlerim));
        }

        // ----------------- ÜRÜN DÜZENLE (GET) -----------------
        [HttpGet]
        public IActionResult UrunDuzenle(int id)
        {
            var restoran = GetCurrentRestoran();
            if (restoran == null) return RedirectToAction("GirisYap", "Kullanici");

            var urun = _urunService.TGet(x => x.UrunId == id);

            // GÜVENLİK: Ürün bu restorana mı ait?
            if (urun == null || urun.RestoranID != restoran.RestoranID)
            {
                TempData["Hata"] = "Ürün bulunamadı veya düzenleme yetkiniz yok.";
                return RedirectToAction(nameof(Urunlerim));
            }

            ViewBag.UrunKategorileri = new SelectList(
                _urunKategoriService.TGetList(),
                "UrunKategoriID",
                "UrunKategoriAd",
                urun.UrunKategoriID
            );

            return View(urun);
        }

        // ----------------- ÜRÜN DÜZENLE (POST) -----------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UrunDuzenle(Urun urun)
        {
            var restoran = GetCurrentRestoran();
            if (restoran == null) return RedirectToAction("GirisYap", "Kullanici");

            // Mevcut ürünü bul
            var existing = _urunService.TGet(x => x.UrunId == urun.UrunId);

            // GÜVENLİK: Ürün bu restorana mı ait?
            if (existing == null || existing.RestoranID != restoran.RestoranID)
            {
                TempData["Hata"] = "Ürün bulunamadı veya düzenleme yetkiniz yok.";
                return RedirectToAction(nameof(Urunlerim));
            }

            // RestoranID'yi koruyoruz (kullanıcı değiştiremesin)
            urun.RestoranID = restoran.RestoranID;
            ModelState.Remove("RestoranID");

            if (!ModelState.IsValid)
            {
                ViewBag.UrunKategorileri = new SelectList(
                    _urunKategoriService.TGetList(),
                    "UrunKategoriID",
                    "UrunKategoriAd",
                    urun.UrunKategoriID
                );
                return View(urun);
            }

            // Sadece izin verilen alanları güncelle
            existing.UrunAd = urun.UrunAd;
            existing.Aciklama = urun.Aciklama;
            existing.Fiyat = urun.Fiyat;
            existing.Stok = urun.Stok;
            existing.UrunKategoriID = urun.UrunKategoriID;
            existing.AktifMi = urun.AktifMi;
            existing.FotoUrl = urun.FotoUrl;

            _urunService.TUpdate(existing);
            TempData["Basarili"] = "Ürün başarıyla güncellendi.";
            return RedirectToAction(nameof(Urunlerim));
        }

        // ----------------- ÜRÜN SİL (SOFT DELETE) -----------------
        public IActionResult UrunSil(int id)
        {
            var restoran = GetCurrentRestoran();
            if (restoran == null) return RedirectToAction("GirisYap", "Kullanici");

            var urun = _urunService.TGet(x => x.UrunId == id);

            // GÜVENLİK: Ürün bu restorana mı ait?
            if (urun == null || urun.RestoranID != restoran.RestoranID)
            {
                TempData["Hata"] = "Ürün bulunamadı veya silme yetkiniz yok.";
                return RedirectToAction(nameof(Urunlerim));
            }

            urun.AktifMi = false; // SOFT DELETE
            _urunService.TUpdate(urun);

            TempData["Basarili"] = "Ürün başarıyla silindi.";
            return RedirectToAction(nameof(Urunlerim));
        }


        public IActionResult Rapor()
        {
            var restoran = GetCurrentRestoran();
            if (restoran == null) return RedirectToAction("GirisYap", "Kullanici");

            var model = _raporService.GetUrunSatisOzeti(restoran.RestoranID)
                .OrderByDescending(x => x.UrunBazliKazanc)
                .ToList();

            return View(model);
        }

        // ----------------- YORUMLARIM (YENİ) -----------------
        [Authorize]
        public IActionResult Yorumlar()
        {
            var restoran = GetCurrentRestoran();
            if (restoran == null)
                return RedirectToAction("GirisYap", "Kullanici");

            var yorumlar = _yorumService.RestoranYorumlariGetir(restoran.RestoranID);

            return View(yorumlar);
        }

    }
}