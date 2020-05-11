using System.Collections.Generic;
using GroceryStore.Core.Models.Base;

namespace GroceryStore.Core.Models
{
    public partial class Role : IBaseEntity
    {
        public Role()
        {
            Employee = new HashSet<Employee>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public double? Salary { get; set; }

        public virtual ICollection<Employee> Employee { get; set; }
    }
}
