using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YemekSepeti.Entities
{
    public class Kategori
    {
        public int KategoriID {  get; set; } // PK

        [Required]
        [MaxLength(100)]
        public string KategoriAd { get; set; } = string.Empty;
    }
}
