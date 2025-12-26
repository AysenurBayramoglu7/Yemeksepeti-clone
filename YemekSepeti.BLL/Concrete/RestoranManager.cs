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

        // Dependency Injection 
        public RestoranManager(IRestoranDal restoranDal)
        {
            _restoranDal = restoranDal;
        }

        public void TDelete(Restoran entity)
        {
            //Soft Delete için gerekli iş kuralları 
            // Siparişler, Yorumlar gibi bağlı veriler olduğu için hard delete yerine soft delete tercih edilir.

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
            //Filtreye uyan tek bir kaydı döndür.
            return _restoranDal.Get(filter);
        }

        public List<Restoran> TGetList(Expression<Func<Restoran, bool>>? filter = null)
        {
            //Filtre varsa filtreyi uygulayarak tüm listeyi döndür.
            return _restoranDal.GetList(filter);
        }

        public void TInsert(Restoran entity)
        {
            //Restoran Adı Zorunlu Kontrolü
            if (string.IsNullOrWhiteSpace(entity.RestoranAd))
            {
                throw new Exception("Restoran adı boş bırakılamaz.");
            }

            // Varsayılan Değer Atama
            entity.OnayliMi = false; // Yeni kayıtlar varsayılan olarak onaysız başlar.
            entity.AktifMi = true; // Yeni kayıt aktif olarak eklenir.
            entity.CreatedAt = DateTime.Now; 

            //Veriyi veritabanına kaydet
            _restoranDal.Insert(entity);
        }

        public void TUpdate(Restoran entity)
        {
            //Güncellenmek istenen RestoranID kontrolü
            if (entity.RestoranID <= 0)
            {
                throw new Exception("Güncellenecek restoran ID'si geçersiz.");
            }
            // DAL ÇAĞRISI: Veriyi veritabanında güncelle
            _restoranDal.Update(entity);
        }

    }
}
