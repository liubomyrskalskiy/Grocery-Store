namespace GroceryStore.Core.DTO
{
    public class EmployeeDTO
    {
        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public int? WorkExperience { get; set; }
        public string Address { get; set; }
        public string FullAddress { get; set; }
        public string RoleTitle { get; set; }
        public string MarketAddress { get; set; }
        public string FullMarketAddress { get; set; }
        public string CityTitle { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}