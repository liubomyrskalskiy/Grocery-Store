using GroceryStore.Core.Abstractions.Repositories;
using GroceryStore.Core.Models;
using GroceryStore.DAL.Repositories.Base;

namespace GroceryStore.DAL.Repositories
{
    public class SaleRepository : BaseRepository<Sale>, ISaleRepository
    {
        public SaleRepository(GroceryStoreDbContext context) : base(context)
        {
        }
    }
}