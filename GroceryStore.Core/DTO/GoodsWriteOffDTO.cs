using System;

namespace GroceryStore.Core.DTO
{
    public class GoodsWriteOffDTO
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public double? Amount { get; set; }
        public string Login { get; set; }
        public string FullName { get; set; }
        public string Reason { get; set; }
        public string ConsignmentNumber { get; set; }
        public DateTime? ShipmentDateTime { get; set; }
        public string GoodTitle { get; set; }
        public string ProductCode { get; set; }
    }
}