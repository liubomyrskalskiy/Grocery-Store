using GroceryStore.Core.Abstractions.Repositories;
using GroceryStore.Core.Models;
using GroceryStore.DAL.Repositories.Base;

namespace GroceryStore.DAL.Repositories
{
    public class MarketRepository : BaseRepository<Market>, IMarketRepository
    {
        public MarketRepository(GroceryStoreDbContext context) : base(context)
        {
        }
    }
}