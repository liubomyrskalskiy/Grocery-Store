using System;
using GroceryStore.Core.Models.Base;

namespace GroceryStore.Core.Models
{
    public class GoodsWriteOff : IBaseEntity
    {
        public DateTime? Date { get; set; }
        public int? IdEmployee { get; set; }
        public int? IdWriteOffReason { get; set; }
        public int? IdDeliveryShipment { get; set; }
        public int? IdGoodsInMarket { get; set; }
        public double? Amount { get; set; }

        public virtual DeliveryShipment IdDeliveryShipmentNavigation { get; set; }
        public virtual Employee IdEmployeeNavigation { get; set; }
        public virtual GoodsInMarket IdGoodsInMarketNavigation { get; set; }
        public virtual WriteOffReason IdWriteOffReasonNavigation { get; set; }
        public int Id { get; set; }
    }
}