using GroceryStore.Core.Abstractions.Repositories;
using GroceryStore.Core.Models;
using GroceryStore.DAL.Repositories.Base;

namespace GroceryStore.DAL.Repositories
{
    public class DeliveryShipmentRepository : BaseRepository<DeliveryShipment>, IDeliveryShipmentRepository
    {
        public DeliveryShipmentRepository(GroceryStoreDbContext context) : base(context)
        {
        }
    }
}