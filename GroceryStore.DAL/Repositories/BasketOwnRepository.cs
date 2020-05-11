using GroceryStore.Core.Abstractions.Repositories;
using GroceryStore.Core.Models;
using GroceryStore.DAL.Repositories.Base;

namespace GroceryStore.DAL.Repositories
{
    public class BasketOwnRepository : BaseRepository<BasketOwn>, IBasketOwnRepository
    {
        public BasketOwnRepository(GroceryStoreDbContext context) : base(context)
        {
        }
    }
}