using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YemekSepeti.Entities;

namespace YemekSepeti.BLL.Abstract
{
    public interface IYorumService : IGenericService<Yorum>
    {
        // Restoran yorumlarını getir (kullanıcı adı ile birlikte)
        List<Yorum> GetYorumlarByRestoran(int restoranId);
        List<Yorum> RestoranYorumlariGetir(int restoranId);
    }
}
