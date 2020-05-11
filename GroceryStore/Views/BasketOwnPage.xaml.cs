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
    /// Interaction logic for BasketOwn.xaml
    /// </summary>
    public partial class BasketOwnPage : Page, IActivable
    {
        private readonly IBasketOwnService _basketOwnService;
        private readonly ISaleService _saleService;
        private readonly IGoodsInMarketOwnService _goodsInMarketOwnService;
        private AppSettings _settings;
        private readonly IMapper _mapper;

        public List<BasketOwnDTO> BasketOwnDtos { get; set; }

        public BasketOwnPage(IBasketOwnService basketOwnService, ISaleService saleService, IGoodsInMarketOwnService goodsInMarketOwnService, IOptions<AppSettings> settings, IMapper mapper)
        {
            _basketOwnService = basketOwnService;
            _saleService = saleService;
            _goodsInMarketOwnService = goodsInMarketOwnService;
            _mapper = mapper;
            _settings = settings.Value;

            InitializeComponent();

            UpdateDataGrid();
        }

        private void UpdateDataGrid()
        {
            BasketOwnDtos = _mapper.Map<List<BasketOwn>, List<BasketOwnDTO>>(_basketOwnService.GetAll());

            DataGrid.ItemsSource = BasketOwnDtos;
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

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedIndex != -1)
            {
                AmountTextBox.Text = BasketOwnDtos[DataGrid.SelectedIndex].Amount.ToString();
                CheckNumberTextBox.Text = BasketOwnDtos[DataGrid.SelectedIndex].CheckNumber;
                ProductCodeTextBox.Text = BasketOwnDtos[DataGrid.SelectedIndex].ProductCode;
            }
        }

        private void CreateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;

            BasketOwn basketOwn = new BasketOwn();
            Sale tempSale;
            GoodsInMarketOwn tempGimo;
            basketOwn.Id = BasketOwnDtos[^1]?.Id + 1 ?? 1;
            basketOwn.Amount = Convert.ToDouble(AmountTextBox.Text);
            if ((tempSale = _saleService.GetAll()
                    .FirstOrDefault(sale => sale.CheckNumber == CheckNumberTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such check number!");
                return;
            }
            else
                basketOwn.IdSale = tempSale.Id;

            if ((tempGimo = _goodsInMarketOwnService.GetAll()
                    .FirstOrDefault(gim =>
                        gim.IdProductionNavigation?.IdGoodsOwnNavigation.ProductCode == ProductCodeTextBox.Text && gim.Amount >= Convert.ToDouble(AmountTextBox.Text))) ==
                null)
            {
                MessageBox.Show("There is no such product code or there is not enough goods in the store!");
                return;
            }
            else
                basketOwn.IdGoodsInMarketOwn = tempGimo.Id;

            _basketOwnService.Create(basketOwn);
            UpdateDataGrid();
        }

        private void UpdateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            if (!ValidateForm()) return;
            BasketOwn basketOwn = new BasketOwn();
            Sale tempSale;
            GoodsInMarketOwn tempGimo;
            basketOwn.Id = BasketOwnDtos[DataGrid.SelectedIndex].Id;
            basketOwn.Amount = Convert.ToDouble(AmountTextBox.Text);
            if ((tempSale = _saleService.GetAll()
                    .FirstOrDefault(sale => sale.CheckNumber == CheckNumberTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such check number!");
                return;
            }
            else
                basketOwn.IdSale = tempSale.Id;
            if ((tempGimo = _goodsInMarketOwnService.GetAll()
                    .FirstOrDefault(gim =>
                        gim.IdProductionNavigation?.IdGoodsOwnNavigation.ProductCode == ProductCodeTextBox.Text && gim.Amount >= Convert.ToDouble(AmountTextBox.Text))) ==
                null)
            {
                MessageBox.Show("There is no such product code or there is not enough goods in the store!");
                return;
            }
            else
                basketOwn.IdGoodsInMarketOwn = tempGimo.Id;

            _basketOwnService.Update(basketOwn);
            UpdateDataGrid();
        }

        private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            _basketOwnService.Delete(BasketOwnDtos[DataGrid.SelectedIndex].Id);
            UpdateDataGrid();
        }
    }
}
