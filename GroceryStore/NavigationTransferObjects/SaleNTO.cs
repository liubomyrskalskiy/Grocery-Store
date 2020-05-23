using GroceryStore.Core.DTO;
using GroceryStore.Core.Models;

namespace GroceryStore.NavigationTransferObjects
{
    public class SaleNTO
    {
        public SaleNTO(Sale sale, EmployeeDTO employeeDto)
        {
            Sale = sale;
            EmployeeDto = employeeDto;
        }

        public Sale Sale { get; set; }
        public EmployeeDTO EmployeeDto { get; set; }
    }
}