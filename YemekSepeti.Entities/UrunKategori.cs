using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace YemekSepeti.Entities
{
    public class UrunKategori
    {
        public int UrunKategoriID { get; set; } // PK

        [Required]
        [MaxLength(100)]
        public string UrunKategoriAd { get; set; } = string.Empty;
        // 1 kategori → N ürün
        public virtual ICollection<Urun> Urunler { get; set; } = new HashSet<Urun>();
    }
}
