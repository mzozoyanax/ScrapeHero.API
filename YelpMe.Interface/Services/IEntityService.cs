using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YelpMe.Domain;

namespace YelpMe.Interfaces.Services
{
    public interface IEntityService<T> where T : BaseEntity
    {
        IEnumerable<T> GetAll();
        T Get(int Id);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        void Remove(T entity);
    }
}
