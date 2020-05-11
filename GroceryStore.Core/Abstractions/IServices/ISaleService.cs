using GroceryStore.Core.Abstractions.IServices.Base;
using GroceryStore.Core.Models;

namespace GroceryStore.Core.Abstractions.IServices
{
    public interface ISaleService : IBaseService<Sale>
    {
        void Refresh(Sale entity);
    }
}