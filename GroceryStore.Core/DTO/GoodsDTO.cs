namespace GroceryStore.Core.DTO
{
    public class GoodsDTO
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string FullName { get; set; }
        public double? Weight { get; set; }
        public string StringWeight { get; set; }
        public string Components { get; set; }
        public double? Price { get; set; }
        public string StringPrice { get; set; }
        public string ProductCode { get; set; }
        public string Title { get; set; }
        public string CategoryTitle { get; set; }
        public string ProducerTitle { get; set; }
    }
}