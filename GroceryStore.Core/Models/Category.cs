using System.Collections.Generic;
using GroceryStore.Core.Models.Base;

namespace GroceryStore.Core.Models
{
    public partial class Category : IBaseEntity
    {
        public Category()
        {
            Goods = new HashSet<Goods>();
            GoodsOwn = new HashSet<GoodsOwn>();
        }
        public int Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Goods> Goods { get; set; }
        public virtual ICollection<GoodsOwn> GoodsOwn { get; set; }
    }
}
