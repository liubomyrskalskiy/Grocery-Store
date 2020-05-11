namespace GroceryStore.Core.DTO
{
    public class BasketOwnDTO
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public double? Amount { get; set; }

        public string CheckNumber { get; set; }

        public string ProductCode { get; set; }
    }
}