using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using YemekSepeti.BLL.Abstract;// IKullaniciService'e erişim için
using YemekSepeti.Entities;
//using System; // try-catch için gerekli
//using YemekSepeti.BLL.Abstract; // IKullaniciService'e erişim için
//using YemekSepeti.BLL.Concrete; // KullaniciManager'a erişim için
//using YemekSepeti.Entities; // Kullanici modeline erişim için

namespace YemekSepeti.WebUI.Controllers
{
    // Controller'daki tüm metodlara erişimi sadece 'Admin' rolüyle sınırlandırır.
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IRestoranService _restoranService;
        private readonly IKategoriService _kategoriService;
        private readonly IKullaniciService _kullaniciService;
        private readonly IUrunService _urunService;
        private readonly IUrunKategoriService _urunKategoriService;
        private readonly IRolService _rolService;

        public AdminController(
            IRestoranService restoranService,
            IKategoriService kategoriService,
            IKullaniciService kullaniciService,
            IUrunService urunService,
            IUrunKategoriService urunKategoriService,
            IRolService rolService)
        {
            _restoranService = restoranService;
            _kategoriService = kategoriService;// Kategori Servisini tanımla
            _kullaniciService = kullaniciService;
            _urunService = urunService;
            _urunKategoriService = urunKategoriService;
            _rolService = rolService;
        }

        public IActionResult Index()
        {
            //sadece admin buraya erişebilir
            return View();
        }

        // ----------------- RESTORAN LİSTELEME -----------------
        public IActionResult RestoranListesi()
        {
            var restoranlar = _restoranService.TGetList();

            foreach (var r in restoranlar)
            {
                r.Kategori = _kategoriService.TGet(x => x.KategoriID == r.KategoriID);
            }

            return View(restoranlar);
        }

        // ----------------- RESTORAN EKLE (GET) -----------------
        //Bunları SelectListItem haline getirir → ViewBag’e atar.
         //View’de dropdown olarak görünür.
        [HttpGet]
        public IActionResult RestoranEkle()
        {
            var sahipler = _kullaniciService.TGetList()
                .Where(x => x.RolID == 2)
                .Select(x => new SelectListItem
                {
                    Value = x.KullaniciID.ToString(),
                    Text = x.Ad + " " + x.Soyad
                })
                .ToList();

            ViewBag.Sahipler = sahipler;

            ViewBag.Kategoriler = new SelectList(
                _kategoriService.TGetList(),
                "KategoriID",
                "KategoriAd"
            );

            return View();
        }

        // ----------------- RESTORAN EKLE (POST) -----------------
        //Formdan gelen restoran nesnesini alıyor
        [HttpPost]
        public IActionResult RestoranEkle(Restoran restoran)
        {
            if (ModelState.IsValid)
            {
                _restoranService.TInsert(restoran);
                return RedirectToAction(nameof(RestoranListesi));
            }

            // Dropdown tekrar dolduruluyor
            var sahipler = _kullaniciService.TGetList()
                .Where(x => x.RolID == 2)
                .Select(x => new SelectListItem
                {
                    Value = x.KullaniciID.ToString(),
                    Text = x.Ad + " " + x.Soyad
                })
                .ToList();

            ViewBag.Sahipler = sahipler;

            ViewBag.Kategoriler = new SelectList(
                _kategoriService.TGetList(),
                "KategoriID",
                "KategoriAd"
            );

            return View(restoran);
        }

        // ----------------- RESTORAN ONAYLAMA -----------------
        public IActionResult RestoranOnayla(int id)
        {
            var r = _restoranService.TGet(x => x.RestoranID == id);
            if (r == null) return NotFound();

            r.OnayliMi = true;
            _restoranService.TUpdate(r);

            return RedirectToAction(nameof(RestoranListesi));
        }

        // ----------------- RESTORAN DÜZENLE (GET) -----------------
        [HttpGet]
        public IActionResult RestoranDuzenle(int id)
        {
            var restoran = _restoranService.TGet(r => r.RestoranID == id);
            if (restoran == null) return NotFound();

            ViewBag.Kategoriler = new SelectList(
                _kategoriService.TGetList(),
                "KategoriID",
                "KategoriAd",
                restoran.KategoriID
            );

            return View(restoran);
        }

