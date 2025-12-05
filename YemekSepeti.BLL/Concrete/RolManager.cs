using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using YemekSepeti.BLL.Abstract;
using YemekSepeti.DAL.Abstract;
using YemekSepeti.Entities;

namespace YemekSepeti.BLL.Concrete
{
    public class RolManager : IRolService
    {
        private readonly IRolDal _rolDal;

        public RolManager(IRolDal rolDal)
        {
            _rolDal = rolDal;
        }

        public void TDelete(Rol entity)
        {
            _rolDal.Delete(entity);
        }

        public Rol? TGet(Expression<Func<Rol, bool>> filter)
        {
            return _rolDal.Get(filter);
        }

        public List<Rol> TGetList(Expression<Func<Rol, bool>>? filter = null)
        {
            if (filter == null)
                return _rolDal.GetList();
            else
                return _rolDal.GetList(filter);
        }

        public void TInsert(Rol entity)
        {
            _rolDal.Insert(entity);
        }

        public void TUpdate(Rol entity)
        {
            _rolDal.Update(entity);
        }
    }
}
