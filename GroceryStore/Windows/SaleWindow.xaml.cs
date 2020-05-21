using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AutoMapper;
using GroceryStore.Core.Abstractions;
using GroceryStore.Core.Abstractions.IServices;
using GroceryStore.Core.DTO;
using GroceryStore.Core.Models;
using GroceryStore.NavigationTransferObjects;
using Microsoft.Extensions.Options;

namespace GroceryStore.Windows
{
    /// <summary>
    /// Interaction logic for SaleWindow.xaml
    /// </summary>
    public partial class SaleWindow : Window, IActivable
    {
        private readonly ISaleService _saleService;
        private readonly IBasketService _basketService;
        private readonly IBasketOwnService _basketOwnService;
        private readonly IGoodsInMarketService _goodsInMarketService;
        private readonly IGoodsInMarketOwnService _goodsInMarketOwnService;
        private readonly AppSettings _settings;
        private readonly IMapper _mapper;
        private Sale _currentSale;
        private EmployeeDTO _curreEmployee;

        public List<Basket> CurrentBaskets { get; set; }
        public List<BasketOwn> CurrentBasketOwns { get; set; }
        public List<UniversalBasketDTO> UniversalBasketDtos { get; set; }

        public List<UniversalBasketDTO> BasketDtos { get; set; }

        public List<UniversalBasketDTO> BasketOwnDtos { get; set; }

        public SaleWindow(ISaleService saleService, IOptions<AppSettings> settings, IMapper mapper,
            IBasketOwnService basketOwnService, IBasketService basketService,
            IGoodsInMarketService goodsInMarketService, IGoodsInMarketOwnService goodsInMarketOwnService)
        {
            _saleService = saleService;
            _settings = settings.Value;
            _mapper = mapper;
            _basketOwnService = basketOwnService;
            _basketService = basketService;
            _goodsInMarketService = goodsInMarketService;
            _goodsInMarketOwnService = goodsInMarketOwnService;

            InitializeComponent();
        }

        public Task ActivateAsync(object parameter)
        {
            var data = (SaleNTO) parameter;
            _currentSale = data.Sale;
            _curreEmployee = data.EmployeeDto;
            CheckLabel.Content = "Сheck number: " + _currentSale.CheckNumber;

            CurrentBaskets = new List<Basket>();
            CurrentBasketOwns = new List<BasketOwn>();

            UpdateDataGrid();

            return Task.CompletedTask;
        }

        private void UpdateDataGrid()
        {
            BasketDtos = _mapper.Map<List<Basket>, List<UniversalBasketDTO>>(_basketService.GetAll());
            BasketOwnDtos = _mapper.Map<List<BasketOwn>, List<UniversalBasketDTO>>(_basketOwnService.GetAll());
            UniversalBasketDtos = BasketDtos.Where(item => item.CheckNumber == _currentSale.CheckNumber).ToList();
            UniversalBasketDtos.AddRange(BasketOwnDtos.Where(item => item.CheckNumber == _currentSale.CheckNumber)
                .ToList());
            _saleService.Refresh(_currentSale);
            _currentSale = _saleService.GetId(_currentSale.Id);

            DateLabel.Content = _currentSale.Date.ToString();
            TotalLabel.Content = $"{_currentSale.Total,0:C2}";

            DataGrid.ItemsSource = UniversalBasketDtos;
        }

        private void BtnClose(object sender, RoutedEventArgs e)
        {
            foreach (var basketDto in UniversalBasketDtos)
            {
                if (!basketDto.IsOwn)
                {
                    _basketService.Delete(basketDto.Id);
                }
                else
                {
                    _basketOwnService.Delete(basketDto.Id);
                }
            }

            _saleService.Delete(_currentSale.Id);
            Close();
        }

