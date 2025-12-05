using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace YemekSepeti.DAL.Abstract
{
    public interface IGenericDal <T> where T : class , new()
    { // T değeri sırasıyla bütün entity lerimiz olucak 
        void Insert (T entity);
        void Update (T entity);
        void Delete (T entity);
        // bunların haricindeki metotlar için entity ye özgü metotlar oluşturmalıyız
        // 🔑 GetList Metodu: GetAll'ın yerini alır ve filtre opsiyonu sunar.
        // Onaylanmış restoranları getirmek (r => r.OnayliMi == true) gibi esnek sorgular sağlar.
        List<T> GetList(Expression<Func<T, bool>>? filter = null);

        // 🔑 Get Metodu: GetById'ın yerini alır ve filtre opsiyonu sunar.
        // Bu metot, KullaniciManager'da hata aldığınız filtreli sorguyu destekler.
        T? Get(Expression<Func<T, bool>> filter);
    }
}
