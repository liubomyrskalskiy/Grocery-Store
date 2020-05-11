using System.Collections.Generic;

namespace GroceryStore.Core.Abstractions.IServices.Base
{
    public interface IBaseService<T> where T : class
    {
        List<T> GetAll();
        T GetId(int id);
        T Create(T entity);
        T Update(T entity);
        void Delete(int id);
    }
}