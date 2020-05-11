using System;
using System.Linq;
using System.Linq.Expressions;

namespace GroceryStore.Core.Abstractions.Repositories.Base
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> GetAll();
        TEntity GetById(object id);
        IQueryable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate);
        TEntity Add(TEntity entity);
        TEntity Update(TEntity entity);
        void Delete(TEntity entity);
        void RefreshEntity(TEntity entity);
    }
}