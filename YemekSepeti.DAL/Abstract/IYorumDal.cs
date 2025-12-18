using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YemekSepeti.Entities;

namespace YemekSepeti.DAL.Abstract
{
    public interface IYorumDal : IGenericDal<Yorum>
    {
        // Restoran yorumlarını getir (kullanıcı adı ile birlikte)
        List<Yorum> GetYorumlarByRestoran(int restoranId);
        List<Yorum> GetRestoranYorumlari(int restoranId); // RESTORAN PANELİ İÇİN
    }
}