        // ----------------- RESTORAN DÜZENLE (POST) -----------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RestoranDuzenle(Restoran restoran)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Kategoriler = new SelectList(
                    _kategoriService.TGetList(),
                    "KategoriID",
                    "KategoriAd",
                    restoran.KategoriID
                );
                return View(restoran);
            }

            var existing = _restoranService.TGet(r => r.RestoranID == restoran.RestoranID);
            if (existing == null) return NotFound();

            existing.RestoranAd = restoran.RestoranAd;
            existing.Adres = restoran.Adres;
            existing.Telefon = restoran.Telefon;
            existing.KategoriID = restoran.KategoriID;
            existing.MinSiparisTutar = restoran.MinSiparisTutar;
            existing.OrtalamaSure = restoran.OrtalamaSure;

            _restoranService.TUpdate(existing);

            return RedirectToAction(nameof(RestoranListesi));
        }

        // ----------------- RESTORAN SİL -----------------
        public IActionResult RestoranSil(int id)
        {
            var restoran = _restoranService.TGet(x => x.RestoranID == id);
            if (restoran == null) return NotFound();

            _restoranService.TDelete(restoran);

            return RedirectToAction(nameof(RestoranListesi));
        }

        // ----------------- ÇIKIŞ -----------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // ----------------- KATEGORİ LİSTE -----------------
        public IActionResult KategoriListesi()
        {
            var kategoriler = _kategoriService.TGetList();
            return View(kategoriler);
        }

        // ----------------- KATEGORİ EKLE (GET) -----------------
        [HttpGet]
        public IActionResult KategoriEkle()
        {
            return View();
        }

        // ----------------- KATEGORİ EKLE (POST) -----------------
        [HttpPost]
        public IActionResult KategoriEkle(Kategori kategori)
        {
            if (ModelState.IsValid)
            {
                _kategoriService.TInsert(kategori);
                return RedirectToAction(nameof(KategoriListesi));
            }

            return View(kategori);
        }

        // ----------------- KATEGORİ DÜZENLE (GET) -----------------
        [HttpGet]
        public IActionResult KategoriDuzenle(int id)
        {
            var kategori = _kategoriService.TGet(x => x.KategoriID == id);
            if (kategori == null) return NotFound();

            return View(kategori);
        }

        // ----------------- KATEGORİ DÜZENLE (POST) -----------------
        [HttpPost]
        public IActionResult KategoriDuzenle(Kategori kategori)
        {
            if (ModelState.IsValid)
            {
                _kategoriService.TUpdate(kategori);
                return RedirectToAction(nameof(KategoriListesi));
            }

            return View(kategori);
        }

        // ----------------- KATEGORİ SİL -----------------
        public IActionResult KategoriSil(int id)
        {
            var kategori = _kategoriService.TGet(x => x.KategoriID == id);
            if (kategori == null) return NotFound();

            try
            {
                _kategoriService.TDelete(kategori);
            }
            catch
            {
                // Yabancı Anahtar Hatası yakalanırsa (Kategoriye bağlı Restoran varsa)
                TempData["Hata"] = "Bu kategoriye bağlı restoranlar olduğu için silinemez.";
            }

            return RedirectToAction(nameof(KategoriListesi));
        }
        //----------------- ÜRÜN LİSTELEME -----------------
        public IActionResult UrunListesi()
        {
            // TÜM ürünleri çek
            var urunler = _urunService.TGetList();
            //Her ürün için restoranı buldu (RestoranID → Restoran tablosu).
            // Restoran adını doldur (JOIN)
            foreach (var u in urunler)
            {
                u.Restoran = _restoranService.TGet(r => r.RestoranID == u.RestoranID);
                u.UrunKategori = _urunKategoriService.TGet(k => k.UrunKategoriID == u.UrunKategoriID);
            }
            return View(urunler);
        }
        // ----------------- ÜRÜN EKLE (GET) -----------------
        // Formu göstermek için
        //Restoranları listelemeliyiz(hangi restorana ürün ekleyeceği için)
        //Kategorileri listelemeliyiz
        //Boş formu göstermeliyiz
        [HttpGet]
        public IActionResult UrunEkle()
        {
            // Restoran listesi (Aktif olanlar)
            var restoranlar = _restoranService.TGetList(x => x.AktifMi == true);
            ViewBag.Restoranlar = new SelectList(restoranlar, "RestoranID", "RestoranAd");

            // Ürün Kategorileri (yeni dropdown)
            ViewBag.UrunKategorileri = new SelectList(
                _urunKategoriService.TGetList(),
                "UrunKategoriID",
                "UrunKategoriAd"
            );

            return View();
        }


        // ----------------- ÜRÜN EKLE (POST) -----------------
        [HttpPost]
        public IActionResult UrunEkle(Urun urun)
        {
            if (!ModelState.IsValid)
            {
                // Dropdownlar hata sonrası tekrar dolsun
                ViewBag.Restoranlar = new SelectList(_restoranService.TGetList(), "RestoranID", "RestoranAd");

                ViewBag.UrunKategorileri = new SelectList(
                    _urunKategoriService.TGetList(),
                    "UrunKategoriID",
                    "UrunKategoriAd"
                );

                return View(urun);
            }

            urun.CreatedAt = DateTime.Now;
            urun.AktifMi = true;

            _urunService.TInsert(urun);

            return RedirectToAction("UrunListesi");
        }
        // ----------------- ÜRÜN DÜZENLE (GET) -----------------
        [HttpGet]
        public IActionResult UrunDuzenle(int id)
        {
            var urun = _urunService.TGet(x => x.UrunId == id);

            // Restoran dropdownı
            var restoranlar = _restoranService.TGetList(x => x.AktifMi == true);
            ViewBag.Restoranlar = new SelectList(restoranlar, "RestoranID", "RestoranAd", urun.RestoranID);

            // Kategori dropdownı
            var kategoriler = _urunKategoriService.TGetList();
            ViewBag.Kategoriler = new SelectList(kategoriler, "UrunKategoriID", "UrunKategoriAd", urun.UrunKategoriID);

            return View(urun);
        }
        // ----------------- ÜRÜN DÜZENLE (POST) -----------------
        [HttpPost]
        public IActionResult UrunDuzenle(Urun urun)
        {
            if (!ModelState.IsValid)
            {
                // Eğer validasyon hata verirse dropdownlar kaybolmasın diye tekrar yükle
                var restoranlar = _restoranService.TGetList(x => x.AktifMi == true);
                ViewBag.Restoranlar = new SelectList(restoranlar, "RestoranID", "RestoranAd", urun.RestoranID);

                var kategoriler = _urunKategoriService.TGetList();
                ViewBag.Kategoriler = new SelectList(kategoriler, "UrunKategoriID", "UrunKategoriAd", urun.UrunKategoriID);

                return View(urun);
            }

            _urunService.TUpdate(urun);
            return RedirectToAction("UrunListesi");
        }
        // ----------------- ÜRÜN SİL (SOFT DELETE) -----------------
        public IActionResult UrunSil(int id)
        {
            var urun = _urunService.TGet(x => x.UrunId == id);

            if (urun == null)
                return NotFound();

            urun.AktifMi = false; // SOFT DELETE
            _urunService.TUpdate(urun);

            return RedirectToAction("UrunListesi");
        }

        // ----------------- KULLANICI LİSTELEME -----------------
        public IActionResult KullaniciListesi()
        {
            var kullanicilar = _kullaniciService.TGetList();

            // Rol adını dolduralım (isteğe bağlı)
            foreach (var k in kullanicilar)
            {
                k.Rol = _rolService.TGet(x => x.RolID == k.RolID);
            }

            return View(kullanicilar);
        }

        // ----------------- KULLANICI SİL (SOFT DELETE) -----------------
        public IActionResult KullaniciSil(int id)
        {
            var kullanici = _kullaniciService.TGet(x => x.KullaniciID == id);

            if (kullanici == null)
                return NotFound();
            kullanici.IsActive = false; // SOFT DELETE

            _kullaniciService.TUpdate(kullanici);

            return RedirectToAction("KullaniciListesi");
        }

    }
}
