using JwtAuth.DataContext;
using JwtAuthModels.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace JwtAuth.Repository
{
    public class Repository<T> : IRepository<T> where T : EntityBase
    {
        private readonly JwtAuthDbContext _context;

        public Repository(JwtAuthDbContext context)
        {
            _context = context;
        }

        public virtual async Task<T> Get(Expression<Func<T, bool>> expression)
        {
            T entity = await _context.Set<T>().FirstOrDefaultAsync(expression);
            Detach(entity);
            return entity;
        }

        public virtual async Task<IEnumerable<T>> GetAll()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> expression)
        {
            return await _context.Set<T>().Where(expression).ToListAsync();
        }

        public virtual async Task Insert(T entity) {
            _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task Update(T entity) {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
        }
        
        public virtual async Task UpdateAttach(T entity, Action<T> changes)
        {
            _context.Set<T>().Attach(entity);
            changes.Invoke(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task Delete(T entity) {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task Delete(int id) {
            T entity = await Get(x => x.Id == id);
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        public void Detach(T entity)
        {
            _context.Entry(entity).State = EntityState.Detached;
        }
    }
}