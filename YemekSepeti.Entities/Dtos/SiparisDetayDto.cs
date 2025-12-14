using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YemekSepeti.Entities.Dtos
{
    public class SiparisDetayDto
    {
        public string UrunAd { get; set; } = string.Empty;

        public int Adet { get; set; }

        public decimal Fiyat { get; set; }

        public decimal SatirToplam { get; set; }
    }
}
