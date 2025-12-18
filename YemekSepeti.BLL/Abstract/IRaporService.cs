using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YemekSepeti.BLL.Concrete;
using YemekSepeti.Entities;
using YemekSepeti.Entities.Dtos;


namespace YemekSepeti.BLL.Abstract
{
    //generic kısmı crud işlemleri için kurulur bu yüzden burda yok
    public interface IRaporService 
    {
        List<UrunSatisRaporDto> GetUrunSatisOzeti(int restoranId);
        int GetTeslimSiparisSayisi(int restoranId);
        decimal GetToplamKazanc(int restoranId);
    }

}
