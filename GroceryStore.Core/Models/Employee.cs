using System.Collections.Generic;
using GroceryStore.Core.Models.Base;

namespace GroceryStore.Core.Models
{
    public class Employee : IBaseEntity
    {
        public Employee()
        {
            GoodsWriteOff = new HashSet<GoodsWriteOff>();
            GoodsWriteOffOwn = new HashSet<GoodsWriteOffOwn>();
            Production = new HashSet<Production>();
            Sale = new HashSet<Sale>();
        }

        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string PhoneNumber { get; set; }
        public int? WorkExperience { get; set; }
        public string Address { get; set; }
        public int? IdMarket { get; set; }
        public int? IdRole { get; set; }
        public int? IdCity { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        public virtual City IdCityNavigation { get; set; }
        public virtual Market IdMarketNavigation { get; set; }
        public virtual Role IdRoleNavigation { get; set; }
        public virtual ICollection<GoodsWriteOff> GoodsWriteOff { get; set; }
        public virtual ICollection<GoodsWriteOffOwn> GoodsWriteOffOwn { get; set; }
        public virtual ICollection<Production> Production { get; set; }
        public virtual ICollection<Sale> Sale { get; set; }

        public int Id { get; set; }
    }
}