        private bool ValidateForm()
        {
            if (!Regex.Match(AmountTextBox.Text, @"^[0-9]*(?:\,[0-9]*)?$").Success || AmountTextBox.Text == "")
            {
                MessageBox.Show("Invalid amount! Check the data you've entered!");
                AmountTextBox.Focus();
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

        private void CreateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;
            if (OwnCheckBox.IsChecked == true)
            {
                if (UniversalBasketDtos.FirstOrDefault(item =>
                        item.ProductCode == ProductCodeTextBox.Text && item.IsOwn == true) != null)
                {
                    MessageBox.Show("There is such goods in basket already!");
                    return;
                }

                BasketOwn basketOwn = new BasketOwn();
                GoodsInMarketOwn tempGimo;
                basketOwn.Id = BasketOwnDtos[^1]?.Id + 1 ?? 1;
                basketOwn.Amount = Convert.ToDouble(AmountTextBox.Text);
                basketOwn.IdSale = _currentSale.Id;

                if ((tempGimo = _goodsInMarketOwnService.GetAll()
                        .FirstOrDefault(gim =>
                            gim.IdProductionNavigation?.IdGoodsOwnNavigation.ProductCode == ProductCodeTextBox.Text &&
                            gim.Amount >= Convert.ToDouble(AmountTextBox.Text) &&
                            gim.IdMarketNavigation.Address == _curreEmployee.MarketAddress)) ==
                    null)
                {
                    MessageBox.Show("There is no such product or there is not enough goods in your store!");
                    return;
                }
                else
                    basketOwn.IdGoodsInMarketOwn = tempGimo.Id;

                CurrentBasketOwns.Add(basketOwn);

                _basketOwnService.Create(basketOwn);
                UpdateDataGrid();
            }
            else
            {
                if (UniversalBasketDtos.FirstOrDefault(item =>
                        item.ProductCode == ProductCodeTextBox.Text && item.IsOwn == false) != null)
                {
                    MessageBox.Show("There is such goods in basket already!");
                    return;
                }

                Basket basket = new Basket();
                GoodsInMarket tempgim;
                basket.Id = BasketDtos[^1]?.Id + 1 ?? 1;
                basket.Amount = Convert.ToDouble(AmountTextBox.Text);
                basket.IdSale = _currentSale.Id;
                if ((tempgim = _goodsInMarketService.GetAll()
                        .FirstOrDefault(gim =>
                            gim.IdGoodsNavigation.ProductCode == ProductCodeTextBox.Text &&
                            gim.Amount >= Convert.ToDouble(AmountTextBox.Text) &&
                            gim.IdMarketNavigation.Address == _curreEmployee.MarketAddress)) == null)
                {
                    MessageBox.Show("There is no such product or there is not enough product in your store!");
                    return;
                }
                else
                    basket.IdGoodsInMarket = tempgim.Id;

                CurrentBaskets.Add(basket);

                _basketService.Create(basket);
                UpdateDataGrid();
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
                if (OwnCheckBox.IsChecked == true)
                {
                    GoodsInMarketOwnDTO goodsInMarketOwnDto;
                    GoodsInMarketOwn tempGimo;
                    if ((tempGimo = _goodsInMarketOwnService.GetAll()
                            .FirstOrDefault(gim =>
                                gim.IdProductionNavigation?.IdGoodsOwnNavigation.ProductCode ==
                                ProductCodeTextBox.Text)) ==
                        null)
                    {
                        GoodTitleLabel.Content = "";
                        ProducerTitleLabel.Content = "";
                        WeightLabel.Content = "";
                        PriceLabel.Content = "";
                        AmountLabel.Content = "";
                        return;
                    }
                    else
                    {
                        goodsInMarketOwnDto = _mapper.Map<GoodsInMarketOwn, GoodsInMarketOwnDTO>(tempGimo);
                        GoodTitleLabel.Content = "Good: " + goodsInMarketOwnDto.Good;
                        ProducerTitleLabel.Content =
                            "Manufacture date: " + goodsInMarketOwnDto.ManufactureDate.ToString();
                        WeightLabel.Content = "Unit weight: " + goodsInMarketOwnDto.Weight;
                        PriceLabel.Content = "Price: " + goodsInMarketOwnDto.Price;
                        AmountLabel.Content = "Remains in store: " + goodsInMarketOwnDto.Amount;
                    }
                }
                else
                {
                    GoodsInMarketDTO goodsInMarketDto;
                    GoodsInMarket tempgim;
                    if ((tempgim = _goodsInMarketService.GetAll()
                            .FirstOrDefault(gim =>
                                gim.IdGoodsNavigation.ProductCode == ProductCodeTextBox.Text &&
                                gim.IdMarketNavigation.Address == _curreEmployee.MarketAddress)) == null)
                    {
                        GoodTitleLabel.Content = "";
                        ProducerTitleLabel.Content = "";
                        WeightLabel.Content = "";
                        PriceLabel.Content = "";
                        AmountLabel.Content = "";
                        return;
                    }
                    else
                    {
                        goodsInMarketDto = _mapper.Map<GoodsInMarket, GoodsInMarketDTO>(tempgim);
                        GoodTitleLabel.Content = "Good: " + goodsInMarketDto.Good;
                        ProducerTitleLabel.Content = "Producer: " + goodsInMarketDto.Producer;
                        WeightLabel.Content = "Unit weight: " + goodsInMarketDto.Weight;
                        PriceLabel.Content = "Price: " + goodsInMarketDto.Price.ToString();
                        AmountLabel.Content = "Remains in store: " + goodsInMarketDto.Amount.ToString();
                    }
                }
            }
        }

        private void formCheck()
        {
            List<string> lines = new List<string>() { $"Check#          {_currentSale.CheckNumber}", $"Employee:          {_curreEmployee.FullName}", $"Date:     {_currentSale.Date}" };
            lines.Add("----------------------------------------");
            foreach (var universalBasketDto in UniversalBasketDtos)
            {
                lines.Add("");
                lines.Add($"{universalBasketDto.Title}     {universalBasketDto.Amount,0:0.00}     {universalBasketDto.Price}");
                lines.Add("----------------------------------------");
            }
            lines.Add("----------------------------------------");
            lines.Add($"Total:          {_currentSale.Total,0:C2}");

            if (File.Exists($"Check#{_currentSale.CheckNumber}.txt"))
            {
                File.Delete($"Check#{_currentSale.CheckNumber}.txt");
            }

            using (StreamWriter file = File.CreateText($"Check#{_currentSale.CheckNumber}.txt"))
            {
                foreach (string line in lines)
                {
                    file.WriteLine(line.PadLeft(40));
                }
            }
            Process.Start("notepad.exe", $"Check#{_currentSale.CheckNumber}.txt");
        }

        private void DoneGoodBtn_OnClick(object sender, RoutedEventArgs e)
        {
            _saleService.Refresh(_currentSale);
            _currentSale = _saleService.GetId(_currentSale.Id);
            foreach (var currentBasket in CurrentBaskets)
            {
                var gim = _goodsInMarketService.GetId(currentBasket.IdGoodsInMarket ?? 0);
                gim.Amount -= currentBasket.Amount;
                _goodsInMarketService.Update(gim);
            }

            foreach (var currentBasketOwn in CurrentBasketOwns)
            {
                var gimo = _goodsInMarketOwnService.GetId(currentBasketOwn.IdGoodsInMarketOwn ?? 0);
                gimo.Amount -= currentBasketOwn.Amount;
                _goodsInMarketOwnService.Update(gimo);
            }

            formCheck();

            Close();
        }

        private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex != -1)
            {
                if (!UniversalBasketDtos[DataGrid.SelectedIndex].IsOwn)
                {
                    Basket tempBasket;
                    tempBasket = CurrentBaskets.FirstOrDefault(item =>
                        item.Id == UniversalBasketDtos[DataGrid.SelectedIndex].Id);
                    CurrentBaskets.Remove(tempBasket);
                    _basketService.Delete(UniversalBasketDtos[DataGrid.SelectedIndex].Id);
                }
                else
                {
                    BasketOwn tempBasket;
                    tempBasket = CurrentBasketOwns.FirstOrDefault(item =>
                        item.Id == UniversalBasketDtos[DataGrid.SelectedIndex].Id);
                    CurrentBasketOwns.Remove(tempBasket);
                    _basketOwnService.Delete(UniversalBasketDtos[DataGrid.SelectedIndex].Id);
                }

                UpdateDataGrid();
            }
        }
    }
}