using System;
using System.Linq;
using System.Linq.Expressions;
using GroceryStore.Core.Abstractions.Repositories.Base;
using GroceryStore.Core.Models.Base;
using Microsoft.EntityFrameworkCore;

namespace GroceryStore.DAL.Repositories.Base
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class, IBaseEntity
    {
        private readonly GroceryStoreDbContext _context;

        protected BaseRepository(GroceryStoreDbContext context)
        {
            _context = context;
        }

        public TEntity Add(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
            return entity;
        }

        public void Delete(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
        }

        public IQueryable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate)
        {
            return _context.Set<TEntity>().Where(predicate).AsQueryable();
        }

        public IQueryable<TEntity> GetAll()
        {
            return _context.Set<TEntity>().AsQueryable();
        }

        public TEntity GetById(object id)
        {
            return _context.Set<TEntity>().Find(id);
        }

        public TEntity Update(TEntity entity)
        {
            var local = _context.Set<TEntity>().Local.FirstOrDefault(entry => entry.Id.Equals(entity.Id));
            if (local != null) _context.Entry(local).State = EntityState.Detached;

            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
            return _context.Set<TEntity>().Find(entity.Id);
        }

        public void RefreshEntity(TEntity entity)
        {
            _context.Entry(entity).Reload();
        }
    }
}