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
    public class YorumManager : IYorumService
    {
        private readonly IYorumDal _yorumDal;

        // Dependency Injection
        public YorumManager(IYorumDal yorumDal)
        {
            _yorumDal = yorumDal;
        }

        public void TDelete(Yorum entity)
        {
            _yorumDal.Delete(entity);
        }

        public Yorum? TGet(Expression<Func<Yorum, bool>> filter)
        {
            return _yorumDal.Get(filter);
        }

        public List<Yorum> TGetList(Expression<Func<Yorum, bool>>? filter = null)
        {
            return _yorumDal.GetList(filter);
        }

        public void TInsert(Yorum entity)
        {
            // İş Kuralı: Puan 1-5 arasında olmalı
            if (entity.Puan < 1 || entity.Puan > 5)
            {
                throw new Exception("Puan 1 ile 5 arasında olmalıdır.");
            }

            // İş Kuralı: Yorum metni boş olamaz
            if (string.IsNullOrWhiteSpace(entity.YorumMetni))
            {
                throw new Exception("Yorum metni boş olamaz.");
            }

            _yorumDal.Insert(entity);
        }

        public void TUpdate(Yorum entity)
        {
            _yorumDal.Update(entity);
        }

        // Restoran yorumlarını getir
        public List<Yorum> GetYorumlarByRestoran(int restoranId)
        {
            return _yorumDal.GetYorumlarByRestoran(restoranId);
        }

        public List<Yorum> RestoranYorumlariGetir(int restoranId)
        {
             return _yorumDal.GetRestoranYorumlari(restoranId);
        }

        public void TYorumEkleSP(Yorum yorum)
        {
            // İş Kuralı: Puan 1-5 arasında olmalı
            if (yorum.Puan < 1 || yorum.Puan > 5)
            {
                throw new Exception("Puan 1 ile 5 arasında olmalıdır.");
            }

            _yorumDal.TYorumEkleSP(yorum);
        }

        public void TYorumGuncelleSP(Yorum yorum)
        {
             // İş Kuralı: Puan 1-5 arasında olmalı
            if (yorum.Puan < 1 || yorum.Puan > 5)
            {
                throw new Exception("Puan 1 ile 5 arasında olmalıdır.");
            }

            _yorumDal.TYorumGuncelleSP(yorum);
        }
    }
}
