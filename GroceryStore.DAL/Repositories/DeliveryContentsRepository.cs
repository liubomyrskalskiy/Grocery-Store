using GroceryStore.Core.Abstractions.Repositories;
using GroceryStore.Core.Models;
using GroceryStore.DAL.Repositories.Base;

namespace GroceryStore.DAL.Repositories
{
    public class DeliveryContentsRepository : BaseRepository<DeliveryContents>, IDeliveryContentsRepository
    {
        public DeliveryContentsRepository(GroceryStoreDbContext context) : base(context)
        {
        }
    }
}