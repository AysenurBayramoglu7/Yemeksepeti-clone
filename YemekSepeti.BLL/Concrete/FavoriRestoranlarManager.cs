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
    public class FavoriRestoranlarManager : IFavoriRestoranlarService
    {
        private readonly IFavoriRestoranlarDal _favoriRestoranlarDal;
        public FavoriRestoranlarManager(IFavoriRestoranlarDal favoriRestoranlarDal)
        {
            _favoriRestoranlarDal = favoriRestoranlarDal;
        }
        public void FavoriEkle(int kullaniciId, int restoranId)
        {
            _favoriRestoranlarDal.FavoriEkle(kullaniciId, restoranId);
        }
        public void FavoriSil(int kullaniciId, int restoranId)
        {
            _favoriRestoranlarDal.FavoriSil(kullaniciId, restoranId);
        }
        public List<Restoran> FavorileriGetir(int kullaniciId)
        {
            return _favoriRestoranlarDal.FavorileriGetir(kullaniciId);
        }

        public void TInsert(FavoriRestoranlar entity)
        {
            throw new NotImplementedException();
        }

        public void TUpdate(FavoriRestoranlar entity)
        {
            throw new NotImplementedException();
        }

        public void TDelete(FavoriRestoranlar entity)
        {
            throw new NotImplementedException();
        }

        public List<FavoriRestoranlar> TGetList(Expression<Func<FavoriRestoranlar, bool>>? filter = null)
        {
            throw new NotImplementedException();
        }

        public FavoriRestoranlar? TGet(Expression<Func<FavoriRestoranlar, bool>> filter)
        {
            throw new NotImplementedException();
        }
    }     
}
