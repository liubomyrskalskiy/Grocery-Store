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
using GroceryStore.Core.Mapping;
using GroceryStore.Core.Models;
using Microsoft.Extensions.Options;

namespace GroceryStore.Views
{
    /// <summary>
    /// Interaction logic for BasketPage.xaml
    /// </summary>
    public partial class BasketPage : Page, IActivable
    {
        private readonly IBasketService _basketService;
        private readonly ISaleService _saleService;
        private readonly IGoodsInMarketService _goodsInMarketService;
        private AppSettings _settings;
        private readonly IMapper _mapper;

        public List<BasketDTO> BasketDtos { get; set; }

        public BasketPage(IBasketService basketService, ISaleService saleService, IGoodsInMarketService goodsInMarketService, IOptions<AppSettings> settings, IMapper mapper)
        {
            _basketService = basketService;
            _saleService = saleService;
            _goodsInMarketService = goodsInMarketService;
            _mapper = mapper;
            _settings = settings.Value;

            InitializeComponent();

            UpdateDataGrid();
        }

        private void UpdateDataGrid()
        {
            BasketDtos = _mapper.Map<List<Basket>, List<BasketDTO>>(_basketService.GetAll());

            DataGrid.ItemsSource = BasketDtos;
        }

        private bool ValidateForm()
        {
            if (!Regex.Match(AmountTextBox.Text, @"^[0-9]*(?:\,[0-9]*)?$").Success)
            {
                MessageBox.Show("Invalid amount! Check the data you've entered!");
                AmountTextBox.Focus();
                return false;
            }

            if (!Regex.Match(CheckNumberTextBox.Text, @"^\d{16}$").Success)
            {
                MessageBox.Show("Invalid check number! It must contain 16 digits");
                CheckNumberTextBox.Focus();
                return false;
            }
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
            return Task.CompletedTask;
        }

        private void CreateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;
            Basket basket = new Basket();
            Sale tempsale;
            GoodsInMarket tempgim;
            basket.Id = BasketDtos[^1]?.Id + 1 ?? 1;
            basket.Amount = Convert.ToDouble(AmountTextBox.Text);
            if ((tempsale = _saleService.GetAll()
                    .FirstOrDefault(sale => sale.CheckNumber == CheckNumberTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such check number!");
                return;
            }
            else
                basket.IdSale = tempsale.Id;
            if ((tempgim = _goodsInMarketService.GetAll()
                    .FirstOrDefault(gim => gim.IdGoodsNavigation.ProductCode == ProductCodeTextBox.Text && gim.Amount >= Convert.ToDouble(AmountTextBox.Text))) == null)
            {
                MessageBox.Show("There is no such product code or there is not enough goods in the store!");
                return;
            }
            else
                basket.IdGoodsInMarket = tempgim.Id;

            _basketService.Create(basket);
            UpdateDataGrid();
        }

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedIndex != -1)
            {
                AmountTextBox.Text = BasketDtos[DataGrid.SelectedIndex].Amount.ToString();
                CheckNumberTextBox.Text = BasketDtos[DataGrid.SelectedIndex].CheckNumber;
                ProductCodeTextBox.Text = BasketDtos[DataGrid.SelectedIndex].ProductCode;
            }
        }

        private void UpdateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex != -1)
            {
                if (!ValidateForm()) return;
                Basket basket = new Basket();
                Sale tempsale;
                GoodsInMarket tempgim;
                basket.Id = BasketDtos[DataGrid.SelectedIndex].Id;
                basket.Amount = Convert.ToDouble(AmountTextBox.Text);
                if ((tempsale = _saleService.GetAll()
                        .FirstOrDefault(sale => sale.CheckNumber == CheckNumberTextBox.Text)) == null)
                {
                    MessageBox.Show("There is no such check number!");
                    return;
                }
                else
                    basket.IdSale = tempsale.Id;
                if ((tempgim = _goodsInMarketService.GetAll()
                        .FirstOrDefault(gim => gim.IdGoodsNavigation.ProductCode == ProductCodeTextBox.Text && gim.Amount >= Convert.ToDouble(AmountTextBox.Text))) == null)
                {
                    MessageBox.Show("There is no such product code or there is not enough goods in the store!");
                    return;
                }
                else
                    basket.IdGoodsInMarket = tempgim.Id;

                _basketService.Update(basket);
                UpdateDataGrid();
            }
        }

        private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex != -1)
            {
                _basketService.Delete(BasketDtos[DataGrid.SelectedIndex].Id);
                UpdateDataGrid();
            }
        }
    }
}
