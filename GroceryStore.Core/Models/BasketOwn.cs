using GroceryStore.Core.Models.Base;

namespace GroceryStore.Core.Models
{
    public partial class BasketOwn : IBaseEntity
    {
        public int Id { get; set; }
        public double? Amount { get; set; }
        public int? IdGoodsInMarketOwn { get; set; }
        public int? IdSale { get; set; }

        public virtual GoodsInMarketOwn IdGoodsInMarketOwnNavigation { get; set; }
        public virtual Sale IdSaleNavigation { get; set; }
    }
}
