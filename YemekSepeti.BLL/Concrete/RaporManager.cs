using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using YemekSepeti.BLL.Abstract;
using YemekSepeti.DAL.Abstract;
using YemekSepeti.Entities;
using YemekSepeti.Entities.Dtos;

namespace YemekSepeti.BLL.Concrete
{
    public class RaporManager : IRaporService
    {
        private readonly IRaporDal _raporDal;

        public RaporManager(IRaporDal raporDal)
        {
            _raporDal = raporDal;
        }

        public List<UrunSatisRaporDto> GetUrunSatisOzeti(int restoranId)
            => _raporDal.GetUrunSatisOzeti(restoranId);

        public int GetTeslimSiparisSayisi(int restoranId)
            => _raporDal.GetTeslimSiparisSayisi(restoranId);

        public decimal GetToplamKazanc(int restoranId)
            => _raporDal.GetToplamKazanc(restoranId);
    }

}
