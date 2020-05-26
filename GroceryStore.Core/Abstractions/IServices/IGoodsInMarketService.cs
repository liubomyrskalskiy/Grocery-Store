using GroceryStore.Core.Abstractions.IServices.Base;
using GroceryStore.Core.Models;

namespace GroceryStore.Core.Abstractions.IServices
{
    public interface IGoodsInMarketService : IBaseService<GoodsInMarket>
    {
        public void Refresh(GoodsInMarket entity);
    }
}