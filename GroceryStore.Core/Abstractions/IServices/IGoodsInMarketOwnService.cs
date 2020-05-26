using GroceryStore.Core.Abstractions.IServices.Base;
using GroceryStore.Core.Models;

namespace GroceryStore.Core.Abstractions.IServices
{
    public interface IGoodsInMarketOwnService : IBaseService<GoodsInMarketOwn>
    {
        public void Refresh(GoodsInMarketOwn entity);
    }
}