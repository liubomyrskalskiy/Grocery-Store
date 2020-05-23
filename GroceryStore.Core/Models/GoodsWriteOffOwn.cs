using System;
using GroceryStore.Core.Models.Base;

namespace GroceryStore.Core.Models
{
    public class GoodsWriteOffOwn : IBaseEntity
    {
        public int? IdGoodsInMarketOwn { get; set; }
        public int? IdEmployee { get; set; }
        public int? IdWriteOffReason { get; set; }
        public DateTime? Date { get; set; }
        public double? Amount { get; set; }
        public int? IdProduction { get; set; }

        public virtual Employee IdEmployeeNavigation { get; set; }
        public virtual GoodsInMarketOwn IdGoodsInMarketOwnNavigation { get; set; }
        public virtual Production IdProductionNavigation { get; set; }
        public virtual WriteOffReason IdWriteOffReasonNavigation { get; set; }
        public int Id { get; set; }
    }
}