using GroceryStore.Core.Models.Base;

namespace GroceryStore.Core.Models
{
    public class DeliveryContents : IBaseEntity
    {
        public int IdConsignment { get; set; }
        public int IdDelivery { get; set; }

        public virtual Consignment IdConsignmentNavigation { get; set; }
        public virtual Delivery IdDeliveryNavigation { get; set; }
        public int Id { get; set; }
    }
}