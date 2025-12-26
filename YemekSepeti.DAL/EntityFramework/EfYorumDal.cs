using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YemekSepeti.DAL.Abstract;
using YemekSepeti.DAL.Repositories;
using YemekSepeti.Entities;
using Microsoft.EntityFrameworkCore;

namespace YemekSepeti.DAL.EntityFramework
{
    public class EfYorumDal : GenericRepository<Yorum>, IYorumDal
    {
        public EfYorumDal(YemekSepetiDbContext context) : base(context)
        {
        }

        // Restoran yorumlarını kullanıcı bilgileri ile getirir
        //EF Core ü<erinden veri çekiyoruz/ kullanarak yorumları RestoranID'ye göre filtreler,
        public List<Yorum> GetYorumlarByRestoran(int restoranId)
        {
            return _context.Yorumlar // context üzerinden Yorumlar tablosuna erişiyoruz
                .Include(y => y.Kullanici)
                .Where(y => y.RestoranID == restoranId && y.AktifMi == true)
                .OrderByDescending(y => y.CreatedAt)
                .ToList();
        }

        public List<Yorum> GetRestoranYorumlari(int restoranId)
        {
            return _context.Yorumlar // Restoran yorumlarını kullanıcı bilgileri ile getirir
                .Include(y => y.Kullanici) // Kullanıcı bilgisini çekmeyi unutmuyoruz
                .Where(x => x.RestoranID == restoranId && x.AktifMi == true)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();
        }

        public void TYorumEkleSP(Yorum yorum)
        {
            // Parametreler: @KullaniciID, @RestoranID, @SiparisID, @YorumMetni, @Puan
            
            var pKullanici = new Microsoft.Data.SqlClient.SqlParameter("@KullaniciID", yorum.KullaniciID);
            var pRestoran = new Microsoft.Data.SqlClient.SqlParameter("@RestoranID", yorum.RestoranID);
            var pSiparis = new Microsoft.Data.SqlClient.SqlParameter("@SiparisID", yorum.SiparisID);
            var pMetin = new Microsoft.Data.SqlClient.SqlParameter("@YorumMetni", yorum.YorumMetni ?? "");
            var pPuan = new Microsoft.Data.SqlClient.SqlParameter("@Puan", yorum.Puan);

            _context.Database.ExecuteSqlRaw("EXEC up_YorumEkle @KullaniciID, @RestoranID, @SiparisID, @YorumMetni, @Puan", 
                pKullanici, pRestoran, pSiparis, pMetin, pPuan);
        }

        public void TYorumGuncelleSP(Yorum yorum)
        {
            // Parametreler: @KullaniciID, @SiparisID, @YeniYorum, @YeniPuan

            var pKullanici = new Microsoft.Data.SqlClient.SqlParameter("@KullaniciID", yorum.KullaniciID);
            var pSiparis = new Microsoft.Data.SqlClient.SqlParameter("@SiparisID", yorum.SiparisID);
            var pYeniYorum = new Microsoft.Data.SqlClient.SqlParameter("@YeniYorum", yorum.YorumMetni ?? "");
            var pYeniPuan = new Microsoft.Data.SqlClient.SqlParameter("@YeniPuan", yorum.Puan);

            _context.Database.ExecuteSqlRaw("EXEC up_YorumGuncelle @KullaniciID, @SiparisID, @YeniYorum, @YeniPuan",
                pKullanici, pSiparis, pYeniYorum, pYeniPuan);
        }
    }
}
