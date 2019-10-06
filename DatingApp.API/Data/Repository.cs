using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DatingApp.API.Data.RepositoryInterfaces;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        private readonly DataContext _context;

        public Repository(DataContext context)
        {
            _context = context;
        }
        public void Add(T entity)
        {
            _context.Set<T>().Add(entity);
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }
        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await _context.Set<T>().AsNoTracking().ToListAsync();
        }
        public async Task<IEnumerable<T>> GetAllWithInclude<P>(Expression<Func<T, P>> predicate)
        {
            return await _context.Set<T>().AsNoTracking().Include(predicate).ToListAsync();
        }
        public IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().Where(predicate);
        }
        public async Task<T> GetEntity(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }
        public async Task<T> GetEntityWithInclude<P>(Expression<Func<T, P>> predicate, Expression<Func<T, bool>> predicate2)
        {
            return await _context.Set<T>().Include(predicate).FirstOrDefaultAsync(predicate2);
        }
    }
}