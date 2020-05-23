using System;

namespace GroceryStore.Core.Models
{
    public class VWritingOffOwnProducts
    {
        public int? IdGoodsInMarketOwn { get; set; }
        public DateTime? Date { get; set; }
        public double? Amount { get; set; }
        public string Description { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public double? TotalCost { get; set; }
    }
}