using GroceryStore.Core.Abstractions.Repositories;
using GroceryStore.Core.Models;
using GroceryStore.DAL.Repositories.Base;

namespace GroceryStore.DAL.Repositories
{
    public class GoodsWriteOffOwnRepository : BaseRepository<GoodsWriteOffOwn>, IGoodsWriteOffOwnRepository
    {
        public GoodsWriteOffOwnRepository(GroceryStoreDbContext context) : base(context)
        {
        }
    }
}