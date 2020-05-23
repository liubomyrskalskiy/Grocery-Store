using System.Collections.Generic;
using GroceryStore.Core.Models.Base;

namespace GroceryStore.Core.Models
{
    public class City : IBaseEntity
    {
        public City()
        {
            Client = new HashSet<Client>();
            Employee = new HashSet<Employee>();
            Market = new HashSet<Market>();
            Provider = new HashSet<Provider>();
        }

        public string Title { get; set; }
        public int? IdCountry { get; set; }

        public virtual Country IdCountryNavigation { get; set; }
        public virtual ICollection<Client> Client { get; set; }
        public virtual ICollection<Employee> Employee { get; set; }
        public virtual ICollection<Market> Market { get; set; }
        public virtual ICollection<Provider> Provider { get; set; }

        public int Id { get; set; }
    }
}