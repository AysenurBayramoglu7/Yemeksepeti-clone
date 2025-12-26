using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YemekSepeti.BLL.Concrete;
using YemekSepeti.Entities;
using YemekSepeti.Entities.Dtos;


namespace YemekSepeti.BLL.Abstract
{
    public interface ISiparisService : IGenericService<Siparis>
    {
        //yeni metodlar 
        List<SiparisGecmisiDto> KullaniciSiparisGecmisiGetir(int kullaniciId);
        List<SiparisDetayDto> SiparisDetayGetir(int siparisId);
        List<SiparisDetay> GetSiparisDetaylariEntity(int siparisId); 
        void SiparisDurumGuncelle(
            int siparisId,
            SiparisDurumu yeniDurum,
            int restoranSahibiKullaniciId
        );
        
        void KullaniciSiparisIptal(int siparisId, int kullaniciId); 

    }
}
