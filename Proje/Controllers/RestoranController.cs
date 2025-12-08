using Microsoft.AspNetCore.Mvc;
using YemekSepeti.BLL.Abstract;
using YemekSepeti.Entities;
using YemekSepeti.WebUI.Models;

namespace YemekSepeti.WebUI.Controllers
{
    public class RestoranController : Controller
    {
        private readonly IRestoranService _restoranService;
        private readonly IUrunService _urunService;
        public RestoranController(IRestoranService restoranService, IUrunService urunService)
        {
            _restoranService = restoranService;
            _urunService = urunService;
        }
        //kategoriId'ye göre restoranları listeleme
        //sadece aktif ve onaylı restoranlar gösterilir
        public IActionResult Index(int kategoriId)
        {
            var restoranlar = _restoranService.TGetList(
                x => x.KategoriID == kategoriId && x.AktifMi == true && x.OnayliMi == true
            );

            return View(restoranlar);
        }
        //RESTORAN DETAY + ÜRÜNLER
        public IActionResult Detay(int id)
        {
            var restoran = _restoranService.TGet(x => x.RestoranID == id);

            if (restoran == null)
                return NotFound();

            var urunler = _urunService.TGetList(
                x => x.RestoranID == id && x.AktifMi == true
            );

            var model = new RestoranDetayViewModel
            {
                Restoran = restoran,
                Urunler = urunler
            };

            return View(model); // Artık Detay.cshtml bu View'i karşılayacak
        }
    }
}
