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
    /// Interaction logic for GoodsInMarketOwnPage.xaml
    /// </summary>
    public partial class GoodsInMarketOwnPage : Page, IActivable
    {
        private readonly IGoodsInMarketOwnService _goodsInMarketOwnService;
        private readonly IMarketService _marketService;
        private readonly IProductionService _productionService;
        private AppSettings _settings;
        private readonly IMapper _mapper;

        public List<GoodsInMarketOwnDTO> GoodsInMarketOwnDtos { get; set; }

        public GoodsInMarketOwnPage(IGoodsInMarketOwnService goodsInMarketOwnService, IMarketService marketService, IProductionService productionService, IOptions<AppSettings> settings, IMapper mapper)
        {
            _goodsInMarketOwnService = goodsInMarketOwnService;
            _marketService = marketService;
            _productionService = productionService;
            _settings = settings.Value;
            _mapper = mapper;

            InitializeComponent();

            UpdateDataGrid();
        }

        private void UpdateDataGrid()
        {
            GoodsInMarketOwnDtos = _mapper.Map<List<GoodsInMarketOwn>, List<GoodsInMarketOwnDTO>>(_goodsInMarketOwnService.GetAll());

            DataGrid.ItemsSource = GoodsInMarketOwnDtos;
        }

        private bool ValidateForm()
        {
            if (!Regex.Match(ProductionCodeTextBox.Text, @"^\d{5,10}$").Success)
            {
                MessageBox.Show("Invalid production code! It must contain at least 5 digits and not exceed 10 digits");
                ProductionCodeTextBox.Focus();
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
                ProductionCodeTextBox.Text = GoodsInMarketOwnDtos[DataGrid.SelectedIndex].ProductionCode;
                AmountTextBox.Text = GoodsInMarketOwnDtos[DataGrid.SelectedIndex].Amount.ToString();
                AddressTextBox.Text = GoodsInMarketOwnDtos[DataGrid.SelectedIndex].Address;
            }
        }

        private void CreateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;
            GoodsInMarketOwn goodsInMarketOwn = new GoodsInMarketOwn();
            Production temProduction = new Production();
            Market tempMarket = new Market();

            goodsInMarketOwn.Id = GoodsInMarketOwnDtos[^1]?.Id + 1 ?? 1;
            goodsInMarketOwn.Amount = Convert.ToDouble(AmountTextBox.Text);
            if ((temProduction = _productionService.GetAll()
                    .FirstOrDefault(goods => goods.ProductionCode == ProductionCodeTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such product in database!");
                return;
            }
            else
                goodsInMarketOwn.IdProduction = temProduction.Id;

            if ((tempMarket =
                    _marketService.GetAll().FirstOrDefault(market => market.Address == AddressTextBox.Text)) == null)
            {
                MessageBox.Show("There is no market on such address in database!");
                return;
            }
            else
                goodsInMarketOwn.IdMarket = tempMarket.Id;

            _goodsInMarketOwnService.Create(goodsInMarketOwn);
            UpdateDataGrid();
        }

        private void UpdateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            if (!ValidateForm()) return;
            GoodsInMarketOwn goodsInMarketOwn = new GoodsInMarketOwn();
            Production temProduction = new Production();
            Market tempMarket = new Market();

            goodsInMarketOwn.Id = GoodsInMarketOwnDtos[DataGrid.SelectedIndex].Id;
            goodsInMarketOwn.Amount = Convert.ToDouble(AmountTextBox.Text);
            if ((temProduction = _productionService.GetAll()
                    .FirstOrDefault(goods => goods.ProductionCode == ProductionCodeTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such product in database!");
                return;
            }
            else
                goodsInMarketOwn.IdProduction = temProduction.Id;

            if ((tempMarket =
                    _marketService.GetAll().FirstOrDefault(market => market.Address == AddressTextBox.Text)) == null)
            {
                MessageBox.Show("There is no market on such address in database!");
                return;
            }
            else
                goodsInMarketOwn.IdMarket = tempMarket.Id;

            _goodsInMarketOwnService.Update(goodsInMarketOwn);
            UpdateDataGrid();
        }

        private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            _goodsInMarketOwnService.Delete(GoodsInMarketOwnDtos[DataGrid.SelectedIndex].Id);
            UpdateDataGrid();
        }
    }
}
