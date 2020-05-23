using System.Collections.Generic;
using GroceryStore.Core.Models.Base;

namespace GroceryStore.Core.Models
{
    public class GoodsInMarketOwn : IBaseEntity
    {
        public GoodsInMarketOwn()
        {
            BasketOwn = new HashSet<BasketOwn>();
            GoodsWriteOffOwn = new HashSet<GoodsWriteOffOwn>();
        }

        public double? Amount { get; set; }
        public int? IdProduction { get; set; }
        public int? IdMarket { get; set; }

        public virtual Market IdMarketNavigation { get; set; }
        public virtual Production IdProductionNavigation { get; set; }
        public virtual ICollection<BasketOwn> BasketOwn { get; set; }
        public virtual ICollection<GoodsWriteOffOwn> GoodsWriteOffOwn { get; set; }

        public int Id { get; set; }
    }
}