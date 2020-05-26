using GroceryStore.Core.Abstractions.IServices.Base;
using GroceryStore.Core.Models;

namespace GroceryStore.Core.Abstractions.IServices
{
    public interface IDeliveryShipmentService : IBaseService<DeliveryShipment>
    {
        public void Refresh(DeliveryShipment entity);
    }
}