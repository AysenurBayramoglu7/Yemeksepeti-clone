using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// EF core ile veri tabanı bağlantısı sağlıyıcaz
using Microsoft.EntityFrameworkCore;
using YemekSepeti.Entities;
using YemekSepeti.Entities.Dtos;


namespace YemekSepeti.DAL
{   // Veri tabanına yansıtılmak istenen tüm sınıflar burda olucak
    public class YemekSepetiDbContext : DbContext
    {
        /*public YemekSepetiDbContext()
        {
        }*/

        public YemekSepetiDbContext(DbContextOptions<YemekSepetiDbContext> options) : base(options)
        {
        }

        //DbSet'ler oluştur yani Sql de tablo olmasını istediğin şeyler
        // <C# tarafında kullanıcak olduğumuz sınıfın ismi> , Kullaniciler ise SQL e yansıyacak olan tablo ismi
        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<Rol> Roller { get; set; }
        public DbSet<Kategori> Kategoriler { get; set; }
        public DbSet<Restoran> Restoranlar { get; set; }
        public DbSet<Siparis> Siparisler { get; set; }
        public DbSet<Urun> Urunler { get; set; }
        public DbSet<Yorum> Yorumlar { get; set; }
        public DbSet<SiparisDetay> SiparisDetaylari { get; set; }
        public DbSet<UrunKategori> UrunKategoriler { get; set; }
        public DbSet<FavoriRestoranlar> FavoriRestoranlar { get; set; }

        public DbSet<SiparisDetayDto> SiparisDetayDtos { get; set; }
        public DbSet<SiparisGecmisiDto> SiparisGecmisiDtos { get; set; } 
        public DbSet<UrunSatisRaporDto> UrunSatisRaporDtos { get; set; }
        public DbSet<RestoranSonuc> RestoranSonuc { get; set; }




        // YemekSepetiDbContext.cs içinde OnModelCreating metodunun sonuna ekleyin

        // [Neden]: Roller tablosuna başlangıç (seed) verilerini ekliyoruz.
        // Bu, KullaniciManager'ın (RolID = 3) hatasız çalışmasını garanti eder.


        // ❗ İLİŞKİ AYARLARI BURAYA GELECEK
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Siparis - Restoran (cascade kapatma)
            modelBuilder.Entity<Siparis>()
                .HasOne(s => s.Restoran)
                .WithMany(r => r.Siparisler)
                .HasForeignKey(s => s.RestoranID)
                .OnDelete(DeleteBehavior.Restrict);

            // Siparis - Kullanici (cascade kapatma)
            modelBuilder.Entity<Siparis>()
                .HasOne(s => s.Kullanici)
                .WithMany(k => k.Siparisler)
                .HasForeignKey(s => s.KullaniciID)
                .OnDelete(DeleteBehavior.Restrict);
            //Entity Framework Core'un (EF Core) varsayılan çoğul adlandırma 
            // Yorum - Kullanici İlişkisi
            modelBuilder.Entity<Yorum>()
                .HasOne(y => y.Kullanici)
                .WithMany(k => k.Yorumlar) // Kullanici.cs'e Yorumlar eklendi
                .HasForeignKey(y => y.KullaniciID)
                .OnDelete(DeleteBehavior.Restrict); // Kısıtla

            // Yorum - Restoran İlişkisi
            modelBuilder.Entity<Yorum>()
                .HasOne(y => y.Restoran)
                .WithMany(r => r.Yorumlar)
                .HasForeignKey(y => y.RestoranID)
                .OnDelete(DeleteBehavior.Restrict); // Kısıtla

            // Yorum - Urun İlişkisi
            modelBuilder.Entity<Yorum>()
                .HasOne(y => y.Urun)
                .WithMany(u => u.Yorumlar) // Urun.cs'e Yorumlar eklendi
                .HasForeignKey(y => y.UrunID)
                .OnDelete(DeleteBehavior.Restrict); // Kısıtla
                                                    // SiparisDetay - Siparis İlişkisi
            modelBuilder.Entity<SiparisDetay>()
                .HasOne(sd => sd.Siparis)
                .WithMany(s => s.SiparisDetaylar)
                .HasForeignKey(sd => sd.SiparisID)
                .OnDelete(DeleteBehavior.Restrict); // Kısıtla

