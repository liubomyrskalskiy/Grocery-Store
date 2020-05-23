using System;
using System.Collections.Generic;
using GroceryStore.Core.Models.Base;

namespace GroceryStore.Core.Models
{
    public class Production : IBaseEntity
    {
        public Production()
        {
            GoodsInMarketOwn = new HashSet<GoodsInMarketOwn>();
            GoodsWriteOffOwn = new HashSet<GoodsWriteOffOwn>();
            ProductionContents = new HashSet<ProductionContents>();
        }

        public int? IdGoodsOwn { get; set; }
        public string ProductionCode { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public DateTime? BestBefore { get; set; }
        public int? Amount { get; set; }
        public int? IdEmployee { get; set; }
        public double? TotalCost { get; set; }

        public virtual Employee IdEmployeeNavigation { get; set; }
        public virtual GoodsOwn IdGoodsOwnNavigation { get; set; }
        public virtual ICollection<GoodsInMarketOwn> GoodsInMarketOwn { get; set; }
        public virtual ICollection<GoodsWriteOffOwn> GoodsWriteOffOwn { get; set; }
        public virtual ICollection<ProductionContents> ProductionContents { get; set; }

        public int Id { get; set; }
    }
}