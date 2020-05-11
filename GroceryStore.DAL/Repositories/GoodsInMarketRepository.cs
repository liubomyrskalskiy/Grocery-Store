using GroceryStore.Core.Abstractions.Repositories;
using GroceryStore.Core.Models;
using GroceryStore.DAL.Repositories.Base;

namespace GroceryStore.DAL.Repositories
{
    public class GoodsInMarketRepository : BaseRepository<GoodsInMarket>, IGoodsInMarketRepository
    {
        public GoodsInMarketRepository(GroceryStoreDbContext context) : base(context)
        {
        }
    }
}