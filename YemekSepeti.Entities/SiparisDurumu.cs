using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YemekSepeti.Entities
{
    public enum SiparisDurumu
    {
        OnayBekliyor = 0,   // Varsayılan Yeni Sipariş
        Hazirlaniyor = 1,   // Restoran Onayladı
        Yolda = 2,          // Kuryede
        TeslimEdildi = 3,   // Bitti
        IptalEdildi = 4     // İptal
    }
}
