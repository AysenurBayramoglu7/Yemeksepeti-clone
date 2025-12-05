using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YemekSepeti.Entities
{
    public class Rol
    {
        public int RolID { get; set; } // Primary key
        [MaxLength(15)]
        public string? RolAd { get; set; } //? koyunca rol altını çizmeyi bıraktı çünkü Null olabilir demek
        //her bir kullanıcının 1 rolu olucak ama bir rolde birden fazla kullanıcı olabilir.
        public ICollection<Kullanici> Kullanicilar { get; set; } = new HashSet<Kullanici>(); //Kullanıcıyı rolle ilişkili yaptım bu yüzden rol un de bundan haberi olmalı

    }
}