            // SiparisDetay - Urun İlişkisi
            modelBuilder.Entity<SiparisDetay>()
                .HasOne(sd => sd.Urun)
                .WithMany(u => u.SiparisDetaylar)
                .HasForeignKey(sd => sd.UrunID)
                .OnDelete(DeleteBehavior.Restrict); // Kısıtla

            // ❗ BURAYA EKLENİYOR
            modelBuilder.Entity<Restoran>().ToTable("Restoran");

            // Trigger kullanan tabloları EF Core'a bildiriyoruz ki trigger'lar çalışsın
            //YENİ EKLENDİİİİİ
            // SiparisDetay tablosunun adını belirt (Hata Çözümü)
            modelBuilder.Entity<SiparisDetay>().ToTable("SiparisDetaylari");
            modelBuilder.Entity<SiparisDetay>().ToTable(tb => tb.HasTrigger("tr_SiparisDetay_StokDus"));
            modelBuilder.Entity<Urun>().ToTable(tb => tb.HasTrigger("tr_Urun_StokKontrol"));

            // Yorum tablosu için Trigger Bildirimi (HATA ÇÖZÜMÜ)
            modelBuilder.Entity<Yorum>().ToTable("Yorumlar"); // Tablo adını da garantiye alalım
            modelBuilder.Entity<Yorum>().ToTable(tb => tb.HasTrigger("trg_RestoranPuanGuncelle")); // Trigger olduğu için bunu eklemeliyiz.

            // Bu satır da eklenmeli! Siparis tablosu için de aynı sorun yaşanıyordu.
            modelBuilder.Entity<Siparis>().ToTable("Siparis");

            // YemekSepetiDbContext.cs içine OnModelCreating metodunun içine eklenmeli

            // Fiyat Alanları
            modelBuilder.Entity<Siparis>().Property(s => s.ToplamTutar).HasPrecision(18, 2);
            modelBuilder.Entity<SiparisDetay>().Property(sd => sd.Fiyat).HasPrecision(18, 2);
            modelBuilder.Entity<Urun>().Property(u => u.Fiyat).HasPrecision(18, 2);

            // Puan Alanı
            modelBuilder.Entity<Restoran>().Property(r => r.Puan).HasPrecision(3, 2); // 0.00 ile 9.99 arası puanlama için
            modelBuilder.Entity<Restoran>().Property(r => r.MinSiparisTutar).HasColumnType("decimal(18,2)");


            modelBuilder.Entity<Rol>().HasData(
                 new Rol { RolID = 1, RolAd = "Admin" },
                new Rol { RolID = 2, RolAd = "RestoranSahibi" },
                new Rol { RolID = 3, RolAd = "Musteri" }
            );

            //FAVORİ AYARLARI
            modelBuilder.Entity<FavoriRestoranlar>()
                .HasKey(fr => fr.FavoriID);

            // Çifte kaydı engellemek için Unique Index
            modelBuilder.Entity<FavoriRestoranlar>()
                .HasIndex(fr => new { fr.KullaniciID, fr.RestoranID })
                .IsUnique();

            modelBuilder.Entity<FavoriRestoranlar>()
                .HasOne(fr => fr.Kullanici)
                .WithMany(k => k.FavoriRestoranlar)
                .HasForeignKey(fr => fr.KullaniciID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FavoriRestoranlar>()
                .HasOne(fr => fr.Restoran)
                .WithMany(r => r.FavoriKullanicilar)
                .HasForeignKey(fr => fr.RestoranID)
                .OnDelete(DeleteBehavior.Restrict);
           
            // 🔹 SP DTO (Keyless Entity)
            modelBuilder.Entity<SiparisGecmisiDto>().HasNoKey();// Keyless entity olarak tanımlanır.YAni tablo oluşturulmaz.
            modelBuilder.Entity<SiparisDetayDto>().HasNoKey();
            modelBuilder.Entity<UrunSatisRaporDto>().HasNoKey();
            modelBuilder.Entity<RestoranSonuc>().HasNoKey();



        }
    }
}
