using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YemekSepeti.Entities.Dtos
{
    public class RestoranSonuc
    {
        public int RestoranID { get; set; }
        public string RestoranAd { get; set; } = string.Empty;
        public string? RestoranResimUrl { get; set; }
        public bool AktifMi { get; set; }
    }
}
