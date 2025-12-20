using Microsoft.AspNetCore.Mvc;
using YemekSepeti.BLL.Abstract;
using YemekSepeti.Entities;
using YemekSepeti.WebUI.Models;

using System.Security.Claims; // ClaimTypes için gerekli

namespace YemekSepeti.WebUI.Controllers
{
    public class RestoranController : Controller
    {
        private readonly IRestoranService _restoranService;
        private readonly IUrunService _urunService;
        private readonly IUrunKategoriService _urunkategoriService;
        private readonly IYorumService _yorumService;
        private readonly IFavoriRestoranlarService _favoriService; // Yeni eklendi

        public RestoranController(
            IRestoranService restoranService, 
            IUrunService urunService, 
            IUrunKategoriService urunkategoriService,
            IYorumService yorumService,
            IFavoriRestoranlarService favoriService) // Constructor güncellendi
        {
            _restoranService = restoranService;
            _urunService = urunService;
            _urunkategoriService = urunkategoriService;
            _yorumService = yorumService;
            _favoriService = favoriService;
        }

        //kategoriId'ye göre restoranları listeleme
        //sadece aktif ve onaylı restoranlar gösterilir
        public IActionResult Index(int kategoriId)
        {
            var restoranlar = _restoranService.TGetList(
                x => x.KategoriID == kategoriId && x.AktifMi == true && x.OnayliMi == true
            );

            // Favori bilgilerini ViewBag ile taşıyalım (Modeli değiştirmeden çözüm)
            // Bu sayede "eski haline" en yakın yapıyı koruruz ama fonksiyonellik çalışır.
            if (User.Identity.IsAuthenticated)
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null)
                {
                    int userId = int.Parse(userIdClaim.Value);
                    var favoriRestoranlar = _favoriService.FavorileriGetir(userId);
                    ViewBag.FavoriRestoranIdleri = favoriRestoranlar.Select(x => x.RestoranID).ToList();
                }
            }

            return View(restoranlar);
        }
        //RESTORAN DETAY + ÜRÜNLER
        public IActionResult Detay(int id)
        {
            var restoran = _restoranService.TGet(x => x.RestoranID == id);

            if (restoran == null)
                return NotFound();

            /*var urunler = _urunService.TGetList(
                x => x.RestoranID == id && x.AktifMi == true
            );*///SP eklenicek
            var urunler = _urunService.GetUrunlerByRestoranSP(id);


            // Kategorileri ürünlere göre alıyoruz
            var kategoriIdList = urunler
                .Select(x => x.UrunKategoriID)
                .Distinct()
                .ToList();

            var kategoriler = _urunkategoriService.TGetList(
             x => kategoriIdList.Contains(x.UrunKategoriID)
             )
            .OrderBy(x => x.SiraNo)
    .       ToList();

            // Restoran yorumlarını getir
            var yorumlar = _yorumService.GetYorumlarByRestoran(id);

            var model = new RestoranDetayViewModel
            {
                Restoran = restoran,
                Urunler = urunler,
                Kategoriler = kategoriler,
                Yorumlar = yorumlar
            };

            return View(model);
        }


    }
}
