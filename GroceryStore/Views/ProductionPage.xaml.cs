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
    ///     Interaction logic for ProductionPage.xaml
    /// </summary>
    public partial class ProductionPage : Page, IActivable
    {
        private readonly ICategoryService _categoryService;
        private readonly IEmployeeService _employeeService;
        private readonly IGoodsOwnService _goodsOwnService;
        private readonly IGoodsInMarketOwnService _goodsInMarketOwnService;
        private readonly IGoodsWriteOffOwnService _goodsWriteOffOwnService;
        private readonly IMapper _mapper;
        private readonly SimpleNavigationService _navigationService;
        private readonly IProductionContentsService _productionContentsService;
        private readonly IProductionService _productionService;
        private readonly AppSettings _settings;
        private EmployeeDTO _currentEmployee;
        private ProductionNTO _productionNto;

        public ProductionPage(IProductionService productionService, IEmployeeService employeeService,
            IGoodsOwnService goodsOwnService, IOptions<AppSettings> settings, IMapper mapper,
            SimpleNavigationService navigationService, IProductionContentsService productionContentsService,
            ICategoryService categoryService, IGoodsInMarketOwnService goodsInMarketOwnService, IGoodsWriteOffOwnService goodsWriteOffOwnService)
        {
            _productionService = productionService;
            _employeeService = employeeService;
            _goodsOwnService = goodsOwnService;
            _mapper = mapper;
            _navigationService = navigationService;
            _productionContentsService = productionContentsService;
            _settings = settings.Value;
            _categoryService = categoryService;
            _goodsInMarketOwnService = goodsInMarketOwnService;
            _goodsWriteOffOwnService = goodsWriteOffOwnService;
            InitializeComponent();

            UpdateDataGrid();
        }

        private List<ProductionDTO> ProductionDtos { get; set; }
        private List<ProductionDTO> FilteredProductionDtos { get; set; }
        public List<CategoryDTO> CategoryDtos { get; set; }

        public Task ActivateAsync(object parameter)
        {
            _currentEmployee = (EmployeeDTO) parameter;
            if (_currentEmployee.RoleTitle.Equals("Адміністратор"))
                DeleteBtn.IsEnabled = true;
            else
                DeleteBtn.IsEnabled = false;

            CategoryDtos = _mapper.Map<List<Category>, List<CategoryDTO>>(_categoryService.GetAll());
            CategoryFilterComboBox.ItemsSource = CategoryDtos;

            return Task.CompletedTask;
        }

        private void UpdateDataGrid()
        {
            ProductionDtos = _mapper.Map<List<Production>, List<ProductionDTO>>(_productionService.GetAll());

            ProductionDtos.Sort(delegate (ProductionDTO x, ProductionDTO y)
            {
                return x.Id.CompareTo(y.Id);
            });

            FilteredProductionDtos = ProductionDtos;

            if (Regex.Match(TitleFilterTextBox.Text, @"^\D{1,20}$").Success)
            {
                var tempList = FilteredProductionDtos.Where(item => item.Title.Contains(TitleFilterTextBox.Text))
                    .ToList();
                FilteredProductionDtos = tempList;
            }

            if (DateFromFilterTextBox.Text != "")
            {
                var tempDate = DateTime.Parse(DateFromFilterTextBox.Text);
                var tempList = FilteredProductionDtos
                    .Where(item => DateTime.Compare(item.ManufactureDate ?? default, tempDate) >= 0).ToList();
                FilteredProductionDtos = tempList;
            }

            if (DateToFilterTextBox.Text != "")
            {
                var tempDate = DateTime.Parse(DateToFilterTextBox.Text);
                var tempList = FilteredProductionDtos
                    .Where(item => DateTime.Compare(item.ManufactureDate ?? default, tempDate) <= 0).ToList();
                FilteredProductionDtos = tempList;
            }

            if (CategoryFilterComboBox.SelectedItem != null)
            {
                var tempCategoty = (CategoryDTO) CategoryFilterComboBox.SelectedItem;
                var tempList = FilteredProductionDtos.Where(item => item.Category == tempCategoty.Title).ToList();
                FilteredProductionDtos = tempList;
            }

            DataGrid.ItemsSource = FilteredProductionDtos;
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

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedIndex != -1)
            {
                ProductCodeTextBox.Text = FilteredProductionDtos[DataGrid.SelectedIndex].ProductCode;
                AmountTextBox.Text = FilteredProductionDtos[DataGrid.SelectedIndex].Amount.ToString();
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
            var production = new Production();
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
            if (_goodsInMarketOwnService.GetAll().FirstOrDefault(item => item.IdProduction == FilteredProductionDtos[DataGrid.SelectedIndex].Id) != null)
            {
                MessageBox.Show("You can only delete recently added rows!");
                return;
            }

            if (_goodsWriteOffOwnService.GetAll().FirstOrDefault(item => item.IdProduction == FilteredProductionDtos[DataGrid.SelectedIndex].Id) != null)
            {
                MessageBox.Show("You can only delete recently added rows!");
                return;
            }
            var productionContents = _productionContentsService.GetAll()
                .Where(item => item.IdProduction == FilteredProductionDtos[DataGrid.SelectedIndex].Id).ToList();
            foreach (var productionContent in productionContents)
                _productionContentsService.Delete(productionContent.Id);

            _productionService.Delete(FilteredProductionDtos[DataGrid.SelectedIndex].Id);
            UpdateDataGrid();
        }

        private async void DataGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            await _navigationService.ShowDialogAsync<ProductionDetailWindow>(
                FilteredProductionDtos[DataGrid.SelectedIndex].ProductionCode);
        }

        private void ClearTitleFilterBtn_OnClick(object sender, RoutedEventArgs e)
        {
            TitleFilterTextBox.Text = "";
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

        private void ClearDateFromFilterFilterBtn_OnClick(object sender, RoutedEventArgs e)
        {
            DateFromFilterTextBox.Text = "";
            UpdateDataGrid();
        }

        private void SearchDateFromFilterBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DateTime.TryParse(DateFromFilterTextBox.Text, out _))
            {
                UpdateDataGrid();
            }
            else
            {
                MessageBox.Show("Cannot parse date you've entered! Please check data you've entered");
                DateFromFilterTextBox.Focus();
            }
        }

        private void ClearDateToFilterBtn_OnClick(object sender, RoutedEventArgs e)
        {
            DateToFilterTextBox.Text = "";
            UpdateDataGrid();
        }

        private void SearchDateToFilterBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DateTime.TryParse(DateToFilterTextBox.Text, out _))
            {
                UpdateDataGrid();
            }
            else
            {
                MessageBox.Show("Cannot parse date you've entered! Please check data you've entered");
                DateToFilterTextBox.Focus();
            }
        }

        private void CategoryFilterComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CategoryFilterComboBox.SelectedItem != null) UpdateDataGrid();
        }

        private void ClearCategoryFilterBtn_OnClick(object sender, RoutedEventArgs e)
        {
            CategoryFilterComboBox.SelectedItem = null;
            UpdateDataGrid();
        }
    }
}