using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for GoodsInMarketLessPage.xaml
    /// </summary>
    public partial class GoodsInMarketLessPage : Page, IActivable
    {
        private readonly IGoodsInMarketService _goodsInMarketService;
        private IProducerService _producerService;
        private ICategoryService _categoryService;
        private readonly AppSettings _settings;
        private readonly IMapper _mapper;
        private EmployeeDTO _currentEmployee;

        public List<GoodsInMarketDTO> GoodsInMarketDtos { get; set; }
        public List<GoodsInMarketDTO> GoodsInCurrentMarketDtos { get; set; }
        public List<GoodsInMarketDTO> FilteredGoodsInCurrentMarketDtos { get; set; }
        public List<CategoryDTO> CategoryDtos { get; set; }
        public List<ProducerDTO> ProducerDtos { get; set; }

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

        private void UpdateDataGrid()
        {
            GoodsInMarketDtos =
                _mapper.Map<List<GoodsInMarket>, List<GoodsInMarketDTO>>(_goodsInMarketService.GetAll());
            GoodsInCurrentMarketDtos = GoodsInMarketDtos.Where(item => item.MarketAddress == _currentEmployee.MarketAddress)
                .ToList();
            FilteredGoodsInCurrentMarketDtos = GoodsInCurrentMarketDtos;

            if (CategoryFilterComboBox.SelectedItem != null)
            {
                var tempCategory = (CategoryDTO) CategoryFilterComboBox.SelectedItem;
                var tempList = FilteredGoodsInCurrentMarketDtos.Where(item => item.Category == tempCategory.Title).ToList();
                FilteredGoodsInCurrentMarketDtos = tempList;
            }

            if (ProducerFilterComboBox.SelectedItem != null)
            {
                var tempProducer = (ProducerDTO) ProducerFilterComboBox.SelectedItem;
                var tempList = FilteredGoodsInCurrentMarketDtos.Where(item => item.Producer == tempProducer.Title).ToList();
                FilteredGoodsInCurrentMarketDtos = tempList;
            }
            DataGrid.ItemsSource = FilteredGoodsInCurrentMarketDtos;
        }

        public Task ActivateAsync(object parameter)
        {
            _currentEmployee = (EmployeeDTO)parameter;
            CategoryDtos = _mapper.Map<List<Category>, List<CategoryDTO>>(_categoryService.GetAll());
            CategoryFilterComboBox.ItemsSource = CategoryDtos;
            ProducerDtos = _mapper.Map<List<Producer>, List<ProducerDTO>>(_producerService.GetAll());
            ProducerFilterComboBox.ItemsSource = ProducerDtos;
            UpdateDataGrid();
            return Task.CompletedTask;
        }

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedIndex != -1)
            {
                TitleTextBox.Text = GoodsInCurrentMarketDtos[DataGrid.SelectedIndex].Good;
                ProducerTextBox.Text = GoodsInCurrentMarketDtos[DataGrid.SelectedIndex].Producer;
                ProductCodeTextBox.Text = GoodsInCurrentMarketDtos[DataGrid.SelectedIndex].ProductCode;
            }
        }

        private void ProducerFilterComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(ProducerFilterComboBox.SelectedItem!= null)
            {
                UpdateDataGrid();
            }
        }

        private void CategoryFilterComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CategoryFilterComboBox.SelectedItem != null)
            {
                UpdateDataGrid();
            }
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
    }
}