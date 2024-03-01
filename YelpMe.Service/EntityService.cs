using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YelpMe.Domain;
using YelpMe.Domain.Models;
using YelpMe.Interfaces.Repositories;
using YelpMe.Interfaces.Services;

namespace YelpMe.Services
{
    public class EntityService<T> : IEntityService<T> where T : BaseEntity
    {
        private readonly IEntityRepository<T> _repo;

        public EntityService(IEntityRepository<T> repo)
        {
            _repo = repo;
        }

        public void Add(T entity)
        {
            _repo.Add(entity);
        }

        public void Delete(T entity)
        {
            _repo.Delete(entity);
        }

        public T Get(int Id)
        {
            return _repo.Get(Id);   
        }

        public IEnumerable<T> GetAll()
        {
            return _repo.GetAll();
        }

        public void Remove(T entity)
        {
            _repo.Remove(entity);
        }

        public void Update(T entity)
        {
            _repo.Update(entity);
        }
    }
}
