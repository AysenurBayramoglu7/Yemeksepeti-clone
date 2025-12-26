using System.Collections.Generic;
using YemekSepeti.Entities;
using YemekSepeti.Entities.Dtos; // SiparisDetayDto burada olmalı

namespace YemekSepeti.WebUI.Models
{
    public class RestoranSiparisDetayViewModel
    {
        //Siparişin Genel Bilgileri (Tarih, Tutar, Adres, Müşteri Adı vb.)
        public Siparis Siparis { get; set; }
        // Siparişe Ait Ürün Detayları
        public List<SiparisDetayDto> Urunler { get; set; }
    }
}
