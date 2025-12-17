using YemekSepeti.Entities;
using YemekSepeti.Entities.Dtos;

namespace YemekSepeti.WebUI.Models
{
    public class SiparisListesiViewModel
    {
        // Satırın görünen kısmı (Sipariş Özeti)
        public SiparisGecmisiDto Siparis { get; set; }

        // Tıklayınca açılacak kısım (SP'den gelen detaylar)
        public List<SiparisDetayDto> Detaylar { get; set; }
    }
}
