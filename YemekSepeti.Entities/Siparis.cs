using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YemekSepeti.Entities
{
    public class Siparis
    {
        public int SiparisID {  get; set; }
        public int RestoranID { get; set; }
        public virtual Restoran? Restoran { get; set; }
        public int KullaniciID { get; set; }
        public virtual Kullanici? Kullanici { get; set; }
        [Required]
        public decimal ToplamTutar {  get; set; }
        public bool AktifMi { get; set; } = true;
        [Required]
        [MaxLength(250)]
        public string TeslimatAdresi { get; set; } = string.Empty;

        public DateTime Tarih { get; set; } = DateTime.Now;
        [Required]
        [MaxLength(20)]
        public string TakipKodu { get; set; } = string.Empty; // Takip kodu
        public SiparisDurumu Durum { get; set; } = SiparisDurumu.OnayBekliyor;
        //public virtual ICollection<SiparisDetay> SiparisDetaylar { get; set; }
        public virtual ICollection<SiparisDetay> SiparisDetaylar { get; set; } = new HashSet<SiparisDetay>();

        public virtual ICollection<Yorum> Yorumlar { get; set; } = new HashSet<Yorum>();


    }
}
