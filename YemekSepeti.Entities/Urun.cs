using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YemekSepeti.Entities
{
    public class Urun
    {
        public int UrunId { get; set; }
        [Required]
        [MaxLength(150)]
        public string UrunAd { get; set; } = string.Empty;
        [MaxLength(250)]
        public string? Aciklama { get; set; }
        [Required]
        public decimal Fiyat {  get; set; }
        public int Stok {  get; set; }
        public bool AktifMi { get; set; } = true;
        public string? FotoUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int RestoranID { get; set; }
        public virtual Restoran? Restoran { get; set; }
        //ürünleri de kategorili yapıcaz
        public int UrunKategoriID { get; set; }      // FK
        public virtual UrunKategori? UrunKategori { get; set; }

        public virtual ICollection<Yorum> Yorumlar { get; set; } = new HashSet<Yorum>();
        public virtual ICollection<SiparisDetay> SiparisDetaylar { get; set; } = new HashSet<SiparisDetay>();

    }
}
