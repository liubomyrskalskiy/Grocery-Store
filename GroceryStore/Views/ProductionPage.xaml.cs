using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using AutoMapper;
using GroceryStore.Core.Abstractions;
using GroceryStore.Core.Abstractions.IServices;
using GroceryStore.Core.DTO;
using GroceryStore.Core.Models;
using Microsoft.Extensions.Options;

namespace GroceryStore.Views
{
    /// <summary>
    /// Interaction logic for ProductionPage.xaml
    /// </summary>
    public partial class ProductionPage : Page, IActivable
    {
        private readonly IProductionService _productionService;
        private readonly IEmployeeService _employeeService;
        private readonly IGoodsOwnService _goodsOwnService;
        private AppSettings _settings;
        private readonly IMapper _mapper;

        private List<ProductionDTO> ProductionDtos { get; set; }
        public ProductionPage(IProductionService productionService, IEmployeeService employeeService, IGoodsOwnService goodsOwnService, IOptions<AppSettings> settings, IMapper mapper)
        {
            _productionService = productionService;
            _employeeService = employeeService;
            _goodsOwnService = goodsOwnService;
            _mapper = mapper;
            _settings = settings.Value;
            InitializeComponent();

            UpdateDataGrid();
        }

        private void UpdateDataGrid()
        {
            ProductionDtos = _mapper.Map<List<Production>, List<ProductionDTO>>(_productionService.GetAll());

            DataGrid.ItemsSource = ProductionDtos;
        }

        private bool ValidateForm()
        {
            if (!Regex.Match(ProductCodeTextBox.Text, @"^\d{5}$").Success)
            {
                MessageBox.Show("Invalid product code! It must contain 5 digits");
                ProductCodeTextBox.Focus();
                return false;
            }

            if (!Regex.Match(ProductionCodeTextBox.Text, @"^\d{5,10}$").Success)
            {
                MessageBox.Show("Invalid production code! It must contain at least 5 digits and not exceed 10 digits");
                ProductionCodeTextBox.Focus();
                return false;
            }

            if (!Regex.Match(AmountTextBox.Text, @"^[0-9]*(?:\,[0-9]*)?$").Success)
            {
                MessageBox.Show("Invalid amount! Check the data you've entered!");
                AmountTextBox.Focus();
                return false;
            }

            if (!Regex.Match(LoginTextBox.Text, @"^\D{6,20}$").Success)
            {
                MessageBox.Show("Login must consist of at least 6 character and not exceed 20 characters!");
                LoginTextBox.Focus();
                return false;
            }

            if (!Regex.Match(TotalCostTextBox.Text, @"^[0-9]*(?:\,[0-9]*)?$").Success)
            {
                MessageBox.Show("Invalid total cost! Check the data you've entered!");
                TotalCostTextBox.Focus();
                return false;
            }

            return true;
        }

        public Task ActivateAsync(object parameter)
        {
            return Task.CompletedTask;
        }

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedIndex != -1)
            {
                ProductCodeTextBox.Text = ProductionDtos[DataGrid.SelectedIndex].ProductCode;
                ProductionCodeTextBox.Text = ProductionDtos[DataGrid.SelectedIndex].ProductionCode;
                AmountTextBox.Text = ProductionDtos[DataGrid.SelectedIndex].Amount.ToString();
                LoginTextBox.Text = ProductionDtos[DataGrid.SelectedIndex].Login;
                TotalCostTextBox.Text = ProductionDtos[DataGrid.SelectedIndex].TotalCost.ToString();
            }
        }

        private void CreateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;
            Production production = new Production();
            Employee tempEmployee = new Employee();
            GoodsOwn tempGoodsOwn = new GoodsOwn();
            production.Id = ProductionDtos[^1]?.Id + 1 ?? 1;
            production.ProductionCode = ProductionCodeTextBox.Text;
            production.ManufactureDate = DateTime.Now;
            production.BestBefore = DateTime.Now.AddDays(2);
            production.Amount = Convert.ToInt32(AmountTextBox.Text);
            production.TotalCost = Convert.ToDouble(TotalCostTextBox.Text);
            if ((tempEmployee = _employeeService.GetAll()
                    .FirstOrDefault(employee => employee.Login == LoginTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such employee in database!");
                return;
            }
            else
                production.IdEmployee = tempEmployee.Id;

            if ((tempGoodsOwn = _goodsOwnService.GetAll()
                    .FirstOrDefault(goodsOwn => goodsOwn.ProductCode == ProductCodeTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such own product in database!");
                return;
            }
            else
                production.IdGoodsOwn = tempGoodsOwn.Id;

            _productionService.Create(production);
            UpdateDataGrid();
        }

        private void UpdateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            if (!ValidateForm()) return;
            Production production = new Production();
            Employee tempEmployee = new Employee();
            GoodsOwn tempGoodsOwn = new GoodsOwn();
            production.Id = ProductionDtos[DataGrid.SelectedIndex].Id;
            production.ProductionCode = ProductionCodeTextBox.Text;
            production.ManufactureDate = ProductionDtos[DataGrid.SelectedIndex].ManufactureDate;
            production.BestBefore = ProductionDtos[DataGrid.SelectedIndex].BestBefore;
            production.Amount = Convert.ToInt32(AmountTextBox.Text);
            production.TotalCost = Convert.ToDouble(TotalCostTextBox.Text);
            if ((tempEmployee = _employeeService.GetAll()
                    .FirstOrDefault(employee => employee.Login == LoginTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such employee in database!");
                return;
            }
            else
                production.IdEmployee = tempEmployee.Id;

            if ((tempGoodsOwn = _goodsOwnService.GetAll()
                    .FirstOrDefault(goodsOwn => goodsOwn.ProductCode == ProductCodeTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such own product in database!");
                return;
            }
            else
                production.IdGoodsOwn = tempGoodsOwn.Id;

            _productionService.Update(production);
            UpdateDataGrid();
        }

        private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            _productionService.Delete(ProductionDtos[DataGrid.SelectedIndex].Id);
            UpdateDataGrid();
        }

        
    }
}
