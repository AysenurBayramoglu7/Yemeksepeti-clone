using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YemekSepeti.Entities
{
    public class SiparisDetay
    {
        public int SiparisDetayID { get; set; }

        public int SiparisID { get; set; }
        public virtual Siparis? Siparis { get; set; } // Sipariş Navigasyonu

        public int UrunID { get; set; }
        public virtual Urun? Urun { get; set; } // Ürün Navigasyonu

        [Required]
        [Range(1, int.MaxValue)] // Adet en az 1 olmalıdır
        public int Adet { get; set; }

        [Required]
        public decimal Fiyat { get; set; } // Ürünün sipariş anındaki fiyatı
    }
}
