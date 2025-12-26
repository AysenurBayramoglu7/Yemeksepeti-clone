using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YemekSepeti.Entities.Dtos
{
    //sanal tablo için dto yani veri tabanında view kulladığımız için
    public class UrunSatisRaporDto
    {
        public int RestoranID { get; set; }
        public int UrunId { get; set; }
        public string UrunAd { get; set; }
        public int SiparisFrekansi { get; set; }
        public int ToplamSatilanAdet { get; set; }
        public decimal UrunBazliKazanc { get; set; }
    }
}
