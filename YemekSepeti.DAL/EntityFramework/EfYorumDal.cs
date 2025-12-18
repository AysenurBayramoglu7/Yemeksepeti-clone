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
        public List<Yorum> GetYorumlarByRestoran(int restoranId)
        {
            return _context.Yorumlar
                .Include(y => y.Kullanici)
                .Where(y => y.RestoranID == restoranId && y.AktifMi == true)
                .OrderByDescending(y => y.CreatedAt)
                .ToList();
        }

        public List<Yorum> GetRestoranYorumlari(int restoranId)
        {
            return _context.Yorumlar
                .Include(y => y.Kullanici) // Kullanıcı bilgisini çekmeyi unutmuyoruz
                .Where(x => x.RestoranID == restoranId && x.AktifMi == true)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();
        }
    }
}
