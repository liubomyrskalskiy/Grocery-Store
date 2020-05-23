using System;
using System.Collections.Generic;
using GroceryStore.Core.Models.Base;

namespace GroceryStore.Core.Models
{
    public class DeliveryShipment : IBaseEntity
    {
        public DeliveryShipment()
        {
            GoodsWriteOff = new HashSet<GoodsWriteOff>();
        }

        public int? IdConsignment { get; set; }
        public int? IdGoodsInMarket { get; set; }
        public double? Amount { get; set; }
        public DateTime? ShipmentDateTime { get; set; }

        public virtual Consignment IdConsignmentNavigation { get; set; }
        public virtual GoodsInMarket IdGoodsInMarketNavigation { get; set; }
        public virtual ICollection<GoodsWriteOff> GoodsWriteOff { get; set; }

        public int Id { get; set; }
    }
}