
//HomeController'da kullanılacak ViewModel
// Home da tek bir model yetmediğinden dolayı ViewModel oluşturuyoruz.
using YemekSepeti.Entities;

namespace YemekSepeti.WebUI.Models
{
    public class HomeViewModel
    {
        public List<Kategori> Kategoriler { get; set; }
        public List<Restoran> Restoranlar { get; set; }
        public List<int> FavoriRestoranIdleri { get; set; } = new List<int>();

    }
}

