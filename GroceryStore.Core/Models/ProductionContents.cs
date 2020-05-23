using GroceryStore.Core.Models.Base;

namespace GroceryStore.Core.Models
{
    public class ProductionContents : IBaseEntity
    {
        public double? Amount { get; set; }
        public int? IdProduction { get; set; }
        public int? IdGoodsInMarket { get; set; }

        public virtual GoodsInMarket IdGoodsInMarketNavigation { get; set; }
        public virtual Production IdProductionNavigation { get; set; }
        public int Id { get; set; }
    }
}