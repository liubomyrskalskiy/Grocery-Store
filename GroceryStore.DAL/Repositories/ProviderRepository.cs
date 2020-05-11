using GroceryStore.Core.Abstractions.Repositories;
using GroceryStore.Core.Models;
using GroceryStore.DAL.Repositories.Base;

namespace GroceryStore.DAL.Repositories
{
    public class ProviderRepository : BaseRepository<Provider>, IProviderRepository
    {
        public ProviderRepository(GroceryStoreDbContext context) : base(context)
        {
        }
    }
}