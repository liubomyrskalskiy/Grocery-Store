using System.Collections.Generic;
using GroceryStore.Core.Models.Base;

namespace GroceryStore.Core.Models
{
    public class Role : IBaseEntity
    {
        public Role()
        {
            Employee = new HashSet<Employee>();
        }

        public string Title { get; set; }
        public double? Salary { get; set; }

        public virtual ICollection<Employee> Employee { get; set; }

        public int Id { get; set; }
    }
}