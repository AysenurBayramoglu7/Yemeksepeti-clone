using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YemekSepeti.Entities
{
    public class FavoriRestoranlar
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FavoriID { get; set; }  
        // Kullanicinin favori yaptığı restoranın ID'si PK parçası
        public int KullaniciID { get; set; }

        // Restoranın ID'si PK parçası
        public int RestoranID { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigasyon Özellikleri
        public virtual Kullanici? Kullanici { get; set; }
        public virtual Restoran? Restoran { get; set; }
    }
}
