using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YemekSepeti.Entities;
using YemekSepeti.Entities.Dtos;

namespace YemekSepeti.DAL.Abstract
{
    public interface IRaporDal 
    {
        List<UrunSatisRaporDto> GetUrunSatisOzeti(int restoranId);
        int GetTeslimSiparisSayisi(int restoranId);
        decimal GetToplamKazanc(int restoranId);
    }
}
