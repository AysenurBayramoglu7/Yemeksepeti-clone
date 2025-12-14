using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using YemekSepeti.DAL.Abstract;

namespace YemekSepeti.DAL.Repositories
{
    public class GenericRepository<T> : IGenericDal<T> where T : class, new()
    {
        // Okuma amaçlı, dışarıdan alınacak Context nesnesi
        // Bunu kullanmamızın nedeni Context de yapıcı içinde option koymayı zorunlu hale getirmemiz
        protected readonly YemekSepetiDbContext _context;
        private readonly DbSet<T> _dbSet; // Tekrar kullanmak için DbSet'i tanımlayalım

        // Constructor Injection (Bağımlılık Enjeksiyonu) ile Context'i alıyoruz
        public GenericRepository(YemekSepetiDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>(); // Hangi tip geldiyse o tablonun setini al
        }
        public void Delete(T entity)
        {
            // Durum takibi mekanizması
            //asıl amacı, EF Core'a bir nesnenin durumunu yönetme imkanı vermektir.
            var deletedEntity = _context.Entry(entity);
            deletedEntity.State = EntityState.Deleted;
            _context.SaveChanges();
        }

        public T? Get(Expression<Func<T, bool>> filter)
        {
            return _dbSet.AsNoTracking().FirstOrDefault(filter);
        }

        public List<T> GetList(Expression<Func<T, bool>>? filter = null)
        {
            return filter == null
         ? _dbSet.AsNoTracking().ToList()
         : _dbSet.AsNoTracking().Where(filter).ToList();
        }
        

        public void Insert(T entity)
        {
            _dbSet.Add(entity);
            _context.SaveChanges();
        }

        public void Update(T entity)
        {
            var updateEntity= _context.Entry(entity);
            updateEntity.State = EntityState.Modified;
            _context.SaveChanges();
        }
    }
}
