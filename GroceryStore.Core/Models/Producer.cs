using System.Collections.Generic;
using GroceryStore.Core.Models.Base;

namespace GroceryStore.Core.Models
{
    public class Producer : IBaseEntity
    {
        public Producer()
        {
            Goods = new HashSet<Goods>();
        }

        public string Title { get; set; }
        public int? IdCountry { get; set; }

        public virtual Country IdCountryNavigation { get; set; }
        public virtual ICollection<Goods> Goods { get; set; }

        public int Id { get; set; }
    }
}