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
    public class EfUrunDal : GenericRepository<Urun>, IUrunDal
    {
        private readonly YemekSepetiDbContext _context;
        //Repository'nin görevi:
        //SQL yazmadan veritabanı işlemlerini yapabilmek
        //SP çalıştırmak
        //Insert, Update, Delete yapmak
        //Liste çekmek
        //Tüm bu işlemler için DbContext’e erişmek zorundayız.
        public EfUrunDal(YemekSepetiDbContext context) : base(context)
        {
            _context = context;
        }

        public List<Urun> GetUrunlerByRestoranSP(int restoranId)
        {
            var param = new SqlParameter("@RestoranID", restoranId);

            return _context.Urunler
                .FromSqlRaw("EXEC up_UrunleriRestoranaGoreGetir @RestoranID", param)
                .ToList();
        }

    }
}
