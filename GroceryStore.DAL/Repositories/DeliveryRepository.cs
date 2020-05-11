using GroceryStore.Core.Abstractions.Repositories;
using GroceryStore.Core.Models;
using GroceryStore.DAL.Repositories.Base;

namespace GroceryStore.DAL.Repositories
{
    public class DeliveryRepository : BaseRepository<Delivery>, IDeliveryRepository
    {
        public DeliveryRepository(GroceryStoreDbContext context) : base(context)
        {
        }
    }
}