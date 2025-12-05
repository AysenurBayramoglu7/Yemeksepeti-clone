using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YemekSepeti.DAL.Abstract;
using YemekSepeti.Entities;
using Microsoft.EntityFrameworkCore;
using YemekSepeti.DAL.Repositories;

namespace YemekSepeti.DAL.EntityFramework
{
    public class EfKategoriDal : GenericRepository<Kategori>, IKategoriDal
    {
        public EfKategoriDal(YemekSepetiDbContext context) : base(context)
        {
        }
    }
}
