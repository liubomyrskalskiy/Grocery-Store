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
    ///     Interaction logic for GoodsInMarketOwnPage.xaml
    /// </summary>
    public partial class GoodsInMarketOwnPage : Page, IActivable
    {
        private readonly IBasketOwnService _basketOwnService;
        private readonly ICategoryService _categoryService;
        private readonly IGoodsInMarketOwnService _goodsInMarketOwnService;
        private readonly IGoodsOwnService _goodsOwnService;
        private readonly IGoodsWriteOffOwnService _goodsWriteOffOwnService;
        private readonly IMapper _mapper;
        private readonly IMarketService _marketService;
        private readonly IProductionService _productionService;
        private readonly AppSettings _settings;
        private EmployeeDTO _currentEmployee;

        public GoodsInMarketOwnPage(IGoodsInMarketOwnService goodsInMarketOwnService, IMarketService marketService,
            IProductionService productionService, IOptions<AppSettings> settings, IMapper mapper,
            IGoodsOwnService goodsOwnService, IBasketOwnService basketOwnService,
            IGoodsWriteOffOwnService goodsWriteOffOwnService, ICategoryService categoryService)
        {
            _goodsInMarketOwnService = goodsInMarketOwnService;
            _marketService = marketService;
            _productionService = productionService;
            _settings = settings.Value;
            _mapper = mapper;
            _goodsOwnService = goodsOwnService;
            _basketOwnService = basketOwnService;
            _goodsWriteOffOwnService = goodsWriteOffOwnService;
            _categoryService = categoryService;

            InitializeComponent();

            ProductionComboBox.IsEnabled = false;
        }

        public List<GoodsInMarketOwnDTO> GoodsInMarketOwnDtos { get; set; }
        public List<GoodsInMarketOwnDTO> FilteredGoodsInMarketOwnDtos { get; set; }
        public List<CategoryDTO> CategoryDtos { get; set; }
        public List<ProductionDTO> ProductionDtos { get; set; }
        public List<ProductionDTO> FilterProductionDtos { get; set; }

        public Task ActivateAsync(object parameter)
        {
            _currentEmployee = (EmployeeDTO) parameter;
            CategoryDtos = _mapper.Map<List<Category>, List<CategoryDTO>>(_categoryService.GetAll());
            CategoryFilterComboBox.ItemsSource = CategoryDtos;
            FilterProductionDtos = _mapper.Map<List<Production>, List<ProductionDTO>>(_productionService.GetAll());
            ManufactureDateFilterComboBox.ItemsSource = FilterProductionDtos.GroupBy(item => item.ManufactureDate)
                .Select(item => item.First());

            UpdateDataGrid();
            return Task.CompletedTask;
        }

        private void UpdateDataGrid()
        {
            GoodsInMarketOwnDtos =
                _mapper.Map<List<GoodsInMarketOwn>, List<GoodsInMarketOwnDTO>>(_goodsInMarketOwnService.GetAll());

            GoodsInMarketOwnDtos.Sort(delegate (GoodsInMarketOwnDTO x, GoodsInMarketOwnDTO y)
            {
                return x.Id.CompareTo(y.Id);
            });

            FilteredGoodsInMarketOwnDtos = GoodsInMarketOwnDtos
                .Where(item => item.Address == _currentEmployee.MarketAddress).ToList();

            FilteredGoodsInMarketOwnDtos.Sort(delegate (GoodsInMarketOwnDTO x, GoodsInMarketOwnDTO y)
            {
                return Convert.ToInt32(x.ProductCode).CompareTo(Convert.ToInt32(y.ProductCode));
            });

            if (Regex.Match(TitleFilterTextBox.Text, @"^\D{1,20}$").Success)
            {
                var tempList = FilteredGoodsInMarketOwnDtos.Where(item => item.Good.Contains(TitleFilterTextBox.Text))
                    .ToList();
                FilteredGoodsInMarketOwnDtos = tempList;
            }

            if (CategoryFilterComboBox.SelectedItem != null)
            {
                var tempCategory = (CategoryDTO) CategoryFilterComboBox.SelectedItem;
                var tempList = FilteredGoodsInMarketOwnDtos.Where(item => item.Category == tempCategory.Title).ToList();
                FilteredGoodsInMarketOwnDtos = tempList;
            }

            if (ManufactureDateFilterComboBox.SelectedItem != null)
            {
                var tempProduction = (ProductionDTO) ManufactureDateFilterComboBox.SelectedItem;
                var tempList = FilteredGoodsInMarketOwnDtos
                    .Where(item => item.ManufactureDate == tempProduction.ManufactureDate).ToList();
                FilteredGoodsInMarketOwnDtos = tempList;
            }

            DataGrid.ItemsSource = FilteredGoodsInMarketOwnDtos;
        }

        private bool ValidateForm()
        {
            if (!Regex.Match(AmountTextBox.Text, @"^[0-9]+$").Success)
            {
                MessageBox.Show("Invalid amount! Check the data you've entered!");
                AmountTextBox.Focus();
                return false;
            }

            return true;
        }

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedIndex != -1)
            {
                AmountTextBox.Text = FilteredGoodsInMarketOwnDtos[DataGrid.SelectedIndex].Amount;
                ProductionComboBox.SelectedItem = ProductionDtos.FirstOrDefault(item =>
                    item.ProductionCode == FilteredGoodsInMarketOwnDtos[DataGrid.SelectedIndex].ProductionCode);
            }
        }

        private void CreateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;
            var goodsInMarketOwn = new GoodsInMarketOwn();
            ProductionDTO tempProduction;
            Market tempMarket;

            goodsInMarketOwn.Id = GoodsInMarketOwnDtos[^1]?.Id + 1 ?? 1;
            goodsInMarketOwn.Amount = Convert.ToDouble(AmountTextBox.Text);
            if (ProductionComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select production date");
                return;
            }

            tempProduction = (ProductionDTO) ProductionComboBox.SelectedItem;
            if (tempProduction.Amount >= goodsInMarketOwn.Amount)
            {
                goodsInMarketOwn.IdProduction = tempProduction.Id;
            }
            else
            {
                MessageBox.Show("There is not enough goods with such manufacture date");
                return;
            }

            if ((tempMarket =
                    _marketService.GetAll()
                        .FirstOrDefault(market => market.Address == _currentEmployee.MarketAddress)) == null)
            {
                MessageBox.Show("There is no market on such address in database!");
                return;
            }

            goodsInMarketOwn.IdMarket = tempMarket.Id;

            _goodsInMarketOwnService.Create(goodsInMarketOwn);
            var currentProduction = _productionService.GetId(tempProduction.Id);
            currentProduction.Amount -= Convert.ToInt32(goodsInMarketOwn.Amount);
            _productionService.Update(currentProduction);
            UpdateDataGrid();
        }

        private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            if (_basketOwnService.GetAll().FirstOrDefault(item =>
                    item.IdGoodsInMarketOwn == FilteredGoodsInMarketOwnDtos[DataGrid.SelectedIndex].Id) != null)
            {
                MessageBox.Show("You can only delete recently added rows!");
                return;
            }

            if (_goodsWriteOffOwnService.GetAll().FirstOrDefault(item =>
                    item.IdGoodsInMarketOwn == FilteredGoodsInMarketOwnDtos[DataGrid.SelectedIndex].Id) != null)
            {
                MessageBox.Show("You can only delete recently added rows!");
                return;
            }

            _goodsInMarketOwnService.Delete(FilteredGoodsInMarketOwnDtos[DataGrid.SelectedIndex].Id);
            UpdateDataGrid();
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

                    if ((ProductionDtos = _mapper.Map<List<Production>, List<ProductionDTO>>(_productionService.GetAll()
                            .Where(item => item.IdGoodsOwnNavigation.ProductCode == ProductCodeTextBox.Text).ToList()))
                        .Count > 0)
                    {
                        ProductionComboBox.ItemsSource = ProductionDtos;
                        ProductionComboBox.IsEnabled = true;
                    }
                    else
                    {
                        ProductionDtos = null;
                        ProductionComboBox.ItemsSource = null;
                        ProductionComboBox.IsEnabled = false;
                    }
                }
            }
            else
            {
                GoodTitleLabel.Content = "";
                CategoryLabel.Content = "";
                WeightLabel.Content = "";
                PriceLabel.Content = "";

                ProductionDtos = null;
                ProductionComboBox.ItemsSource = null;
                ProductionComboBox.IsEnabled = false;
            }
        }

        private void ProductionComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProductionComboBox.SelectedItem != null)
            {
                var tempProduction = (ProductionDTO) ProductionComboBox.SelectedItem;
                AmountLabel.Content = "Amount: " + tempProduction.Amount;
            }
            else
            {
                AmountLabel.Content = "";
            }
        }

        private void CategoryFilterComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CategoryFilterComboBox.SelectedItem != null) UpdateDataGrid();
        }

        private void ManufactureDateFilterComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ManufactureDateFilterComboBox.SelectedItem != null) UpdateDataGrid();
        }

        private void ClearCategoryFilterBtn_OnClick(object sender, RoutedEventArgs e)
        {
            CategoryFilterComboBox.SelectedItem = null;
            UpdateDataGrid();
        }

        private void ClearManufactureDateFilterBtn_OnClick(object sender, RoutedEventArgs e)
        {
            ManufactureDateFilterComboBox.SelectedItem = null;
            UpdateDataGrid();
        }

        private void SearchTitleBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (Regex.Match(TitleFilterTextBox.Text, @"^\D{1,20}$").Success)
            {
                UpdateDataGrid();
            }
            else
            {
                MessageBox.Show("Title must consist of at least 1 character and not exceed 20 characters!");
                TitleFilterTextBox.Focus();
            }
        }

        private void ClearTitleFilterBtn_OnClick(object sender, RoutedEventArgs e)
        {
            TitleFilterTextBox.Text = "";
            UpdateDataGrid();
        }
    }
}