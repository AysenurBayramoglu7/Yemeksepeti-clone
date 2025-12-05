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
    // EfKullaniciDal.cs dosyasını açın:
    public class EfKullaniciDal : GenericRepository<Kullanici>, IKullaniciDal
    {
        

        // [Neden]: Üst sınıfın zorunluluğu nedeniyle context'i alıp base'e iletiyoruz.
        // Bu, zorunlu bir C# dil kuralıdır.
        public EfKullaniciDal(YemekSepetiDbContext context) : base(context)
        {
            // Bu Constructor artık doğru çalışır.
        }
        public Kullanici? GetUserByCredentials(string email, string sifre)
        {
            // Context'i üst sınıftan gelen base çağrısı ile kullanın.
            // **Context'e direkt erişim sağlayarak Include kullanmak zorundayız.**

            return _context.Kullanicilar
                           .Include(k => k.Rol) // 🔑 Rol bilgisini zorla yüklüyoruz
                           .SingleOrDefault(k => k.Email == email && k.Sifre == sifre);
        }
        
        /*public Kullanici GetUserByCredentials(string email, string sifre)
        {
            return Get(k => k.Email == email && k.Sifre == sifre);
        }*/

        
    }
}

/*
 * Entity ye özel olmayan metotlar yazmam gerekebilir
 */