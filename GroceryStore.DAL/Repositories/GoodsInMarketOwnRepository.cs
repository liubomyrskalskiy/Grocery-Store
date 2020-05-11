using GroceryStore.Core.Abstractions.Repositories;
using GroceryStore.Core.Models;
using GroceryStore.DAL.Repositories.Base;

namespace GroceryStore.DAL.Repositories
{
    public class GoodsInMarketOwnRepository : BaseRepository<GoodsInMarketOwn>, IGoodsInMarketOwnRepository
    {
        public GoodsInMarketOwnRepository(GroceryStoreDbContext context) : base(context)
        {
        }
    }
}