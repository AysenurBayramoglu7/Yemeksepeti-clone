using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using YemekSepeti.DAL.Abstract;

namespace YemekSepeti.DAL.EntityFramework
{
    // Kısıtlamayı sizin arayüzünüzdeki gibi ayarlıyoruz.
    public class EfEntityRepositoryBase<T> : IGenericDal<T>
        where T : class, new() // <-- IGenericDal'a eklenecek tek kısıtlama.
    {
        // private → protected yapılıyor!
        protected readonly YemekSepetiDbContext _context;

        public EfEntityRepositoryBase(YemekSepetiDbContext context)
        {
            _context = context;
        }

        // --- CRUD İşlemleri (Sizin İsimlerinizle) ---

        // IGenericDal.Insert metodunu uyguluyor.
        public void Insert(T entity)
        {
            var addedEntity = _context.Entry(entity);
            addedEntity.State = EntityState.Added;
            //_context.SaveChanges();
            int affected = _context.SaveChanges();

            Console.WriteLine("AFECTED ROWS = " + affected);
        }

        // IGenericDal.Update metodunu uyguluyor.
        public void Update(T entity)
        {
            var updatedEntity = _context.Entry(entity);
            updatedEntity.State = EntityState.Modified;
            _context.SaveChanges();
        }

        // IGenericDal.Delete metodunu uyguluyor.
        public void Delete(T entity)
        {
            var deletedEntity = _context.Entry(entity);
            deletedEntity.State = EntityState.Deleted;
            _context.SaveChanges();
        }

        // --- READ İşlemleri (Sizin İsimlerinizle) ---

        // --- Ekstra Metotlar (Filtreleme kolaylığı için) ---

        // GetList'in eski mantığını geri getirelim (BLL'de filtreleme için çok önemlidir!)
        public List<T> GetList(Expression<Func<T, bool>>? filter = null)
        {
            return filter == null
                ? _context.Set<T>().ToList()
                : _context.Set<T>().Where(filter).ToList();
        }

        // Tek kayıt filtreleme (Email veya özel koşul ile bulma)
        public T? Get(Expression<Func<T, bool>> filter)
        {
            return _context.Set<T>().SingleOrDefault(filter);
        }
    }
}