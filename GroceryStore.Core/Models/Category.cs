using System.Collections.Generic;
using GroceryStore.Core.Models.Base;

namespace GroceryStore.Core.Models
{
    public class Category : IBaseEntity
    {
        public Category()
        {
            Goods = new HashSet<Goods>();
            GoodsOwn = new HashSet<GoodsOwn>();
        }

        public string Title { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Goods> Goods { get; set; }
        public virtual ICollection<GoodsOwn> GoodsOwn { get; set; }

        public int Id { get; set; }
    }
}