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
        private readonly YemekSepetiDbContext _context;

        public EfSiparisDal(YemekSepetiDbContext context) : base(context)
        {
            _context = context;
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
            // 1. Parametreyi hazırla
            var pSiparisID = new SqlParameter("@SiparisID", siparisId);

            // 2. SP'yi çalıştır (FromSqlRaw ile)
            // Bu kod veritabanına gider, SP'yi çalıştırır ve sonucu DTO listesine çevirir.
            return _context.Set<SiparisDetayDto>()
                           .FromSqlRaw("EXEC up_SiparisDetayGetir @SiparisID", pSiparisID)
                           .AsEnumerable() // Veriyi çekmek için
                           .ToList();
        }
    }
}
