using GroceryStore.Core.DTO;
using GroceryStore.Core.Models;

namespace GroceryStore.NavigationTransferObjects
{
    public class ProductionNTO
    {
        public ProductionNTO(Production production, EmployeeDTO employee)
        {
            Production = production;
            Employee = employee;
        }

        public Production Production { get; set; }
        public EmployeeDTO Employee { get; set; }
    }
}