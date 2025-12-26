using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YemekSepeti.Entities.Dtos
{
    public class SiparisGecmisiDto
    {
        public int SiparisID { get; set; }
        public DateTime Tarih { get; set; }
        public decimal ToplamTutar { get; set; }
        public string TakipKodu { get; set; } = string.Empty;
        public string TeslimatAdresi { get; set; } = string.Empty;
        public int Durum { get; set; }
        public int UrunCesidiSayisi { get; set; }//Sp de count ile alındı.
    }
}
