using GroceryStore.Core.Abstractions.Repositories;
using GroceryStore.Core.Models;
using GroceryStore.DAL.Repositories.Base;

namespace GroceryStore.DAL.Repositories
{
    public class ProducerRepository : BaseRepository<Producer>, IProducerRepository
    {
        public ProducerRepository(GroceryStoreDbContext context) : base(context)
        {
        }
    }
}