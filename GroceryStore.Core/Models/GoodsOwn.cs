using System.Collections.Generic;
using GroceryStore.Core.Models.Base;

namespace GroceryStore.Core.Models
{
    public class GoodsOwn : IBaseEntity
    {
        public GoodsOwn()
        {
            Production = new HashSet<Production>();
        }

        public string Title { get; set; }
        public double? Weight { get; set; }
        public string Components { get; set; }
        public double? Price { get; set; }
        public int? IdCategory { get; set; }
        public string ProductCode { get; set; }

        public virtual Category IdCategoryNavigation { get; set; }
        public virtual ICollection<Production> Production { get; set; }

        public int Id { get; set; }
    }
}