namespace GroceryStore.Core.DTO
{
    public class ProductionContentsDTO
    {
        public int Id { get; set; }
        public string ProductCode { get; set; }
        public string Producer { get; set; }
        public string Category { get; set; }
        public string Title { get; set; }
        public string ProductionCode { get; set; }
        public double? Amount { get; set; }
        public double? Price { get; set; }
        public string StringPrice { get; set; }
    }
}