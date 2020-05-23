using System.Collections.Generic;
using GroceryStore.Core.Models.Base;

namespace GroceryStore.Core.Models
{
    public class Client : IBaseEntity
    {
        public Client()
        {
            Sale = new HashSet<Sale>();
        }

        public string LastName { get; set; }
        public string FirstName { get; set; }
        public double? Bonuses { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public int? IdCity { get; set; }
        public string AccountNumber { get; set; }

        public virtual City IdCityNavigation { get; set; }
        public virtual ICollection<Sale> Sale { get; set; }

        public int Id { get; set; }
    }
}