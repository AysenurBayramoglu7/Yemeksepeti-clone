using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YemekSepeti.Entities;

namespace YemekSepeti.DAL.Abstract
{
    public interface IUrunDal : IGenericDal<Urun>
    {
        // Ürüne özel ek metodlar buraya yazılır
        List<Urun> GetUrunlerByRestoranSP(int restoranId);
    }
   
}
