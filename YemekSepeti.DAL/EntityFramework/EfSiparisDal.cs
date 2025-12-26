using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using YemekSepeti.DAL.Abstract;
using YemekSepeti.DAL.Repositories;
using YemekSepeti.Entities;
using YemekSepeti.Entities.Dtos;

namespace YemekSepeti.DAL.EntityFramework
{
    public class EfSiparisDal : GenericRepository<Siparis>, ISiparisDal
    {
        public EfSiparisDal(YemekSepetiDbContext context) : base(context)
        {
        }

        public List<SiparisGecmisiDto> KullaniciSiparisGecmisiGetir(int kullaniciId)
        {
            var param = new SqlParameter("@KullaniciID", kullaniciId);

            return _context.Set<SiparisGecmisiDto>()
                .FromSqlRaw(
                    "EXEC up_KullaniciSiparisGecmisi @KullaniciID",
                    param
                )
                .ToList();
        }

        public List<SiparisDetayDto> SiparisDetayGetir(int siparisId)
        {
            //Parametreyi hazırla
            var pSiparisID = new SqlParameter("@SiparisID", siparisId);

            //SP'yi çalıştır (FromSqlRaw ile)
            // Bu kod veritabanına gider, SP'yi çalıştırır ve sonucu DTO listesine çevirir.
            return _context.Set<SiparisDetayDto>()
                           .FromSqlRaw("EXEC up_SiparisDetayGetir @SiparisID", pSiparisID)
                           .AsEnumerable() // Veriyi çekmek için
                           .ToList();
        }
        // Bu metot sipariş iptalinde ürünlerin stoklarını geri eklemek için kullanılacak.
        public List<SiparisDetay> GetSiparisDetaylariEntity(int siparisId)
        {
            return _context.Set<SiparisDetay>()
                           .Where(x => x.SiparisID == siparisId)
                           .ToList();
        }
    }
}
