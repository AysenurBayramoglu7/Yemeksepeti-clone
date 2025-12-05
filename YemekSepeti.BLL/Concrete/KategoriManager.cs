using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using YemekSepeti.BLL.Abstract;
using YemekSepeti.DAL.Abstract;
using YemekSepeti.DAL.EntityFramework;
using YemekSepeti.Entities;

namespace YemekSepeti.BLL.Concrete
{
    public class KategoriManager : IKategoriService
    {
        private readonly IKategoriDal _kategoriDal;

        // Dependency Injection (Bağımlılık Enjeksiyonu)
        public KategoriManager(IKategoriDal kategoriDal)
        {
            _kategoriDal = kategoriDal;
        }

        public void TDelete(Kategori entity)
        {
            _kategoriDal.Delete(entity);
        }

        public Kategori? TGet(Expression<Func<Kategori, bool>> filter)
        {
            return _kategoriDal.Get(filter);
        }

        public List<Kategori> TGetList(Expression<Func<Kategori, bool>>? filter = null)
        {
            return _kategoriDal.GetList(filter);
        }

        public void TInsert(Kategori entity)
        {
            // İŞ KURALI: Kategori Adı Boş Olamaz
            if (string.IsNullOrWhiteSpace(entity.KategoriAd))
            {
                throw new Exception("Kategori adı boş geçilemez.");
            }
            _kategoriDal.Insert(entity);
        }

        public void TUpdate(Kategori entity)
        {
            _kategoriDal.Update(entity);
        }
    }
}
