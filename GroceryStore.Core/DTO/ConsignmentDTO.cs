using System;

namespace GroceryStore.Core.DTO
{
    public class ConsignmentDTO
    {
        public int Id { get; set; }
        public string ConsignmentNumber { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public DateTime? BestBefore { get; set; }
        public double? Amount { get; set; }
        public string StringAmount { get; set; } //
        public double? IncomePrice { get; set; }
        public string StringIncomePrice { get; set; } //
        public string GoodTitle { get; set; }
        public string ProductCode { get; set; }
        public string Producer { get; set; }
        public string Category { get; set; }
    }
}