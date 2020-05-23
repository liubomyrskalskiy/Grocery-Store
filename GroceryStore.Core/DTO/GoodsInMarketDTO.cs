namespace GroceryStore.Core.DTO
{
    public class GoodsInMarketDTO
    {
        public int Id { get; set; }
        public string ProductCode { get; set; }
        public string Producer { get; set; }
        public string Category { get; set; }
        public string Good { get; set; }
        public string Weight { get; set; }
        public string Price { get; set; }
        public string MarketAddress { get; set; }
        public string FullMarketAddress { get; set; }
        public string Amount { get; set; }
    }
}