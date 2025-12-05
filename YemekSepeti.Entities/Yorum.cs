using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YemekSepeti.Entities
{
    public class Yorum
    {
        public int YorumID { get; set; }
        public int RestoranID { get; set; }
        public virtual Restoran? Restoran { get; set; }
        public int KullaniciID { get; set; }
        public virtual Kullanici? Kullanici { get; set; }
        public int? UrunID { get; set; }
        public virtual Urun? Urun { get; set; }

        [Required]
        [MaxLength(500)]
        public string YorumMetni { get; set; } = string.Empty;
        [Range(1, 5)]//int kısmını küçültüyoruz
        public int Puan {  get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool AktifMi { get; set; } = true;
    }
}
