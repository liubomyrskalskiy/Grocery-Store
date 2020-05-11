using GroceryStore.Core.Abstractions.Repositories;
using GroceryStore.Core.Models;
using GroceryStore.DAL.Repositories.Base;

namespace GroceryStore.DAL.Repositories
{
    public class GoodsRepository : BaseRepository<Goods>, IGoodsRepository
    {
        public GoodsRepository(GroceryStoreDbContext context) : base(context)
        {
        }
    }
}