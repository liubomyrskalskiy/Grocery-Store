using GroceryStore.Core.Abstractions.Repositories;
using GroceryStore.Core.Models;
using GroceryStore.DAL.Repositories.Base;

namespace GroceryStore.DAL.Repositories
{
    public class GoodsWriteOffRepository : BaseRepository<GoodsWriteOff>, IGoodsWriteOffRepository
    {
        public GoodsWriteOffRepository(GroceryStoreDbContext context) : base(context)
        {
        }
    }
}