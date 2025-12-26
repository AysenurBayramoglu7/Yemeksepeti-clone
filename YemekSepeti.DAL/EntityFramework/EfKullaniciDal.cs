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
    public class EfKullaniciDal : GenericRepository<Kullanici>, IKullaniciDal
    {

        public EfKullaniciDal(YemekSepetiDbContext context) : base(context)
        {
        }
        public Kullanici? GetUserByCredentials(string email, string sifre)
        { //Sadece kullanıcıyı alıp gelme, onun Rolünü de getir.
            return _context.Kullanicilar
                           .Include(k => k.Rol) // Rol bilgisini zorla yüklüyoruz
                           .SingleOrDefault(k => k.Email == email && k.Sifre == sifre);
        }
        
    }
}
