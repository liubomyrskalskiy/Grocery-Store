using GroceryStore.Core.Abstractions.Repositories;
using GroceryStore.Core.Models;
using GroceryStore.DAL.Repositories.Base;

namespace GroceryStore.DAL.Repositories
{
    public class ProductionContentsRepository : BaseRepository<ProductionContents>, IProductionContentsRepository
    {
        public ProductionContentsRepository(GroceryStoreDbContext context) : base(context)
        {
        }
    }
}