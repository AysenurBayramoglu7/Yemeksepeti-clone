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
        //Repository Tasarım Kalıbı

        /*private readonly IGenericDal<Kullanici> _kullaniciDal;

        // Constructor: Repository'i manuel olarak başlatıyoruz.
        // Normalde bu adım Dependency Injection ile yapılır.
        public KullaniciManager(IGenericDal<Kullanici> kullaniciDal)
        {
            // Gelen objeyi değişkenimize atıyoruz.
            _kullaniciDal = kullaniciDal;
        }*/
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
            // 1. Kural: E-posta zaten kayıtlı mı?
            // IKullaniciDal'ın Get metodu ile veritabanında aynı e-postaya sahip bir kullanıcı aranır.
            Kullanici? mevcutKullanici = _kullaniciDal.Get(k => k.Email == yeniKullanici.Email);

            if (mevcutKullanici != null)
            {
                // Eğer arama sonucunda bir kullanıcı bulunursa (mevcutKullanici boş değilse), hata fırlatılır.
                throw new Exception("Bu e-posta adresi zaten kullanılmaktadır. Lütfen farklı bir adres girin.");
            }

            // 2. Kural: Şifre uzunluğu kontrolü (Basit validasyon)
            if (string.IsNullOrEmpty(yeniKullanici.Sifre) || yeniKullanici.Sifre.Length < 6)
            {
                throw new Exception("Şifre en az 6 karakter olmalıdır.");
            }

            // 3. Kural: Varsayılan Rol (Müşteri) atama
            // Eğer RolID sıfır olarak geldiyse, kullanıcıya varsayılan Müşteri rolünü atıyoruz (RolID = 3 varsayımı).
            if (yeniKullanici.RolID == 0)
            {
                yeniKullanici.RolID = 3;
            }

            // 4. DAL Çağrısı: Tüm kurallar geçti, veriyi DAL'a gönder.
            // _kullaniciDal'ın Insert metodu çağrılır ve bu, EfEntityRepositoryBase'deki SaveChanges komutunu çalıştırır.
            _kullaniciDal.Insert(yeniKullanici);
        }

        public void TUpdate(Kullanici entity)
        {
            _kullaniciDal.Update(entity);
        }
        // KullaniciManager.cs içine eklenmeli:

        public Kullanici TLogin(string email, string sifre)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(sifre))
                throw new Exception("Email ve şifre boş bırakılamaz.");

            // 🔥 ARTIK Include ile Rol geliyor
            Kullanici? user = _kullaniciDal.GetUserByCredentials(email, sifre);

            if (user == null)
                throw new Exception("Hatalı e-posta veya şifre girdiniz.");

            return user;
        }
       
    }
}
