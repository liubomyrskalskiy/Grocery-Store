using System.Collections.Generic;
using GroceryStore.Core.Models.Base;

namespace GroceryStore.Core.Models
{
    public class GoodsInMarket : IBaseEntity
    {
        public GoodsInMarket()
        {
            Basket = new HashSet<Basket>();
            DeliveryShipment = new HashSet<DeliveryShipment>();
            GoodsWriteOff = new HashSet<GoodsWriteOff>();
            ProductionContents = new HashSet<ProductionContents>();
        }

        public double? Amount { get; set; }
        public int? IdMarket { get; set; }
        public int? IdGoods { get; set; }

        public virtual Goods IdGoodsNavigation { get; set; }
        public virtual Market IdMarketNavigation { get; set; }
        public virtual ICollection<Basket> Basket { get; set; }
        public virtual ICollection<DeliveryShipment> DeliveryShipment { get; set; }
        public virtual ICollection<GoodsWriteOff> GoodsWriteOff { get; set; }
        public virtual ICollection<ProductionContents> ProductionContents { get; set; }

        public int Id { get; set; }
    }
}