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
        private readonly IGoodsService _goodsService;
        private readonly IMarketService _marketService;
        private AppSettings _settings;
        private readonly IMapper _mapper;
        private EmployeeDTO _currentEmployee;


        public List<GoodsInMarketDTO> GoodsInMarketDtos { get; set; }

        public List<GoodsInMarketDTO> GoodsInCurrentMarketDtos { get; set; }

        public GoodsInMarketPage(IGoodsInMarketService goodsInMarketService, IGoodsService goodsService,
            IMarketService marketService, IOptions<AppSettings> settings, IMapper mapper)
        {
            _goodsInMarketService = goodsInMarketService;
            _goodsService = goodsService;
            _marketService = marketService;
            _mapper = mapper;
            _settings = settings.Value;

            InitializeComponent();
        }

        private void UpdateDataGrid()
        {
            GoodsInMarketDtos =
                _mapper.Map<List<GoodsInMarket>, List<GoodsInMarketDTO>>(_goodsInMarketService.GetAll());

            GoodsInCurrentMarketDtos = GoodsInMarketDtos.Where(item => item.Address == _currentEmployee.MarketAddress).ToList();

            DataGrid.ItemsSource = GoodsInCurrentMarketDtos;
        }

        private bool ValidateForm()
        {
            if (!Regex.Match(ProductCodeTextBox.Text, @"^\d{5}$").Success)
            {
                MessageBox.Show("Invalid product code! It must contain 5 digits");
                ProductCodeTextBox.Focus();
                return false;
            }

            return true;
        }

        public Task ActivateAsync(object parameter)
        {
            _currentEmployee = (EmployeeDTO)parameter;
            UpdateDataGrid();
            return Task.CompletedTask;
        }

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedIndex != -1)
            {
                ProductCodeTextBox.Text = GoodsInCurrentMarketDtos[DataGrid.SelectedIndex].ProductCode;
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
            Market tempMarket;

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

            if ((tempMarket =
                    _marketService.GetAll().FirstOrDefault(market => market.Address == _currentEmployee.MarketAddress)) == null)
            {
                MessageBox.Show("There is no market on such address in database!");
                return;
            }
            else
                goodsInMarket.IdMarket = tempMarket.Id;

            _goodsInMarketService.Create(goodsInMarket);
            UpdateDataGrid();
        }

        private void UpdateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            //if (DataGrid.SelectedIndex == -1) return;
            //if (!ValidateForm()) return;
            //GoodsInMarket goodsInMarket = new GoodsInMarket();
            //Goods tempGoods;
            //Market tempMarket;

            //goodsInMarket.Id = GoodsInCurrentMarketDtos[DataGrid.SelectedIndex].Id;
            //goodsInMarket.Amount = Convert.ToDouble(AmountTextBox.Text);
            //if ((tempGoods = _goodsService.GetAll()
            //        .FirstOrDefault(goods => goods.ProductCode == ProductCodeTextBox.Text)) == null)
            //{
            //    MessageBox.Show("There is no such product in database!");
            //    return;
            //}
            //else
            //    goodsInMarket.IdGoods = tempGoods.Id;

            //if ((tempMarket =
            //        _marketService.GetAll().FirstOrDefault(market => market.Address == _marketAddress)) == null)
            //{
            //    MessageBox.Show("There is no market on such address in database!");
            //    return;
            //}
            //else
            //    goodsInMarket.IdMarket = tempMarket.Id;

            //_goodsInMarketService.Update(goodsInMarket);
            //UpdateDataGrid();
        }

        private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            _goodsInMarketService.Delete(GoodsInCurrentMarketDtos[DataGrid.SelectedIndex].Id);
            UpdateDataGrid();
        }
    }
}