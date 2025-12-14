using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YemekSepeti.BLL.Abstract;
using System.Linq;
using YemekSepeti.Entities.Dtos;
using System;

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

            // SP Call
            var siparisler = _siparisService.KullaniciSiparisGecmisiGetir(userId);

            return View(siparisler);
        }

        // Sipariş detaylarını getirir. AJAX çağrısı için kullanılır.
        [HttpGet]
        public IActionResult DetayGetir(int siparisId)
        {
            Console.WriteLine("---------------------------------------------");
            Console.WriteLine("DEBUG /Siparis/DetayGetir CALLED");
            Console.WriteLine("SiparisID: " + siparisId);

            if (siparisId <= 0)
            {
                 Console.WriteLine("ERROR: Invalid ID");
                 return BadRequest();
            }

            try
            {
                var detaylar = _siparisService.SiparisDetayGetir(siparisId);
                Console.WriteLine($"Service Returned Data. Count: {(detaylar != null ? detaylar.Count : 0)}");
                
                if(detaylar != null) {
                    foreach(var d in detaylar) {
                        Console.WriteLine($"Item: {d.UrunAd} - {d.Adet} - {d.Fiyat}");
                    }
                }
                
                return Json(detaylar);
            }
            catch(Exception ex)
            {
                Console.WriteLine("EXCEPTION in Controller: " + ex.Message);
                return BadRequest("Server Error: " + ex.Message);
            }
        }
    }
}
