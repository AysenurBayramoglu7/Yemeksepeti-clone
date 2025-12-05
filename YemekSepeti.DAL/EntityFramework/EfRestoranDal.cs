using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YemekSepeti.DAL.Abstract;
using YemekSepeti.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using YemekSepeti.Entities;

namespace YemekSepeti.DAL.EntityFramework
{
    public class EfRestoranDal : GenericRepository<Restoran>, IRestoranDal
    {
        public EfRestoranDal(YemekSepetiDbContext context) : base(context)
        {
        }
    }
}
