using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System; // try-catch için gerekli
using System.Security.Claims;
using YemekSepeti.BLL.Abstract; // IKullaniciService'e erişim için
using YemekSepeti.BLL.Concrete; // KullaniciManager'a erişim için
using YemekSepeti.Entities; // Kullanici modeline erişim için

namespace YemekSepeti.WebUI.Controllers
{
    // [Neden]: Tüm kullanıcı işlemlerini (Kayıt, Giriş vb.) yönetecek trafik polisidir.
    public class KullaniciController : Controller
    {
        // 1. MANAGER SINIFINI TANIMLAMA
        private readonly IKullaniciService _kullaniciService;

        public KullaniciController(IKullaniciService kullaniciService)
        {
            _kullaniciService = kullaniciService;
        }

        // --- 2. Kayıt Formunu Gösterme (GET) ---
        // Kullanıcı tarayıcıda /Kullanici/KayitOl yazdığında çalışır.
        [HttpGet]
        public IActionResult KayitOl()
        {
            // [Neden]: Kayıt formunu döndürür.
            return View();
        }

        // --- 3. Veriyi İşleme (POST) ---
        // Kullanıcı formu doldurup POST ettiğinde çalışır.
        [HttpPost]
        public IActionResult KayitOl(Kullanici yeniKullanici)
        {
            // [Neden]: Eğer formda C# Model kısıtlamalarına uymayan veri varsa tekrar formu gösterir.
            if (!ModelState.IsValid)
            {
                return View(yeniKullanici);
            }

            // [Neden]: BLL'deki iş kurallarını (email, şifre) çalıştırmak için try-catch kullanılır.
            try
            {
                // BLL'deki tüm iş kurallarını aktive eder.
                _kullaniciService.TInsert(yeniKullanici);
                TempData["SuccessMessage"] = "Kayıt işlemi başarıyla tamamlandı! Giriş yapabilirsiniz.";
                // [Neden]: Başarılıysa kullanıcıyı Giriş sayfasına yönlendirir.
                return RedirectToAction("GirisYap", "Kullanici");
            }
            catch (Exception ex)
            {
                // [Neden]: BLL'den gelen hata mesajını (Örn: "Email zaten kayıtlı") View'a taşır.
                ModelState.AddModelError("", ex.Message);

                // Hata mesajıyla formu tekrar gösterir.
                return View(yeniKullanici);
            }
        }

        // --- Giriş Formunu Gösterme (GET) ---
        [HttpGet]
        public IActionResult GirisYap()
        {
            return View();
        }

        // KullaniciController.cs içinde

        [HttpPost]
        public async Task<IActionResult> GirisYap(string email, string sifre, string? returnUrl = null)
        {
            Kullanici? kullanici = null;
            try
            {
                kullanici = _kullaniciService.TLogin(email, sifre);
            }
            catch (Exception ex)
            {
                // Hata mesajını yakala ve kullanıcıya göster
                ModelState.AddModelError("", "Giriş başarısız: " + ex.Message);
                ViewData["ReturnUrl"] = returnUrl; 
                return View();
            }

            if (kullanici == null)
            {
                ModelState.AddModelError("", "Geçersiz email veya şifre.");
                ViewData["ReturnUrl"] = returnUrl;
                return View();
            }

            /*Giriş yapan kullanıcının kim olduğunu, hangi ID’ye sahip olduğunu ve hangi role ait olduğunu 
             ASP.NET Core tam olarak bu kod sayesinde biliyor*/
            // Kullanıcının rol bilgisinin null olmaması için EfKullaniciDal GetUserByCredentials ile Rol include edildi.
            var claims = new List<Claim>
            {
                // Kullanıcı bilgilerini Claim olarak ekle
                new Claim(ClaimTypes.Name, kullanici.Email ?? string.Empty),
                new Claim(ClaimTypes.NameIdentifier, kullanici.KullaniciID.ToString()),
                new Claim(ClaimTypes.Role, kullanici.Rol?.RolAd ?? "Musteri") // rol null ise default Musteri
            };

            var identity = new ClaimsIdentity(claims, "Cookies");
            var principal = new ClaimsPrincipal(identity);

            // Kullanıcıyı cookie ile kimlik doğrulama sistemi içine al
            //Bu Claim listesi kullanıcının tarayıcısına cookie olarak yazılıyor:
            await HttpContext.SignInAsync("Cookies", principal);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                 return Redirect(returnUrl);
            }

            // Rol bazlı yönlendirme (isteğe bağlı)
            if (kullanici.Rol != null && kullanici.Rol.RolAd == "Admin")
                return RedirectToAction("Index", "Admin");

            return RedirectToAction("Index", "Home");
        }

        // CikisYap metodu
        [HttpGet]
        public async Task<IActionResult> CikisYap()
        {
            await HttpContext.SignOutAsync("Cookies");
            return RedirectToAction("GirisYap");
        }
    }
}
