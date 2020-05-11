using GroceryStore.Core.Models.Base;

namespace GroceryStore.Core.Models
{
    public partial class Basket : IBaseEntity
    {
        public int Id { get; set; }
        public double? Amount { get; set; }
        public int? IdGoodsInMarket { get; set; }
        public int? IdSale { get; set; }

        public virtual GoodsInMarket IdGoodsInMarketNavigation { get; set; }
        public virtual Sale IdSaleNavigation { get; set; }
    }
}
