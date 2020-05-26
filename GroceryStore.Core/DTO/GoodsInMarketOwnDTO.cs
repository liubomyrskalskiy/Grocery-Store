using System;

namespace GroceryStore.Core.DTO
{
    public class GoodsInMarketOwnDTO
    {
        public int Id { get; set; }
        public string ProductCode { get; set; }
        public string Category { get; set; }
        public string Good { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public string Price { get; set; }
        public string Weight { get; set; }
        public string Amount { get; set; }
        public double DoubleAmount { get; set; }
        public string ProductionCode { get; set; }
        public string Address { get; set; }
        public string FullAddress { get; set; }
    }
}