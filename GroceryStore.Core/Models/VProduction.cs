using System;

namespace GroceryStore.Core.Models
{
    public class VProduction
    {
        public string Title { get; set; }
        public string ProductCode { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public double? Amount { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
    }
}