using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace YemekSepeti.BLL.Abstract
{
    public interface IGenericService<T> where T : class, new()
    {
        //DAl Generic in aynısı sadece başına karışmasın diye T ekliyoruz.
        void TInsert(T entity);
        void TUpdate(T entity);
        void TDelete(T entity);
        // 2. READ İşlemleri (Tüm filtreleme işlemleri burada gerçekleşir)

        // DAL'daki GetList metoduna karşılık gelir.
        List<T> TGetList(Expression<Func<T, bool>>? filter = null);

        // DAL'daki Get metoduna karşılık gelir.
        T? TGet(Expression<Func<T, bool>> filter);
    }
}
