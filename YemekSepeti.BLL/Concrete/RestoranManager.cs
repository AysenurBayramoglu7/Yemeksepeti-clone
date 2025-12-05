using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using YemekSepeti.BLL.Abstract;
using YemekSepeti.DAL.Abstract;
using YemekSepeti.Entities;

namespace YemekSepeti.BLL.Concrete
{
    public class RestoranManager : IRestoranService
    {
        private readonly IRestoranDal _restoranDal;

        // Dependency Injection (Bağımlılık Enjeksiyonu)
        public RestoranManager(IRestoranDal restoranDal)
        {
            _restoranDal = restoranDal;
        }

        public void TDelete(Restoran entity)
        {
            // İŞ KURALI: Soft Delete (Pasifleştirme) Uygulanması
            // Siparişler, Yorumlar gibi bağlı veriler olduğu için hard delete yerine pasifleştirme tercih edilir.

            var restoranToDelete = _restoranDal.Get(r => r.RestoranID == entity.RestoranID);

            if (restoranToDelete != null)
            {
                restoranToDelete.AktifMi = false; // Aktiflik durumunu pasife çek.
                _restoranDal.Update(restoranToDelete);
            }
            else
            {
                throw new Exception("Pasifleştirilmek istenen restoran bulunamadı.");
            }
        }

        public Restoran? TGet(Expression<Func<Restoran, bool>> filter)
        {
            // DAL ÇAĞRISI: Filtreye uyan tek bir kaydı döndür.
            return _restoranDal.Get(filter);
        }

        public List<Restoran> TGetList(Expression<Func<Restoran, bool>>? filter = null)
        {
            // DAL ÇAĞRISI: Filtre varsa filtreyi uygulayarak tüm listeyi döndür.
            return _restoranDal.GetList(filter);
        }

        public void TInsert(Restoran entity)
        {
            // İŞ KURALI 1: Restoran Adı Zorunlu Kontrolü
            if (string.IsNullOrWhiteSpace(entity.RestoranAd))
            {
                throw new Exception("Restoran adı boş bırakılamaz.");
            }

            // İŞ KURALI 2: Varsayılan Değer Atama (Admin onayı ve Aktiflik durumu)
            entity.OnayliMi = false; // Yeni kayıtlar varsayılan olarak onaysız başlar.
            entity.AktifMi = true; // Yeni kayıt aktif olarak eklenir.
            entity.CreatedAt = DateTime.Now; // Oluşturulma tarihi BLL'de set edilir.

            // DAL ÇAĞRISI: Veriyi veritabanına kaydet
            _restoranDal.Insert(entity);
        }

        public void TUpdate(Restoran entity)
        {
            // İŞ KURALI: Güncellenmek istenen RestoranID kontrolü
            if (entity.RestoranID <= 0)
            {
                throw new Exception("Güncellenecek restoran ID'si geçersiz.");
            }

            // Not: Eğer şifreleme/karma (hashing) gibi işlemler olsaydı, onlar da burada yapılırdı.

            // DAL ÇAĞRISI: Veriyi veritabanında güncelle
            _restoranDal.Update(entity);
        }
    }
}
