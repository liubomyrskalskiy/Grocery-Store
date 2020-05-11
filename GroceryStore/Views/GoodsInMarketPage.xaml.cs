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

        public List<GoodsInMarketDTO> GoodsInMarketDtos { get; set; }

        public GoodsInMarketPage(IGoodsInMarketService goodsInMarketService, IGoodsService goodsService, IMarketService marketService, IOptions<AppSettings> settings, IMapper mapper)
        {
            _goodsInMarketService = goodsInMarketService;
            _goodsService = goodsService;
            _marketService = marketService;
            _mapper = mapper;
            _settings = settings.Value;

            InitializeComponent();

            UpdateDataGrid();
        }

        private void UpdateDataGrid()
        {
            GoodsInMarketDtos = _mapper.Map<List<GoodsInMarket>, List<GoodsInMarketDTO>>(_goodsInMarketService.GetAll());

            DataGrid.ItemsSource = GoodsInMarketDtos;
        }

        private bool ValidateForm()
        {
            if (!Regex.Match(ProductCodeTextBox.Text, @"^\d{5}$").Success)
            {
                MessageBox.Show("Invalid product code! It must contain 5 digits");
                AmountTextBox.Focus();
                return false;
            }

            if (!Regex.Match(AddressTextBox.Text, @"^(Вул\.\s\D{1,40}\,\s\d{1,3})$").Success)
            {
                MessageBox.Show("Market address must consist of at least 1 character and not exceed 50 characters!");
                AddressTextBox.Focus();
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
            return Task.CompletedTask;
        }

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedIndex != -1)
            {
                ProductCodeTextBox.Text = GoodsInMarketDtos[DataGrid.SelectedIndex].ProductCode;
                AmountTextBox.Text = GoodsInMarketDtos[DataGrid.SelectedIndex].Amount.ToString();
                AddressTextBox.Text = GoodsInMarketDtos[DataGrid.SelectedIndex].Address;
            }
        }

        private void CreateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;
            GoodsInMarket goodsInMarket = new GoodsInMarket();
            Goods tempGoods;
            Market tempMarket;

            goodsInMarket.Id = GoodsInMarketDtos[^1]?.Id + 1 ?? 1;
            goodsInMarket.Amount = Convert.ToDouble(AmountTextBox.Text);
            if ((tempGoods = _goodsService.GetAll()
                    .FirstOrDefault(goods => goods.ProductCode == ProductCodeTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such product in database!");
                return;
            }
            else
                goodsInMarket.IdGoods = tempGoods.Id;

            if ((tempMarket =
                    _marketService.GetAll().FirstOrDefault(market => market.Address == AddressTextBox.Text)) == null)
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
            if (DataGrid.SelectedIndex == -1) return;
            if (!ValidateForm()) return;
            GoodsInMarket goodsInMarket = new GoodsInMarket();
            Goods tempGoods;
            Market tempMarket;

            goodsInMarket.Id = GoodsInMarketDtos[DataGrid.SelectedIndex].Id;
            goodsInMarket.Amount = Convert.ToDouble(AmountTextBox.Text);
            if ((tempGoods = _goodsService.GetAll()
                    .FirstOrDefault(goods => goods.ProductCode == ProductCodeTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such product in database!");
                return;
            }
            else
                goodsInMarket.IdGoods = tempGoods.Id;

            if ((tempMarket =
                    _marketService.GetAll().FirstOrDefault(market => market.Address == AddressTextBox.Text)) == null)
            {
                MessageBox.Show("There is no market on such address in database!");
                return;
            }
            else
                goodsInMarket.IdMarket = tempMarket.Id;

            _goodsInMarketService.Update(goodsInMarket);
            UpdateDataGrid();
        }

        private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            _goodsInMarketService.Delete(GoodsInMarketDtos[DataGrid.SelectedIndex].Id);
            UpdateDataGrid();
        }

        
    }
}
