using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace JwtAuth.Repository
{
    public interface IRepository<T>
    {
        //Task<T> Get(int id);
        Task<T> Get(Expression<Func<T, bool>> expression);
        Task<IEnumerable<T>> GetAll();
        Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> expression);
        Task Insert(T entity);
        Task Update(T entity);
        Task UpdateAttach(T entity, Action<T> changes);
        //Task Delete(T entity);
        //Task Delete(int id);
        void Detach(T entity);
    }
}
