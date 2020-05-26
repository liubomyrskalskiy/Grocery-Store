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

namespace GroceryStore.Views.LessViews
{
    /// <summary>
    ///     Interaction logic for GoodsInMarketOwnLessPage.xaml
    /// </summary>
    public partial class GoodsInMarketOwnLessPage : Page, IActivable
    {
        private readonly ICategoryService _categoryService;
        private readonly IGoodsInMarketOwnService _goodsInMarketOwnService;
        private readonly IMapper _mapper;
        private readonly IProductionService _productionService;
        private readonly AppSettings _settings;
        private EmployeeDTO _currentEmployee;

        public GoodsInMarketOwnLessPage(IGoodsInMarketOwnService goodsInMarketOwnService,
            IOptions<AppSettings> settings, IMapper mapper, ICategoryService categoryService,
            IProductionService productionService)
        {
            _goodsInMarketOwnService = goodsInMarketOwnService;
            _settings = settings.Value;
            _mapper = mapper;
            _categoryService = categoryService;
            _productionService = productionService;

            InitializeComponent();
        }

        public List<GoodsInMarketOwnDTO> GoodsInMarketOwnDtos { get; set; }
        public List<GoodsInMarketOwnDTO> FilteredGoodsInMarketOwnDtos { get; set; }
        public List<CategoryDTO> CategoryDtos { get; set; }
        public List<ProductionDTO> ProductionDtos { get; set; }

        public Task ActivateAsync(object parameter)
        {
            _currentEmployee = (EmployeeDTO) parameter;
            CategoryDtos = _mapper.Map<List<Category>, List<CategoryDTO>>(_categoryService.GetAll());
            CategoryFilterComboBox.ItemsSource = CategoryDtos;
            ProductionDtos = _mapper.Map<List<Production>, List<ProductionDTO>>(_productionService.GetAll());
            ManufactureDateFilterComboBox.ItemsSource =
                ProductionDtos.GroupBy(item => item.ManufactureDate).Select(item => item.First());
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

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedIndex != -1)
            {
                ProductCodeTextBox.Text = FilteredGoodsInMarketOwnDtos[DataGrid.SelectedIndex].ProductCode;
                TitleTextBox.Text = FilteredGoodsInMarketOwnDtos[DataGrid.SelectedIndex].Good;
                ManufactureDateTextBox.Text =
                    FilteredGoodsInMarketOwnDtos[DataGrid.SelectedIndex].ManufactureDate.ToString();
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