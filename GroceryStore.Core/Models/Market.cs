using System.Collections.Generic;
using GroceryStore.Core.Models.Base;

namespace GroceryStore.Core.Models
{
    public class Market : IBaseEntity
    {
        public Market()
        {
            Employee = new HashSet<Employee>();
            GoodsInMarket = new HashSet<GoodsInMarket>();
            GoodsInMarketOwn = new HashSet<GoodsInMarketOwn>();
        }

        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public int? IdCity { get; set; }

        public virtual City IdCityNavigation { get; set; }
        public virtual ICollection<Employee> Employee { get; set; }
        public virtual ICollection<GoodsInMarket> GoodsInMarket { get; set; }
        public virtual ICollection<GoodsInMarketOwn> GoodsInMarketOwn { get; set; }

        public int Id { get; set; }
    }
}