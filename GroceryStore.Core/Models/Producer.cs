using System.Collections.Generic;
using GroceryStore.Core.Models.Base;

namespace GroceryStore.Core.Models
{
    public partial class Producer : IBaseEntity
    {
        public Producer()
        {
            Goods = new HashSet<Goods>();
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public int? IdCountry { get; set; }

        public virtual Country IdCountryNavigation { get; set; }
        public virtual ICollection<Goods> Goods { get; set; }
    }
}
