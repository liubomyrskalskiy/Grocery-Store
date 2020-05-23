using System.Collections.Generic;
using GroceryStore.Core.Models.Base;

namespace GroceryStore.Core.Models
{
    public class Country : IBaseEntity
    {
        public Country()
        {
            City = new HashSet<City>();
            Producer = new HashSet<Producer>();
        }

        public string Title { get; set; }

        public virtual ICollection<City> City { get; set; }
        public virtual ICollection<Producer> Producer { get; set; }

        public int Id { get; set; }
    }
}