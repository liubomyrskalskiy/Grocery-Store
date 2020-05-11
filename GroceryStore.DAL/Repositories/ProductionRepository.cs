using GroceryStore.Core.Abstractions.Repositories;
using GroceryStore.Core.Models;
using GroceryStore.DAL.Repositories.Base;

namespace GroceryStore.DAL.Repositories
{
    public class ProductionRepository : BaseRepository<Production>, IProductionRepository
    {
        public ProductionRepository(GroceryStoreDbContext context) : base(context)
        {
        }
    }
}