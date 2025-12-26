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
        // Restoran yorumlarını getir 
        List<Yorum> GetYorumlarByRestoran(int restoranId);
        List<Yorum> GetRestoranYorumlari(int restoranId); // RESTORAN PANELİ İÇİN
        
        // SP ile yorum ekleme
        void TYorumEkleSP(Yorum yorum);
        // SP ile yorum güncelleme
        void TYorumGuncelleSP(Yorum yorum);
    }
}
