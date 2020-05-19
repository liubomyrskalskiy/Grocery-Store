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
    /// Interaction logic for GoodsInMarketPage.xaml
    /// </summary>
    public partial class GoodsInMarketPage : Page, IActivable
    {
        private readonly IGoodsInMarketService _goodsInMarketService;
        private IProducerService _producerService;
        private readonly IGoodsService _goodsService;
        private readonly IMarketService _marketService;
        private AppSettings _settings;
        private readonly IMapper _mapper;
        private EmployeeDTO _currentEmployee;


        public List<GoodsInMarketDTO> GoodsInMarketDtos { get; set; }
        public List<MarketDTO> MarketDtos { get; set; }
        public List<ProducerDTO> ProducerDtos { get; set; }
        public List<GoodsInMarketDTO> FilteredGoodsInMarketDtos { get; set; }

        public GoodsInMarketPage(IGoodsInMarketService goodsInMarketService, IGoodsService goodsService,
            IMarketService marketService, IOptions<AppSettings> settings, IMapper mapper, IProducerService producerService)
        {
            _goodsInMarketService = goodsInMarketService;
            _goodsService = goodsService;
            _marketService = marketService;
            _mapper = mapper;
            _producerService = producerService;
            _settings = settings.Value;

            InitializeComponent();
        }

        private void UpdateDataGrid()
        {
            GoodsInMarketDtos =
                _mapper.Map<List<GoodsInMarket>, List<GoodsInMarketDTO>>(_goodsInMarketService.GetAll());

            MarketComboBox.ItemsSource = MarketDtos;

            FilteredGoodsInMarketDtos = GoodsInMarketDtos;

            if (ProducerFilterComboBox.SelectedItem != null && MarketFilterComboBox.SelectedItem != null)
            {
                var tempProducer = (ProducerDTO) ProducerFilterComboBox.SelectedItem;
                var tempMarket = (MarketDTO) MarketFilterComboBox.SelectedItem;
                FilteredGoodsInMarketDtos = GoodsInMarketDtos.Where(item => item.ProducerTitle == tempProducer.Title && item.FullMarketAddress == tempMarket.FullAddress).ToList();
            }

            if (ProducerFilterComboBox.SelectedItem != null && MarketFilterComboBox.SelectedItem == null)
            {
                var tempProducer = (ProducerDTO)ProducerFilterComboBox.SelectedItem;
                FilteredGoodsInMarketDtos = GoodsInMarketDtos.Where(item => item.ProducerTitle == tempProducer.Title).ToList();
            }

            if (ProducerFilterComboBox.SelectedItem == null && MarketFilterComboBox.SelectedItem != null)
            {
                var tempMarket = (MarketDTO)MarketFilterComboBox.SelectedItem;
                FilteredGoodsInMarketDtos = GoodsInMarketDtos.Where(item => item.FullMarketAddress == tempMarket.FullAddress).ToList();
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

        public Task ActivateAsync(object parameter)
        {
            _currentEmployee = (EmployeeDTO)parameter;
            MarketDtos = _mapper.Map<List<Market>, List<MarketDTO>>(_marketService.GetAll());
            ProducerDtos = _mapper.Map<List<Producer>, List<ProducerDTO>>(_producerService.GetAll());
            MarketFilterComboBox.ItemsSource = MarketDtos;
            ProducerFilterComboBox.ItemsSource = ProducerDtos;
            UpdateDataGrid();
            return Task.CompletedTask;
        }

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedIndex != -1)
            {
                ProductCodeTextBox.Text = FilteredGoodsInMarketDtos[DataGrid.SelectedIndex].ProductCode;
            }
        }

        private void ProductCodeTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!Regex.Match(ProductCodeTextBox.Text, @"^\d{5}$").Success)
            {
                return;
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
                    return;
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

            
            GoodsInMarket goodsInMarket = new GoodsInMarket();
            Goods tempGoods;
            var tempMarket = (MarketDTO)MarketComboBox.SelectedItem;
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
            else
                goodsInMarket.IdGoods = tempGoods.Id;
            
            goodsInMarket.IdMarket = tempMarket.Id;

            _goodsInMarketService.Create(goodsInMarket);
            UpdateDataGrid();
        }

        private void ClearFilterBtn_OnClick(object sender, RoutedEventArgs e)
        {
            ProducerFilterComboBox.SelectedItem = null;
            MarketFilterComboBox.SelectedItem = null;
            UpdateDataGrid();
        }

        private void ProducerFilterComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(ProducerFilterComboBox.SelectedItem != null)
            {
                UpdateDataGrid();
            }
        }

        private void MarketFilterComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MarketFilterComboBox.SelectedItem != null)
            {
                UpdateDataGrid();
            }
        }

        private void ClearProducerFilterBtn_OnClick(object sender, RoutedEventArgs e)
        {
            ProducerFilterComboBox.SelectedItem = null;
            UpdateDataGrid();
        }

        private void ClearMarketFilterBtn_OnClick(object sender, RoutedEventArgs e)
        {
            MarketFilterComboBox.SelectedItem = null;
            UpdateDataGrid();
        }
    }
}