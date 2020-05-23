using System;

namespace GroceryStore.Core.DTO
{
    public class DeliveryContentsDTO
    {
        public int Id { get; set; }

        public string ProductCode { get; set; }
        public string GoodTitle { get; set; }
        public string ProducerTitle { get; set; }


        public string DeliveryNumber { get; set; }
        public string ProviderTitle { get; set; }
        public string ContactPerson { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? OrderDate { get; set; }


        public string ConsignmentNumber { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public DateTime? BestBefore { get; set; }
        public double? OrderAmount { get; set; }
        public string StringOrderAmount { get; set; }
        public double? IncomePrice { get; set; }
        public string StringIncomePrice { get; set; }
    }
}