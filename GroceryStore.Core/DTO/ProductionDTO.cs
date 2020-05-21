using System;

namespace GroceryStore.Core.DTO
{
    public class ProductionDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string ProductCode { get; set; }
        public string ProductionCode { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public DateTime? BestBefore { get; set; }
        public int? Amount { get; set; }
        public string Login { get; set; }
        public string FullName { get; set; }
        public double? TotalCost { get; set; }
        public string StringTotalCost { get; set; }
    }
}