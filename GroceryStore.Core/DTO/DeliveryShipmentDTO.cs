using System;

namespace GroceryStore.Core.DTO
{
    public class DeliveryShipmentDTO
    {
        public int Id { get; set; }
        public double? Amount { get; set; }
        public string StringAmount { get; set; }
        public DateTime? ShipmentDateTime { get; set; }
        public string GoodsTitle { get; set; }
        public string ProductCode { get; set; }
        public string ConsignmentNumber { get; set; }
        public string Address { get; set; }
    }
}