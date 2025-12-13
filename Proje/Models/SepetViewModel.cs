using System.Collections.Generic;
using System.Linq;
using YemekSepeti.Entities;

namespace YemekSepeti.WebUI.Models
{
    // Sepet sayfasında (View) kullanılacak hepsi bir arada model.
    // Hem ürün listesini hem de genel toplamı tutar.
    public class SepetViewModel
    {
        public List<SepetItem> SepetItems { get; set; } = new List<SepetItem>();
        
        // Tüm ürünlerin toplam fiyatını hesaplar
        public decimal GenelToplam => SepetItems.Sum(x => x.Toplam);
        
        // Sepetteki toplam ürün adedi (Navbar için)
        public int ToplamAdet => SepetItems.Sum(x => x.Adet);
    }
}
