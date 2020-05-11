using System.Collections.Generic;
using GroceryStore.Core.Models.Base;

namespace GroceryStore.Core.Models
{
    public partial class Provider : IBaseEntity
    {
        public Provider()
        {
            Delivery = new HashSet<Delivery>();
        }

        public int Id { get; set; }
        public string CompanyTitle { get; set; }
        public string ContactPerson { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public int? IdCity { get; set; }

        public virtual City IdCityNavigation { get; set; }
        public virtual ICollection<Delivery> Delivery { get; set; }
    }
}
