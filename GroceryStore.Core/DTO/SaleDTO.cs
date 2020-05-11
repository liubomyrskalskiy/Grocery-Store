using System;

namespace GroceryStore.Core.DTO
{
    public class SaleDTO
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public double? Total { get; set; }
        public string CheckNumber { get; set; }
        public string AccountNumber { get; set; }
        public string Login { get; set; }
    }
}