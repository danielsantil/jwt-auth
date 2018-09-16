using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using JwtAuth.DataContext;
using JwtAuthModels.Data;
using Microsoft.EntityFrameworkCore;

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

        public virtual async Task<int> Count(Expression<Func<T, bool>> expression)
        {
            int count = await _context.Set<T>().CountAsync(expression);
            return count;
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

        public virtual void Detach(T entity)
        {
            _context.Entry(entity).State = EntityState.Detached;
        }

        public async Task Delete(Expression<Func<T, bool>> expression)
        {
            IEnumerable<T> list = await GetAll(expression);
            _context.Set<T>().RemoveRange(list);
            await _context.SaveChangesAsync();
        }
    }
}