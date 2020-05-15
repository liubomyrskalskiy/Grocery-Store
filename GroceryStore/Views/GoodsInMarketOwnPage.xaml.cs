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
        private readonly IGoodsOwnService _goodsOwnService;
        private readonly AppSettings _settings;
        private readonly IMapper _mapper;
        private EmployeeDTO _currentEmployee;

        public List<GoodsInMarketOwnDTO> GoodsInMarketOwnDtos { get; set; }

        public List<ProductionDTO> ProductionDtos { get; set; }

        public GoodsInMarketOwnPage(IGoodsInMarketOwnService goodsInMarketOwnService, IMarketService marketService,
            IProductionService productionService, IOptions<AppSettings> settings, IMapper mapper,
            IGoodsOwnService goodsOwnService)
        {
            _goodsInMarketOwnService = goodsInMarketOwnService;
            _marketService = marketService;
            _productionService = productionService;
            _settings = settings.Value;
            _mapper = mapper;
            _goodsOwnService = goodsOwnService;

            InitializeComponent();

            ProductionComboBox.IsEnabled = false;
        }

        private void UpdateDataGrid()
        {
            GoodsInMarketOwnDtos =
                _mapper.Map<List<GoodsInMarketOwn>, List<GoodsInMarketOwnDTO>>(_goodsInMarketOwnService.GetAll());

            DataGrid.ItemsSource = GoodsInMarketOwnDtos.Where(item => item.Address == _currentEmployee.MarketAddress).ToList();
        }

        private bool ValidateForm()
        {
            if (!Regex.Match(AmountTextBox.Text, @"^[0-9]+$").Success)
            {
                MessageBox.Show("Invalid amount! Check the data you've entered!");
                AmountTextBox.Focus();
                return false;
            }

            return true;
        }

        public Task ActivateAsync(object parameter)
        {
            _currentEmployee = (EmployeeDTO)parameter;
            //_marketAddress = parameter.ToString();
            UpdateDataGrid();
            return Task.CompletedTask;
        }

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedIndex != -1)
            {
                AmountTextBox.Text = GoodsInMarketOwnDtos[DataGrid.SelectedIndex].Amount.ToString();
            }
        }

        private void CreateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;
            GoodsInMarketOwn goodsInMarketOwn = new GoodsInMarketOwn();
            ProductionDTO tempProduction;
            Market tempMarket;

            goodsInMarketOwn.Id = GoodsInMarketOwnDtos[^1]?.Id + 1 ?? 1;
            goodsInMarketOwn.Amount = Convert.ToDouble(AmountTextBox.Text);
            if (ProductionComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select production date");
                return;
            }
            else
            {
                tempProduction = (ProductionDTO) ProductionComboBox.SelectedItem;
                if (tempProduction.Amount >= goodsInMarketOwn.Amount)
                    goodsInMarketOwn.IdProduction = tempProduction.Id;
                else
                {
                    MessageBox.Show("There is not enough goods with such manufacture date");
                    return;
                }
            }

            if ((tempMarket =
                    _marketService.GetAll().FirstOrDefault(market => market.Address == _currentEmployee.MarketAddress)) == null)
            {
                MessageBox.Show("There is no market on such address in database!");
                return;
            }
            else
                goodsInMarketOwn.IdMarket = tempMarket.Id;

            _goodsInMarketOwnService.Create(goodsInMarketOwn);
            Production currentProduction = _productionService.GetId(tempProduction.Id);
            currentProduction.Amount -= Convert.ToInt32(goodsInMarketOwn.Amount);
            _productionService.Update(currentProduction);
            UpdateDataGrid();
        }

        private void UpdateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            //if (DataGrid.SelectedIndex == -1) return;
            //if (!ValidateForm()) return;
            //GoodsInMarketOwn goodsInMarketOwn = new GoodsInMarketOwn();
            //Production temProduction;
            //Market tempMarket;

            //goodsInMarketOwn.Id = GoodsInMarketOwnDtos[DataGrid.SelectedIndex].Id;
            //goodsInMarketOwn.Amount = Convert.ToDouble(AmountTextBox.Text);
            //if ((temProduction = _productionService.GetAll()
            //        .FirstOrDefault(goods => goods.ProductionCode == ProductionCodeTextBox.Text)) == null)
            //{
            //    MessageBox.Show("There is no such product in database!");
            //    return;
            //}
            //else
            //    goodsInMarketOwn.IdProduction = temProduction.Id;

            //if ((tempMarket =
            //        _marketService.GetAll().FirstOrDefault(market => market.Address == _marketAddress)) == null)
            //{
            //    MessageBox.Show("There is no market on such address in database!");
            //    return;
            //}
            //else
            //    goodsInMarketOwn.IdMarket = tempMarket.Id;

            //_goodsInMarketOwnService.Update(goodsInMarketOwn);
            //UpdateDataGrid();
        }

        private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            _goodsInMarketOwnService.Delete(GoodsInMarketOwnDtos[DataGrid.SelectedIndex].Id);
            UpdateDataGrid();
        }

        private void ProductCodeTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (Regex.Match(ProductCodeTextBox.Text, @"^\d{5}$").Success)
            {
                GoodsOwnDTO tempGoodsOwnDto;

                if ((tempGoodsOwnDto = _mapper.Map<GoodsOwn, GoodsOwnDTO>(_goodsOwnService.GetAll()
                        .FirstOrDefault(item => item.ProductCode == ProductCodeTextBox.Text))) != null)
                {
                    GoodTitleLabel.Content = "Good: " + tempGoodsOwnDto.Title;
                    CategoryLabel.Content = "Category: " + tempGoodsOwnDto.Category;
                    WeightLabel.Content = "Unit weight: " + tempGoodsOwnDto.Weight;
                    PriceLabel.Content = "Price: " + $"{tempGoodsOwnDto.Price,0:C2}";

                    if ((ProductionDtos = _mapper.Map<List<Production>, List<ProductionDTO>>(_productionService.GetAll()
                            .Where(item => item.IdGoodsOwnNavigation.ProductCode == ProductCodeTextBox.Text).ToList()))
                        .Count > 0)
                    {
                        ProductionComboBox.ItemsSource = ProductionDtos;
                        ProductionComboBox.IsEnabled = true;
                    }
                    else
                    {
                        ProductionDtos = null;
                        ProductionComboBox.ItemsSource = null;
                        ProductionComboBox.IsEnabled = false;
                    }
                }
            }
            else
            {
                GoodTitleLabel.Content = "";
                CategoryLabel.Content = "";
                WeightLabel.Content = "";
                PriceLabel.Content = "";

                ProductionDtos = null;
                ProductionComboBox.ItemsSource = null;
                ProductionComboBox.IsEnabled = false;
            }
        }

        private void ProductionComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProductionComboBox.SelectedItem != null)
            {
                ProductionDTO tempProduction = (ProductionDTO) ProductionComboBox.SelectedItem;
                AmountLabel.Content = "Amount: " + tempProduction.Amount;
            }
            else
                AmountLabel.Content = "";
        }
    }
}