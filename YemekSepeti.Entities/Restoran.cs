using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YemekSepeti.Entities
{
    public class Restoran
    {
        public int RestoranID {  get; set; }
        public int KategoriID {  get; set; }
        public virtual Kategori? Kategori { get; set; } // 1 e N ilişkisi
        public int KullaniciID { get; set; }
        public virtual Kullanici? Kullanici { get; set; }

        [Required]
        [MaxLength(50)]
        public string RestoranAd {  get; set; } = string.Empty;

        [Required]
        [MaxLength(250)]
        public string Adres { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Telefon { get; set; }
        public bool OnayliMi { get; set; } = false;
        public decimal? Puan { get; set; }
        public bool AktifMi { get; set; } = true;
        public string? RestoranResimUrl { get; set; }
        public decimal MinSiparisTutar { get; set; }   // Örn: 150.00
        public int OrtalamaSure { get; set; }          // Örn: 30 (dakika)


        public DateTime CreatedAt { get; set; } = DateTime.Now;

        //Buraya da 1-N ilişkisinden dolayı koleksiyon navigasyonu koyuyoruz
        public virtual ICollection<Urun> Urunler { get; set; } = new HashSet<Urun>();
        public virtual ICollection<Siparis> Siparisler { get; set; } = new HashSet<Siparis>();
        public virtual ICollection<Yorum> Yorumlar { get; set; } = new HashSet<Yorum>();
        public virtual ICollection<FavoriRestoranlar> FavoriKullanicilar { get; set; } = new HashSet<FavoriRestoranlar>();

    }
}
