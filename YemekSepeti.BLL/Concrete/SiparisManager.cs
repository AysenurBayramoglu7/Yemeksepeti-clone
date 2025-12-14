using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using YemekSepeti.BLL.Abstract;
using YemekSepeti.DAL.Abstract;
using YemekSepeti.Entities;
using YemekSepeti.Entities.Dtos;

namespace YemekSepeti.BLL.Concrete
{
    public class SiparisManager : ISiparisService
    {
        private readonly ISiparisDal _siparisDal;

        public SiparisManager(ISiparisDal siparisDal)
        {
            _siparisDal = siparisDal;
        }

        public void TDelete(Siparis t)
        {
            _siparisDal.Delete(t);
        }

        public Siparis TGet(Expression<Func<Siparis, bool>> filter)
        {
            return _siparisDal.Get(filter);
        }

        public List<Siparis> TGetList(Expression<Func<Siparis, bool>>? filter = null)
        {
            return _siparisDal.GetList(filter);
        }

        public void TInsert(Siparis t)
        {
            _siparisDal.Insert(t);
        }

        public void TUpdate(Siparis t)
        {
            _siparisDal.Update(t);
        }
        public List<SiparisGecmisiDto> KullaniciSiparisGecmisiGetir(int kullaniciId)
        {
            return _siparisDal.KullaniciSiparisGecmisiGetir(kullaniciId);
        }
        public List<SiparisDetayDto> SiparisDetayGetir(int siparisId)
        {
            return _siparisDal.SiparisDetayGetir(siparisId);
        }

    }
}
