namespace GroceryStore.Core.DTO
{
    public class GoodsInMarketDTO
    {
        public int Id { get; set; }
        public string GoodsTitle { get; set; }
        public string ProductCode { get; set; }
        public string ProducerTitle { get; set; }
        public string Price { get; set; }
        public string Weight { get; set; }
        public string Address { get; set; }
        public string FullMarketAddress { get; set; }
        public string Amount { get; set; }
    }
}