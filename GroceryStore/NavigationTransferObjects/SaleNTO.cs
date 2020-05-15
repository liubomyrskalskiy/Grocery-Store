using GroceryStore.Core.DTO;
using GroceryStore.Core.Models;

namespace GroceryStore.NavigationTransferObjects
{
    public class SaleNTO
    {
        public Sale Sale { get; set; }
        public EmployeeDTO EmployeeDto { get; set; }

        public SaleNTO(Sale sale, EmployeeDTO employeeDto)
        {
            Sale = sale;
            EmployeeDto = employeeDto;
        }
    }
}