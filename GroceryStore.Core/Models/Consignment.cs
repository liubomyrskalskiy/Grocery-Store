using System;
using System.Collections.Generic;
using GroceryStore.Core.Models.Base;

namespace GroceryStore.Core.Models
{
    public class Consignment : IBaseEntity
    {
        public Consignment()
        {
            DeliveryContents = new HashSet<DeliveryContents>();
            DeliveryShipment = new HashSet<DeliveryShipment>();
        }

        public string ConsignmentNumber { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public DateTime? BestBefore { get; set; }
        public double? Amount { get; set; }
        public int? IdGoods { get; set; }
        public double? IncomePrice { get; set; }

        public virtual Goods IdGoodsNavigation { get; set; }
        public virtual ICollection<DeliveryContents> DeliveryContents { get; set; }
        public virtual ICollection<DeliveryShipment> DeliveryShipment { get; set; }

        public int Id { get; set; }
    }
}