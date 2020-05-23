using GroceryStore.Core.Models.Base;

namespace GroceryStore.Core.Models
{
    public class BasketOwn : IBaseEntity
    {
        public double? Amount { get; set; }
        public int? IdGoodsInMarketOwn { get; set; }
        public int? IdSale { get; set; }

        public virtual GoodsInMarketOwn IdGoodsInMarketOwnNavigation { get; set; }
        public virtual Sale IdSaleNavigation { get; set; }
        public int Id { get; set; }
    }
}