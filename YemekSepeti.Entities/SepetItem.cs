using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YemekSepeti.Entities
{
    // Sepetteki tek bir ürünü temsil eden model
    public class SepetItem
    {
        public int UrunId { get; set; }
        public string UrunAd { get; set; } = string.Empty;
        public decimal Fiyat { get; set; }
        public int Adet { get; set; }
        
        public string? FotoUrl { get; set; }
        public int RestoranId { get; set; }
        
        // Birim fiyat ile adedin çarpımı sonucu toplam tutar
        public decimal Toplam => Fiyat * Adet;
    }
}
