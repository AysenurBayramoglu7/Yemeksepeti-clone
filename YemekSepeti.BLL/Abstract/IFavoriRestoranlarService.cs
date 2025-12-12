using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YemekSepeti.DAL.Abstract;
using YemekSepeti.Entities;
using System.Collections.Generic;

namespace YemekSepeti.BLL.Abstract
{
    public interface IFavoriRestoranlarService : IGenericService<FavoriRestoranlar>
    {
        void FavoriEkle(int kullaniciId, int restoranId);
        void FavoriSil(int kullaniciId, int restoranId);
        List<Restoran> FavorileriGetir(int kullaniciId);
    }

}
