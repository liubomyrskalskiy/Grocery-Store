namespace GroceryStore.Core.DTO
{
    public class GoodsInMarketDTO
    {
        public int Id { get; set; }
        public string GoodsTitle { get; set; }
        public string ProductCode { get; set; }
        public string Address { get; set; }
        public double? Amount { get; set; }
    }
}