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
        //yeni metod
        List<SiparisGecmisiDto> KullaniciSiparisGecmisiGetir(int kullaniciId);
        List<SiparisDetayDto> SiparisDetayGetir(int siparisId);
        void SiparisDurumGuncelle(
            int siparisId,
            SiparisDurumu yeniDurum,
            int restoranSahibiKullaniciId
        );

    }
}
