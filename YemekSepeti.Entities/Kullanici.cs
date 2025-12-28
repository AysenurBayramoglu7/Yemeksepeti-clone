using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YemekSepeti.Entities
{
    public class Kullanici
    {
        public int KullaniciID { get; set; } // Pirary key= Classla aynı isimde ve ID olduğunu belirttik

        // Hata mesajı daha hızlı gelsin diye sınırlamaları (özellik olarak) bi de buraya ekledik
        [Required] 
        [MaxLength(30)] 
        public string Ad { get; set; } = string.Empty;
        [Required]
        [MaxLength(30)] 
        public string Soyad { get; set; } = string.Empty;
        [Required]
        [MaxLength(50)] 
        public string Email { get; set; } = string.Empty;
        [Required]
        [MaxLength(500)]
        public string Sifre { get; set; } = string.Empty;

       
        [MaxLength(250)]
        public string? Adres { get; set; } 

        [MaxLength(15)]
        public string? Telefon { get; set; } 

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;

        //ilişkilendirmek için foreign key (FK), Rol tablosuna gidebilmek için
        public int RolID { get; set; } // FK
        public virtual Rol? Rol { get; set; } // Navigation Property . 1'e N ilişkisi için
        public virtual ICollection<Siparis> Siparisler { get; set; } = new HashSet<Siparis>(); 
        public virtual ICollection<Yorum> Yorumlar {  get; set; } = new HashSet<Yorum>();
        public virtual ICollection<FavoriRestoranlar> FavoriRestoranlar { get; set; } = new HashSet<FavoriRestoranlar>();
    }
}
