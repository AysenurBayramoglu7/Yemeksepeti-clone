using System.Collections.Generic;
using YemekSepeti.Entities;
using YemekSepeti.Entities.Dtos; // SiparisDetayDto burada olmalı

namespace YemekSepeti.WebUI.Models
{
    public class RestoranSiparisDetayViewModel
    {
        // 1. Siparişin Genel Bilgileri (Tarih, Tutar, Adres, Müşteri Adı vb.)
        // Bu veriler "Siparis" tablosundan gelir.
        public Siparis Siparis { get; set; }

        // 2. Siparişin İçindeki Ürünler (Hangi yemekten kaç tane?)
        // Bu veriler senin dediğin "SiparisDetaylari" tablosundan gelir.
        // Biz bunları "SiparisDetayDto" olarak taşıyoruz.
        public List<SiparisDetayDto> Urunler { get; set; }
    }
}
