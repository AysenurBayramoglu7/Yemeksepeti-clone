using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YemekSepeti.DAL.Abstract;
using YemekSepeti.DAL.Repositories;
using YemekSepeti.Entities;

namespace YemekSepeti.DAL.EntityFramework
{
    public class EfFavoriRestoranlarDal : GenericRepository<FavoriRestoranlar>, IFavoriRestoranlarDal
    {
        public EfFavoriRestoranlarDal(YemekSepetiDbContext context) : base(context)
        {
        }
        public void FavoriEkle(int kullaniciId, int restoranId)
        {
            var p1 = new SqlParameter("@KullaniciID", kullaniciId);
            var p2 = new SqlParameter("@RestoranID", restoranId);

            _context.Database.ExecuteSqlRaw(
                "EXEC up_FavoriRestoranEkle @KullaniciID, @RestoranID",
                p1, p2
            );
        }

        public void FavoriSil(int kullaniciId, int restoranId)
        {
            var p1 = new SqlParameter("@KullaniciID", kullaniciId);
            var p2 = new SqlParameter("@RestoranID", restoranId);

            _context.Database.ExecuteSqlRaw(
                "EXEC up_FavoriRestoranSil @KullaniciID, @RestoranID",
                p1, p2
            );
        }

        public List<Restoran> FavorileriGetir(int kullaniciId)
        {
            var p = new SqlParameter("@KullaniciID", kullaniciId);

            return _context.Restoranlar
                .FromSqlRaw("EXEC up_FavoriRestoranlariGetir @KullaniciID", p)
                .ToList();
        }
    }
}
