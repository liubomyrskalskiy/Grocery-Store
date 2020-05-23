namespace GroceryStore.Core.DTO
{
    public class UniversalBasketDTO
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public string FullTitle { get; set; }
        public string Producer { get; set; }
        public string ProductCode { get; set; }
        public string Price { get; set; }
        public double? Amount { get; set; }
        public string CheckNumber { get; set; }
        public bool IsOwn { get; set; }
    }
}