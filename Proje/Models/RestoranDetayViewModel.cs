using System.Collections.Generic;
using YemekSepeti.Entities;


namespace YemekSepeti.WebUI.Models
{
    public class RestoranDetayViewModel
    {
        public Restoran Restoran { get; set; }
        public List<Urun> Urunler { get; set; }
    }
}
