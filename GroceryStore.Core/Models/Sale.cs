using System;
using System.Collections.Generic;
using GroceryStore.Core.Models.Base;

namespace GroceryStore.Core.Models
{
    public class Sale : IBaseEntity
    {
        public Sale()
        {
            Basket = new HashSet<Basket>();
            BasketOwn = new HashSet<BasketOwn>();
        }

        public DateTime? Date { get; set; }
        public double? Total { get; set; }
        public string CheckNumber { get; set; }
        public int? IdClient { get; set; }
        public int? IdEmployee { get; set; }

        public virtual Client IdClientNavigation { get; set; }
        public virtual Employee IdEmployeeNavigation { get; set; }
        public virtual ICollection<Basket> Basket { get; set; }
        public virtual ICollection<BasketOwn> BasketOwn { get; set; }

        public int Id { get; set; }
    }
}