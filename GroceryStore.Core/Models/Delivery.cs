using System;
using System.Collections.Generic;
using GroceryStore.Core.Models.Base;

namespace GroceryStore.Core.Models
{
    public partial class Delivery : IBaseEntity
    {
        public Delivery()
        {
            DeliveryContents = new HashSet<DeliveryContents>();
        }
        public int Id { get; set; }
        public string DeliveryNumber { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public int? IdProvider { get; set; }

        public virtual Provider IdProviderNavigation { get; set; }
        public virtual ICollection<DeliveryContents> DeliveryContents { get; set; }
    }
}
