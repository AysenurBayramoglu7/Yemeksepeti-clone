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
using YemekSepeti.Entities.Dtos;

namespace YemekSepeti.BLL.Concrete
{
    public class SiparisManager : ISiparisService
    {
        private readonly ISiparisDal _siparisDal;
        private readonly IRestoranDal _restoranDal;

        public SiparisManager(ISiparisDal siparisDal, IRestoranDal restoranDal)
        {
            _siparisDal = siparisDal;
            _restoranDal = restoranDal;
        }

        public void TDelete(Siparis t)
        {
            _siparisDal.Delete(t);
        }

        public Siparis TGet(Expression<Func<Siparis, bool>> filter)
        {
            return _siparisDal.Get(filter);
        }

        public List<Siparis> TGetList(Expression<Func<Siparis, bool>>? filter = null)
        {
            return _siparisDal.GetList(filter);
        }

        public void TInsert(Siparis t)
        {
            _siparisDal.Insert(t);
        }

        public void TUpdate(Siparis t)
        {
            _siparisDal.Update(t);
        }
        public List<SiparisGecmisiDto> KullaniciSiparisGecmisiGetir(int kullaniciId)
        {
            return _siparisDal.KullaniciSiparisGecmisiGetir(kullaniciId);
        }
        public List<SiparisDetayDto> SiparisDetayGetir(int siparisId)
        {
            return _siparisDal.SiparisDetayGetir(siparisId);
        }
        //Siparis iptali için siparis detaylarını entity olarak getir
        public List<SiparisDetay> GetSiparisDetaylariEntity(int siparisId)
        {
            return _siparisDal.GetSiparisDetaylariEntity(siparisId);
        }

        public void SiparisDurumGuncelle(
            int siparisId,
            SiparisDurumu yeniDurum,
            int restoranSahibiKullaniciId)
        {
            //Siparişi bul
            var siparis = _siparisDal.Get(x => x.SiparisID == siparisId);

            if (siparis == null)
                throw new Exception("Sipariş bulunamadı.");

            //Sipariş bu restoran sahibine mi ait kontrol edilir.
            // (RestoranID → Restoran → KullaniciID)
            var restoran = _restoranDal.Get(x =>
                x.RestoranID == siparis.RestoranID &&
                x.KullaniciID == restoranSahibiKullaniciId);

            if (restoran == null)
                throw new Exception("Bu sipariş üzerinde işlem yapma yetkiniz yok.");

            //Durum geçişi kontrolü olası tüm geçerli durumlar
            var mevcutDurum = (SiparisDurumu)siparis.Durum;

            bool gecerliGecis = (mevcutDurum == SiparisDurumu.OnayBekliyor &&yeniDurum == SiparisDurumu.Hazirlaniyor)
            || (mevcutDurum == SiparisDurumu.OnayBekliyor &&yeniDurum == SiparisDurumu.IptalEdildi)
            || (mevcutDurum == SiparisDurumu.Hazirlaniyor && yeniDurum == SiparisDurumu.IptalEdildi) 
            || (mevcutDurum == SiparisDurumu.Hazirlaniyor && yeniDurum == SiparisDurumu.Yolda)
            || (mevcutDurum == SiparisDurumu.Yolda && yeniDurum == SiparisDurumu.TeslimEdildi);

            if (!gecerliGecis)
                throw new Exception("Bu durum geçişi yapılamaz.");

            //Güncelle
            siparis.Durum = yeniDurum;
            _siparisDal.Update(siparis);

        }

        public void KullaniciSiparisIptal(int siparisId, int kullaniciId)
        {
            var siparis = _siparisDal.Get(x => x.SiparisID == siparisId);
            if (siparis == null) throw new Exception("Sipariş bulunamadı.");

            if (siparis.KullaniciID != kullaniciId)
                throw new Exception("Bu sipariş size ait değil.");

            // Sadece OnayBekliyor iptal edilebilir.
            if (siparis.Durum >= SiparisDurumu.Hazirlaniyor)
                throw new Exception("Hazırlanmaya başlanan sipariş iptal edilemez.");

            siparis.Durum = SiparisDurumu.IptalEdildi;
            _siparisDal.Update(siparis);
        }

    }
}
