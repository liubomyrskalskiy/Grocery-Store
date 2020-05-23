using System.Collections.Generic;
using GroceryStore.Core.Models.Base;

namespace GroceryStore.Core.Models
{
    public class Goods : IBaseEntity
    {
        public Goods()
        {
            Consignment = new HashSet<Consignment>();
            GoodsInMarket = new HashSet<GoodsInMarket>();
        }

        public string Description { get; set; }
        public double? Weight { get; set; }
        public string Components { get; set; }
        public double? Price { get; set; }
        public int? IdCategory { get; set; }
        public int? IdProducer { get; set; }
        public string ProductCode { get; set; }
        public string Title { get; set; }

        public virtual Category IdCategoryNavigation { get; set; }
        public virtual Producer IdProducerNavigation { get; set; }
        public virtual ICollection<Consignment> Consignment { get; set; }
        public virtual ICollection<GoodsInMarket> GoodsInMarket { get; set; }

        public int Id { get; set; }
    }
}