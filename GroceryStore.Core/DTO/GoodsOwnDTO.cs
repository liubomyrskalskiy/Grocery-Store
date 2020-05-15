namespace GroceryStore.Core.DTO
{
    public class GoodsOwnDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public double? Weight { get; set; }
        public string StringWeight { get; set; }
        public string Components { get; set; }
        public double? Price { get; set; }
        public string StringPrice { get; set; }
        public string Category { get; set; }
        public string ProductCode { get; set; }
    }
}