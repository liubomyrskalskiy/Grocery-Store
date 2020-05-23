namespace GroceryStore.Core.DTO
{
    public class ClientDTO
    {
        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string FullName { get; set; }
        public double? Bonuses { get; set; }
        public string Address { get; set; }
        public string FullAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string AccountNumber { get; set; }
        public string CityTitle { get; set; }
    }
}