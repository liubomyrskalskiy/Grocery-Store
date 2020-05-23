using System;

namespace GroceryStore.Core.DTO
{
    public class DeliveryDTO
    {
        public int Id { get; set; }
        public string DeliveryNumber { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string ProviderTitle { get; set; }
        public string ContactPerson { get; set; }
        public string PhoneNumber { get; set; }
        public double Total { get; set; }
        public string StringTotal { get; set; }
    }
}