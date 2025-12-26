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
    public class UrunKategoriManager : IUrunKategoriService
    {
        private readonly IUrunKategoriDal _urunKategoriDal;
        public UrunKategoriManager(IUrunKategoriDal urunKategoriDal)
        {
            _urunKategoriDal = urunKategoriDal;
        }

        public void TDelete(UrunKategori entity)
        {
            _urunKategoriDal.Delete(entity);
        }

        public UrunKategori? TGet(Expression<Func<UrunKategori, bool>> filter)
        {
            return _urunKategoriDal.Get(filter);
        }

        public List<UrunKategori> TGetList(Expression<Func<UrunKategori, bool>>? filter = null)
        {
            return _urunKategoriDal.GetList(filter);
        }

        public void TInsert(UrunKategori entity)
        {
            _urunKategoriDal.Insert(entity);
        }

        public void TUpdate(UrunKategori entity)
        {
            _urunKategoriDal.Update(entity);
        }
    }
}
