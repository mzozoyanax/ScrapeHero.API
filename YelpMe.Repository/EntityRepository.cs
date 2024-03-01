using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YelpMe.Domain;
using YelpMe.Interfaces.Repositories;

namespace YelpMe.Repository
{
    public class EntityRepository<T> : IEntityRepository<T> where T : BaseEntity
    {
        private readonly AppDbContext _appDbContext;
        private DbSet<T> _entities;

        public EntityRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            _entities = appDbContext.Set<T>();
        }

        public void Delete(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            _entities.Remove(entity);
            _appDbContext.SaveChanges();
        }
        public T Get(int Id)
        {
            return _entities.SingleOrDefault(c => c.Id == Id);
        }
        public IEnumerable<T> GetAll()
        {
            return _entities.AsEnumerable();
        }
        public void Add(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            _entities.Add(entity);
            _appDbContext.SaveChanges();
        }
        public void Remove(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            _entities.Remove(entity);
        }
        public void SaveChanges()
        {
            _appDbContext.SaveChanges();
        }
        public void Update(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            _entities.Update(entity);
            _appDbContext.SaveChanges();
        }
    }
}
