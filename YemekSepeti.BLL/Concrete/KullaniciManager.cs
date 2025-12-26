using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YemekSepeti.BLL.Abstract;
using YemekSepeti.Entities;
using YemekSepeti.DAL.Abstract; // IKullaniciDal için
using YemekSepeti.DAL.EntityFramework;
using System.Linq.Expressions; // EfKullaniciDal için


namespace YemekSepeti.BLL.Concrete
{
    public class KullaniciManager : IKullaniciService
    {
        private readonly IKullaniciDal _kullaniciDal;

        public KullaniciManager(IKullaniciDal kullaniciDal)
        {
            _kullaniciDal = kullaniciDal;
        }

        public void TDelete(Kullanici entity)
        {
            _kullaniciDal.Delete(entity);
        }

        public Kullanici? TGet(Expression<Func<Kullanici, bool>> filter)
        {
            return _kullaniciDal.Get(filter);
        }

        public List<Kullanici> TGetList(Expression<Func<Kullanici, bool>>? filter = null)
        {
            return filter == null
            ? _kullaniciDal.GetList()
            : _kullaniciDal.GetList(filter);
        }
        //giriş yapmak için
        public void TInsert(Kullanici yeniKullanici)
        {
            // E-posta zaten kayıtlı mı kontrolü
            // IKullaniciDal'ın Get metodu ile veritabanında aynı e-postaya sahip bir kullanıcı aranır.
            Kullanici? mevcutKullanici = _kullaniciDal.Get(k => k.Email == yeniKullanici.Email);

            if (mevcutKullanici != null)
            {
                // Eğer arama sonucunda bir kullanıcı bulunursa (mevcutKullanici boş değilse), hata fırlatılır.
                throw new Exception("Bu e-posta adresi zaten kullanılmaktadır. Lütfen farklı bir adres girin.");
            }

            // Şifre uzunluğu kontrolü 
            if (string.IsNullOrEmpty(yeniKullanici.Sifre) || yeniKullanici.Sifre.Length < 6)
            {
                throw new Exception("Şifre en az 6 karakter olmalıdır.");
            }

            // Varsayılan Rol atama varsayılan olarak müşteri rolü atanır
            // Eğer RolID sıfır olarak geldiyse, kullanıcıya varsayılan Müşteri rolünü atıyoruz (RolID = 3 varsayımı).
            if (yeniKullanici.RolID == 0)
            {
                yeniKullanici.RolID = 3;
            }

            // Tüm kurallar geçtiyse veriyi DAL'a gönderiyoruz
            // _kullaniciDal'ın Insert metodu çağrılır ve bu, EfEntityRepositoryBase'deki SaveChanges komutunu çalıştırır.
            _kullaniciDal.Insert(yeniKullanici);
        }

        public void TUpdate(Kullanici entity)
        {
            _kullaniciDal.Update(entity);
        }

        public Kullanici TLogin(string email, string sifre)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(sifre))
                throw new Exception("Email ve şifre boş bırakılamaz.");

            // Artık kullanıcıyı veritabanında arıyoruz
            Kullanici? user = _kullaniciDal.GetUserByCredentials(email, sifre);

            if (user == null)
                throw new Exception("Hatalı e-posta veya şifre girdiniz.");

            return user;
        }
       
    }
}
