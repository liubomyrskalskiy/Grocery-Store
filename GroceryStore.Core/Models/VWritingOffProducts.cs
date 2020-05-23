using System;

namespace GroceryStore.Core.Models
{
    public class VWritingOffProducts
    {
        public int? IdGoodsInMarket { get; set; }
        public DateTime? DateСписування { get; set; }
        public double? КСтьСписаногоТовару { get; set; }
        public string Description { get; set; }
        public DateTime? ShipmentDateTime { get; set; }
    }
}