using System;

namespace GroceryStore.Core.DTO
{
    public class SaleDTO
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public string Total { get; set; }
        public string CheckNumber { get; set; }
        public string AccountNumber { get; set; }
        public string Login { get; set; }
        public string FullName { get; set; }
        public string FullMarketAddress { get; set; }
    }
}