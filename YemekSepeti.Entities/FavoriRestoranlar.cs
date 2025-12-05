using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YemekSepeti.Entities
{
    public class FavoriRestoranlar
    {
        // Kullanicinin favori yaptığı restoranın ID'si (PK parçası)
        public int KullaniciID { get; set; }

        // Restoranın ID'si (PK parçası)
        public int RestoranID { get; set; }

        // Navigasyon Özellikleri
        public virtual Kullanici? Kullanici { get; set; }
        public virtual Restoran? Restoran { get; set; }
    }
}
