using GroceryStore.Core.Models.Base;

namespace GroceryStore.Core.Models
{
    public partial class DeliveryContents : IBaseEntity
    {
        public int Id { get; set; }
        public int IdConsignment { get; set; }
        public int IdDelivery { get; set; }

        public virtual Consignment IdConsignmentNavigation { get; set; }
        public virtual Delivery IdDeliveryNavigation { get; set; }
    }
}
