using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YemekSepeti.Entities;
using YemekSepeti.Entities.Dtos;

namespace YemekSepeti.DAL.Abstract
{
    public interface ISiparisDal : IGenericDal<Siparis>
    {
        List<SiparisGecmisiDto> KullaniciSiparisGecmisiGetir(int kullaniciId);
        List<SiparisDetayDto> SiparisDetayGetir(int siparisId);
        List<SiparisDetay> GetSiparisDetaylariEntity(int siparisId); // EKLENDİ

    }
}
