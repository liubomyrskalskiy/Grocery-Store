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
    ///     Interaction logic for GoodsInMarketPage.xaml
    /// </summary>
    public partial class GoodsInMarketPage : Page, IActivable
    {
        private readonly ICategoryService _categoryService;
        private readonly IGoodsInMarketService _goodsInMarketService;
        private readonly IGoodsService _goodsService;
        private readonly IMapper _mapper;
        private readonly IMarketService _marketService;
        private readonly IProducerService _producerService;
        private readonly AppSettings _settings;
        private EmployeeDTO _currentEmployee;

        public GoodsInMarketPage(IGoodsInMarketService goodsInMarketService, IGoodsService goodsService,
            IMarketService marketService, IOptions<AppSettings> settings, IMapper mapper,
            IProducerService producerService, ICategoryService categoryService)
        {
            _goodsInMarketService = goodsInMarketService;
            _goodsService = goodsService;
            _marketService = marketService;
            _mapper = mapper;
            _producerService = producerService;
            _categoryService = categoryService;
            _settings = settings.Value;

            InitializeComponent();
        }


        public List<GoodsInMarketDTO> GoodsInMarketDtos { get; set; }
        public List<MarketDTO> MarketDtos { get; set; }
        public List<CategoryDTO> CategoryDtos { get; set; }
        public List<ProducerDTO> ProducerDtos { get; set; }
        public List<GoodsInMarketDTO> FilteredGoodsInMarketDtos { get; set; }

        public Task ActivateAsync(object parameter)
        {
            _currentEmployee = (EmployeeDTO) parameter;
            MarketDtos = _mapper.Map<List<Market>, List<MarketDTO>>(_marketService.GetAll());
            ProducerDtos = _mapper.Map<List<Producer>, List<ProducerDTO>>(_producerService.GetAll());
            CategoryDtos = _mapper.Map<List<Category>, List<CategoryDTO>>(_categoryService.GetAll());
            MarketComboBox.ItemsSource = MarketDtos;
            MarketFilterComboBox.ItemsSource = MarketDtos;
            MarketFilterComboBox.SelectedItem = MarketDtos[0];
            CategoryFilterComboBox.ItemsSource = CategoryDtos;
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

            FilteredGoodsInMarketDtos = GoodsInMarketDtos;

            FilteredGoodsInMarketDtos.Sort(delegate (GoodsInMarketDTO x, GoodsInMarketDTO y)
            {
                return Convert.ToInt32(x.ProductCode).CompareTo(Convert.ToInt32(y.ProductCode));
            });

            if (MarketFilterComboBox.SelectedItem != null)
            {
                var tempMarket = (MarketDTO) MarketFilterComboBox.SelectedItem;
                var tempList = FilteredGoodsInMarketDtos.Where(item => item.FullMarketAddress == tempMarket.FullAddress)
                    .ToList();
                FilteredGoodsInMarketDtos = tempList;
            }

            if (Regex.Match(TitleFilterTextBox.Text, @"^\D{1,20}$").Success)
            {
                var tempList = FilteredGoodsInMarketDtos.Where(item => item.Good.Contains(TitleFilterTextBox.Text))
                    .ToList();
                FilteredGoodsInMarketDtos = tempList;
            }

            if (CategoryFilterComboBox.SelectedItem != null)
            {
                var tempCategory = (CategoryDTO) CategoryFilterComboBox.SelectedItem;
                var tempList = FilteredGoodsInMarketDtos.Where(item => item.Category == tempCategory.Title).ToList();
                FilteredGoodsInMarketDtos = tempList;
            }

            if (ProducerFilterComboBox.SelectedItem != null)
            {
                var tempProducer = (ProducerDTO) ProducerFilterComboBox.SelectedItem;
                var tempList = FilteredGoodsInMarketDtos.Where(item => item.Producer == tempProducer.Title).ToList();
                FilteredGoodsInMarketDtos = tempList;
            }

            DataGrid.ItemsSource = FilteredGoodsInMarketDtos;
        }

        private bool ValidateForm()
        {
            if (!Regex.Match(ProductCodeTextBox.Text, @"^\d{5}$").Success)
            {
                MessageBox.Show("Invalid product code! It must contain 5 digits");
                ProductCodeTextBox.Focus();
                return false;
            }

            if (MarketComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select market!");
                return false;
            }

            return true;
        }

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedIndex != -1)
                ProductCodeTextBox.Text = FilteredGoodsInMarketDtos[DataGrid.SelectedIndex].ProductCode;
        }

        private void ProductCodeTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!Regex.Match(ProductCodeTextBox.Text, @"^\d{5}$").Success)
            {
            }
            else
            {
                GoodsDTO goodsDto;
                Goods tempgood;
                if ((tempgood = _goodsService.GetAll()
                        .FirstOrDefault(gim =>
                            gim.ProductCode == ProductCodeTextBox.Text)) == null)
                {
                    GoodTitleLabel.Content = "";
                    ProducerTitleLabel.Content = "";
                    WeightLabel.Content = "";
                    PriceLabel.Content = "";
                }
                else
                {
                    goodsDto = _mapper.Map<Goods, GoodsDTO>(tempgood);
                    GoodTitleLabel.Content = "Good: " + goodsDto.Title;
                    ProducerTitleLabel.Content = "Producer: " + goodsDto.ProducerTitle;
                    WeightLabel.Content = "Unit weight: " + goodsDto.Weight;
                    PriceLabel.Content = "Price: " + $"{goodsDto.Price,0:C2}";
                }
            }
        }

        private void CreateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;


            var goodsInMarket = new GoodsInMarket();
            Goods tempGoods;
            var tempMarket = (MarketDTO) MarketComboBox.SelectedItem;
            if (GoodsInMarketDtos.FirstOrDefault(item =>
                    item.ProductCode == ProductCodeTextBox.Text && item.FullMarketAddress == tempMarket.FullAddress) !=
                null)
            {
                MessageBox.Show("Such good is already in this market");
                return;
            }

            goodsInMarket.Id = GoodsInMarketDtos[^1]?.Id + 1 ?? 1;
            goodsInMarket.Amount = 0;
            if ((tempGoods = _goodsService.GetAll()
                    .FirstOrDefault(goods => goods.ProductCode == ProductCodeTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such product in database!");
                return;
            }

            goodsInMarket.IdGoods = tempGoods.Id;

            goodsInMarket.IdMarket = tempMarket.Id;

            _goodsInMarketService.Create(goodsInMarket);
            UpdateDataGrid();
        }

        private void ProducerFilterComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProducerFilterComboBox.SelectedItem != null) UpdateDataGrid();
        }

        private void MarketFilterComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MarketFilterComboBox.SelectedItem != null) UpdateDataGrid();
        }

        private void CategoryFilterComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CategoryFilterComboBox.SelectedItem != null) UpdateDataGrid();
        }

        private void ClearProducerFilterBtn_OnClick(object sender, RoutedEventArgs e)
        {
            ProducerFilterComboBox.SelectedItem = null;
            UpdateDataGrid();
        }

        private void ClearCategoryFilterBtn_OnClick(object sender, RoutedEventArgs e)
        {
            CategoryFilterComboBox.SelectedItem = null;
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