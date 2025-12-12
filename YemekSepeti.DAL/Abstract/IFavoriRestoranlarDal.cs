using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YemekSepeti.Entities;
using System.Collections.Generic;


namespace YemekSepeti.DAL.Abstract
{
    public interface IFavoriRestoranlarDal : IGenericDal<FavoriRestoranlar>
    {
        void FavoriEkle(int kullaniciId, int restoranId);
        void FavoriSil(int kullaniciId, int restoranId);
        List<Restoran> FavorileriGetir(int kullaniciId);
    }
}

