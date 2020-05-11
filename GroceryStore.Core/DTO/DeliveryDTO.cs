using System;

namespace GroceryStore.Core.DTO
{
    public class DeliveryDTO
    {
        public int Id { get; set; }
        public string DeliveryNumber { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string ProviderTitle { get; set; }
    }
}
