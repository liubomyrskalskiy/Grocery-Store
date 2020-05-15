using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AutoMapper;
using GroceryStore.Core.Abstractions;
using GroceryStore.Core.Abstractions.IServices;
using GroceryStore.Core.DTO;
using GroceryStore.Core.Models;
using GroceryStore.NavigationTransferObjects;
using GroceryStore.Windows;
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
        private readonly IProductionContentsService _productionContentsService;
        private readonly AppSettings _settings;
        private readonly IMapper _mapper;
        private readonly SimpleNavigationService _navigationService;
        private EmployeeDTO _currentEmployee;
        private ProductionNTO _productionNto;

        private List<ProductionDTO> ProductionDtos { get; set; }

        public ProductionPage(IProductionService productionService, IEmployeeService employeeService,
            IGoodsOwnService goodsOwnService, IOptions<AppSettings> settings, IMapper mapper,
            SimpleNavigationService navigationService, IProductionContentsService productionContentsService)
        {
            _productionService = productionService;
            _employeeService = employeeService;
            _goodsOwnService = goodsOwnService;
            _mapper = mapper;
            _navigationService = navigationService;
            _productionContentsService = productionContentsService;
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

            if (!Regex.Match(AmountTextBox.Text, @"^[0-9]*(?:\,[0-9]*)?$").Success)
            {
                MessageBox.Show("Invalid amount! Check the data you've entered!");
                AmountTextBox.Focus();
                return false;
            }

            return true;
        }

        public Task ActivateAsync(object parameter)
        {
            _currentEmployee = (EmployeeDTO) parameter;
            if (_currentEmployee.RoleTitle.Equals("Адміністратор"))
            {
                DeleteBtn.IsEnabled = true;
            }
            else
            {
                DeleteBtn.IsEnabled = false;
            }

            return Task.CompletedTask;
        }

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedIndex != -1)
            {
                ProductCodeTextBox.Text = ProductionDtos[DataGrid.SelectedIndex].ProductCode;
                AmountTextBox.Text = ProductionDtos[DataGrid.SelectedIndex].Amount.ToString();
            }
        }

        private void ProductCodeTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (Regex.Match(ProductCodeTextBox.Text, @"^\d{5}$").Success)
            {
                GoodsOwnDTO tempGoodsOwnDto;

                if ((tempGoodsOwnDto = _mapper.Map<GoodsOwn, GoodsOwnDTO>(_goodsOwnService.GetAll()
                        .FirstOrDefault(item => item.ProductCode == ProductCodeTextBox.Text))) != null)
                {
                    GoodTitleLabel.Content = "Good: " + tempGoodsOwnDto.Title;
                    CategoryLabel.Content = "Category: " + tempGoodsOwnDto.Category;
                    WeightLabel.Content = "Unit weight: " + tempGoodsOwnDto.Weight;
                    PriceLabel.Content = "Price: " + $"{tempGoodsOwnDto.Price,0:C2}";
                }
            }
            else
            {
                GoodTitleLabel.Content = "";
                CategoryLabel.Content = "";
                WeightLabel.Content = "";
                PriceLabel.Content = "";
            }
        }

        private async void CreateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;
            Production production = new Production();
            GoodsOwn tempGoodsOwn;
            production.Id = ProductionDtos[^1]?.Id + 1 ?? 1;
            production.ProductionCode = production.Id.ToString("D10");
            production.IdEmployee = _currentEmployee.Id;
            if ((tempGoodsOwn = _goodsOwnService.GetAll()
                    .FirstOrDefault(goodsOwn => goodsOwn.ProductCode == ProductCodeTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such own product in database!");
                return;
            }
            else
                production.IdGoodsOwn = tempGoodsOwn.Id;

            production.ManufactureDate = DateTime.Now;
            production.BestBefore = DateTime.Now.AddDays(1);
            production.Amount = Convert.ToInt32(AmountTextBox.Text);
            production.TotalCost = 0;

            _productionNto = new ProductionNTO(production, _currentEmployee);

            _productionService.Create(production);

            var result = await _navigationService.ShowDialogAsync<ProductionWindow>(_productionNto);

            UpdateDataGrid();
        }

        private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            List<ProductionContents> productionContents = _productionContentsService.GetAll()
                .Where(item => item.IdProduction == ProductionDtos[DataGrid.SelectedIndex].Id).ToList();
            foreach (var productionContent in productionContents)
            {
                _productionContentsService.Delete(productionContent.Id);
            }
            _productionService.Delete(ProductionDtos[DataGrid.SelectedIndex].Id);
            UpdateDataGrid();
        }

        private async void DataGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            await _navigationService.ShowDialogAsync<ProductionDetailWindow>(ProductionDtos[DataGrid.SelectedIndex].ProductionCode);
        }
    }
}