using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YemekSepeti.DAL.Abstract;
using YemekSepeti.DAL.Repositories;
using YemekSepeti.Entities;

namespace YemekSepeti.DAL.EntityFramework
{
    public class EfRolDal: GenericRepository<Rol> , IRolDal
    {
        public EfRolDal(YemekSepetiDbContext context) : base(context)
        {
        }

        
    }
}
