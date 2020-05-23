using System.Collections.Generic;
using GroceryStore.Core.Models.Base;

namespace GroceryStore.Core.Models
{
    public class WriteOffReason : IBaseEntity
    {
        public WriteOffReason()
        {
            GoodsWriteOff = new HashSet<GoodsWriteOff>();
            GoodsWriteOffOwn = new HashSet<GoodsWriteOffOwn>();
        }

        public string Description { get; set; }

        public virtual ICollection<GoodsWriteOff> GoodsWriteOff { get; set; }
        public virtual ICollection<GoodsWriteOffOwn> GoodsWriteOffOwn { get; set; }

        public int Id { get; set; }
    }
}