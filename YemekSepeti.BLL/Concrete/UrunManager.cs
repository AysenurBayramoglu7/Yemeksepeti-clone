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
//Bir entity’nin BLL (iş katmanı) kodu, o entity’nin iş kurallarına göre şekillenir.

namespace YemekSepeti.BLL.Concrete
{
    public class UrunManager : IUrunService
    {
        private readonly IUrunDal _urunDal;
        // Dependency Injection ile IUrunDal örneği alınıyor
        // Bu sayede UrunManager, veri erişim katmanına bağımlı hale geliyor
        public UrunManager(IUrunDal urunDal)
        {
            _urunDal = urunDal;
        }
        public void TDelete(Urun entity)
        {
            _urunDal.Delete(entity);
        }

        public Urun? TGet(Expression<Func<Urun, bool>> filter)
        {
            return _urunDal.Get(filter);
        }

        public List<Urun> TGetList(Expression<Func<Urun, bool>>? filter = null)
        {
           return _urunDal.GetList(filter);
        }

        public void TInsert(Urun entity)
        {
            _urunDal.Insert(entity);
        }

        public void TUpdate(Urun entity)
        {
            _urunDal.Update(entity);
        }
        //SP ile ürünleri restoranId'ye göre getirme
        public List<Urun> GetUrunlerByRestoranSP(int restoranId)
        {
            return _urunDal.GetUrunlerByRestoranSP(restoranId);
        }

    }
}
