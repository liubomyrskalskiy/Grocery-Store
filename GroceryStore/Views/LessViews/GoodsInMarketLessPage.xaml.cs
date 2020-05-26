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
    ///     Interaction logic for GoodsInMarketLessPage.xaml
    /// </summary>
    public partial class GoodsInMarketLessPage : Page, IActivable
    {
        private readonly ICategoryService _categoryService;
        private readonly IGoodsInMarketService _goodsInMarketService;
        private readonly IMapper _mapper;
        private readonly IProducerService _producerService;
        private readonly AppSettings _settings;
        private EmployeeDTO _currentEmployee;

        public GoodsInMarketLessPage(IGoodsInMarketService goodsInMarketService, IOptions<AppSettings> settings,
            IMapper mapper, IProducerService producerService, ICategoryService categoryService)
        {
            _goodsInMarketService = goodsInMarketService;
            _mapper = mapper;
            _producerService = producerService;
            _categoryService = categoryService;
            _settings = settings.Value;

            InitializeComponent();
        }

        public List<GoodsInMarketDTO> GoodsInMarketDtos { get; set; }
        public List<GoodsInMarketDTO> GoodsInCurrentMarketDtos { get; set; }
        public List<GoodsInMarketDTO> FilteredGoodsInCurrentMarketDtos { get; set; }
        public List<CategoryDTO> CategoryDtos { get; set; }
        public List<ProducerDTO> ProducerDtos { get; set; }

        public Task ActivateAsync(object parameter)
        {
            _currentEmployee = (EmployeeDTO) parameter;
            CategoryDtos = _mapper.Map<List<Category>, List<CategoryDTO>>(_categoryService.GetAll());
            CategoryFilterComboBox.ItemsSource = CategoryDtos;
            ProducerDtos = _mapper.Map<List<Producer>, List<ProducerDTO>>(_producerService.GetAll());
            ProducerFilterComboBox.ItemsSource = ProducerDtos;
            UpdateDataGrid();
            return Task.CompletedTask;
        }

        private void UpdateDataGrid()
        {
            GoodsInMarketDtos =
                _mapper.Map<List<GoodsInMarket>, List<GoodsInMarketDTO>>(_goodsInMarketService.GetAll());

            GoodsInMarketDtos.Sort(delegate (GoodsInMarketDTO x, GoodsInMarketDTO y)
            {
                return x.Id.CompareTo(y.Id);
            });

            GoodsInCurrentMarketDtos = GoodsInMarketDtos
                .Where(item => item.MarketAddress == _currentEmployee.MarketAddress)
                .ToList();
            FilteredGoodsInCurrentMarketDtos = GoodsInCurrentMarketDtos;

            FilteredGoodsInCurrentMarketDtos.Sort(delegate (GoodsInMarketDTO x, GoodsInMarketDTO y)
            {
                return Convert.ToInt32(x.ProductCode).CompareTo(Convert.ToInt32(y.ProductCode));
            });

            if (Regex.Match(TitleFilterTextBox.Text, @"^\D{1,20}$").Success)
            {
                var tempList = FilteredGoodsInCurrentMarketDtos
                    .Where(item => item.Good.Contains(TitleFilterTextBox.Text)).ToList();
                FilteredGoodsInCurrentMarketDtos = tempList;
            }

            if (CategoryFilterComboBox.SelectedItem != null)
            {
                var tempCategory = (CategoryDTO) CategoryFilterComboBox.SelectedItem;
                var tempList = FilteredGoodsInCurrentMarketDtos.Where(item => item.Category == tempCategory.Title)
                    .ToList();
                FilteredGoodsInCurrentMarketDtos = tempList;
            }

            if (ProducerFilterComboBox.SelectedItem != null)
            {
                var tempProducer = (ProducerDTO) ProducerFilterComboBox.SelectedItem;
                var tempList = FilteredGoodsInCurrentMarketDtos.Where(item => item.Producer == tempProducer.Title)
                    .ToList();
                FilteredGoodsInCurrentMarketDtos = tempList;
            }

            DataGrid.ItemsSource = FilteredGoodsInCurrentMarketDtos;
        }

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedIndex != -1)
            {
                TitleTextBox.Text = FilteredGoodsInCurrentMarketDtos[DataGrid.SelectedIndex].Good;
                ProducerTextBox.Text = FilteredGoodsInCurrentMarketDtos[DataGrid.SelectedIndex].Producer;
                ProductCodeTextBox.Text = FilteredGoodsInCurrentMarketDtos[DataGrid.SelectedIndex].ProductCode;
            }
        }

        private void ProducerFilterComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProducerFilterComboBox.SelectedItem != null) UpdateDataGrid();
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

        private void ClearProducerFilterBtn_OnClick(object sender, RoutedEventArgs e)
        {
            ProducerFilterComboBox.SelectedItem = null;
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