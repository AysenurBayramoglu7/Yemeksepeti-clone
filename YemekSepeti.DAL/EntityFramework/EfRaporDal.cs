using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using YemekSepeti.DAL.Abstract;
using YemekSepeti.Entities;
using YemekSepeti.Entities.Dtos;

namespace YemekSepeti.DAL.EntityFramework
{
    public class EfRaporDal : IRaporDal
    {
        private readonly YemekSepetiDbContext _context;

        public EfRaporDal(YemekSepetiDbContext context)
        {
            _context = context;
        }

        public List<UrunSatisRaporDto> GetUrunSatisOzeti(int restoranId)
        {
            //sanal tabloyu kullanarak raporu çekiyoruz. O yüzden dto kullanıyoruz.
            return _context.Set<UrunSatisRaporDto>()
                .FromSqlRaw(
                    "SELECT * FROM vw_RestoranUrunSatisOzeti WHERE RestoranID = @rid",
                    new SqlParameter("@rid", restoranId)
                )
                .ToList();
        }

        public int GetTeslimSiparisSayisi(int restoranId)
        {
            // Burda sqlQuery ile doğrudan fonksiyon çağrısı yapıyoruz.sonuç tek bir int değer döneceği için first ile alıyoruz.
            return _context.Database
                .SqlQuery<int>($"SELECT dbo.fn_RestoranTeslimSiparisSayisi({restoranId}) AS Value")
                .First();
        }

        public decimal GetToplamKazanc(int restoranId)
        {
            return _context.Database
                .SqlQuery<decimal>($"SELECT dbo.fn_RestoranToplamKazanc({restoranId}) AS Value")
                .First();
        }
    }
}

