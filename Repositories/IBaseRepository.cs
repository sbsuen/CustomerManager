using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomerManager.Models;

namespace CustomerManager.Repositories
{
    interface IBaseRepository<T> where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAll();

        Task<T> GetById(long id);
        Task Add(T entity);
        Task Update(T entity);
        Task Delete(T entity);
        bool Exists(long id);
    }
}
