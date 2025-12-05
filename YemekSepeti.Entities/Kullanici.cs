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

        //yeni eklenen alanlar
        // EKLENEN ALANLAR
        [MaxLength(250)]
        public string? Adres { get; set; } // opsiyonel

        [MaxLength(15)]
        public string? Telefon { get; set; } // opsiyonel

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;

        //ilişkilendirmek için foreign key (FK), Rol tablosuna gidebilmek için
        //EF Core'a ve sizin C# kodunuza mantıksal ilişkiyi anlatır.
        //rol türünde bir property olmalı
        public int RolID { get; set; } // FK
        public virtual Rol? Rol { get; set; } // Navigation Property EF core için. 1'e N ilişkisi için
        //public object Siparisler { get; set; }
        public virtual ICollection<Siparis> Siparisler { get; set; } = new HashSet<Siparis>(); // deneme amaçlı
        public virtual ICollection<Yorum> Yorumlar {  get; set; } = new HashSet<Yorum>();
        public virtual ICollection<FavoriRestoranlar> FavoriRestoranlar { get; set; } = new HashSet<FavoriRestoranlar>();
    }
}
