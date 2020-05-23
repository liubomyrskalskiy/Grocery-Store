using System;

namespace GroceryStore.Core.Models
{
    public class VProductsInfo
    {
        public string Title { get; set; }
        public string ProductCode { get; set; }
        public double? Weight { get; set; }
        public double? Price { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public string ConsignmentNumber { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string CompanyTitle { get; set; }
        public string Address { get; set; }
        public string TitleCity { get; set; }
    }
}