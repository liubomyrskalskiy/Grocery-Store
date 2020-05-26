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
using GroceryStore.NavigationTransferObjects;
using Microsoft.Extensions.Options;

namespace GroceryStore.Windows
{
    /// <summary>
    ///     Interaction logic for ProductionWindow.xaml
    /// </summary>
    public partial class ProductionWindow : Window, IActivable
    {
        private readonly IGoodsInMarketService _goodsInMarketService;
        private readonly IMapper _mapper;
        private readonly IProductionContentsService _productionContentsService;
        private readonly IProductionService _productionService;
        private readonly AppSettings _settings;
        private EmployeeDTO _curreEmployee;
        private Production _currentProduction;

        public ProductionWindow(IProductionContentsService productionContentsService,
            IProductionService productionService, IGoodsInMarketService goodsInMarketService, IMapper mapper,
            IOptions<AppSettings> settings)
        {
            _productionContentsService = productionContentsService;
            _productionService = productionService;
            _goodsInMarketService = goodsInMarketService;
            _mapper = mapper;
            _settings = settings.Value;
            InitializeComponent();
        }

        private List<ProductionContentsDTO> ProductionContentsDtos { get; set; }
        private List<ProductionContentsDTO> CurrentProductionContentsDtos { get; set; }
        private List<ProductionContents> CurrentProductionContentses { get; set; }

        public Task ActivateAsync(object parameter)
        {
            var data = (ProductionNTO) parameter;
            _currentProduction = data.Production;
            _curreEmployee = data.Employee;

            ProductionLabel.Content = "Production number: " + _currentProduction.ProductionCode;
            CurrentProductionContentses = new List<ProductionContents>();

            UpdateDataGrid();

            return Task.CompletedTask;
        }

        private void UpdateDataGrid()
        {
            ProductionContentsDtos =
                _mapper.Map<List<ProductionContents>, List<ProductionContentsDTO>>(_productionContentsService.GetAll());

            ProductionContentsDtos.Sort(delegate (ProductionContentsDTO x, ProductionContentsDTO y)
            {
                return x.Id.CompareTo(y.Id);
            });

            ProductionLabel.Content = "Production number: " + _currentProduction.ProductionCode;
            DateLabel.Content = "Manufacture Date: " + _currentProduction.ManufactureDate;
            TotalLabel.Content = $"Total: {_currentProduction.TotalCost,0:C2}";

            CurrentProductionContentsDtos = ProductionContentsDtos
                .Where(item => item.ProductionCode == _currentProduction.ProductionCode).ToList();
            DataGrid.ItemsSource = CurrentProductionContentsDtos;
        }

        private bool ValidateForm()
        {
            if (!Regex.Match(ProductCodeTextBox.Text, @"^\d{5}$").Success)
            {
                MessageBox.Show("Invalid product code! It must contain 5 digits");
                ProductCodeTextBox.Focus();
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

        private void CreateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;
            var productionContents = new ProductionContents();
            GoodsInMarket tempGoodsInMarket;
            productionContents.Id = ProductionContentsDtos[^1]?.Id + 1 ?? 1;
            productionContents.Amount = Convert.ToDouble(AmountTextBox.Text);
            productionContents.IdProduction = _currentProduction.Id;
            if ((tempGoodsInMarket = _goodsInMarketService.GetAll()
                    .FirstOrDefault(gim =>
                        gim.IdGoodsNavigation.ProductCode == ProductCodeTextBox.Text &&
                        gim.Amount >= Convert.ToDouble(AmountTextBox.Text) &&
                        gim.IdMarketNavigation.Address == _curreEmployee.MarketAddress)) == null)
            {
                MessageBox.Show("There is no such product or there is not enough product in your store!");
                return;
            }

            productionContents.IdGoodsInMarket = tempGoodsInMarket.Id;

            CurrentProductionContentses.Add(productionContents);

            var tempProductionContentsDto =
                _mapper.Map<ProductionContents, ProductionContentsDTO>(
                    _productionContentsService.Create(productionContents));
            _currentProduction.TotalCost += tempProductionContentsDto.Price;
            UpdateDataGrid();
        }

        private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex != -1)
            {
                _currentProduction.TotalCost -= CurrentProductionContentsDtos[DataGrid.SelectedIndex].Price;
                _productionContentsService.Delete(CurrentProductionContentsDtos[DataGrid.SelectedIndex].Id);
                UpdateDataGrid();
            }
        }

        private void DoneGoodBtn_OnClick(object sender, RoutedEventArgs e)
        {
            _productionService.Update(_currentProduction);

            foreach (var currentProductionContentse in CurrentProductionContentses)
            {
                var gim = _goodsInMarketService.GetId(currentProductionContentse.IdGoodsInMarket ?? 0);
                gim.Amount -= currentProductionContentse.Amount;
                _goodsInMarketService.Update(gim);
            }

            Close();
        }

        private void ProductCodeTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!Regex.Match(ProductCodeTextBox.Text, @"^\d{5}$").Success)
            {
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
                }
                else
                {
                    goodsInMarketDto = _mapper.Map<GoodsInMarket, GoodsInMarketDTO>(tempgim);
                    GoodTitleLabel.Content = "Good: " + goodsInMarketDto.Good;
                    ProducerTitleLabel.Content = "Producer: " + goodsInMarketDto.Producer;
                    WeightLabel.Content = "Unit weight: " + goodsInMarketDto.Weight;
                    PriceLabel.Content = "Price: " + goodsInMarketDto.Price;
                    AmountLabel.Content = "Remains in store: " + goodsInMarketDto.Amount;
                }
            }
        }

        private void BtnClose(object sender, RoutedEventArgs e)
        {
            foreach (var currentProductionContentsDto in CurrentProductionContentsDtos)
                _productionContentsService.Delete(currentProductionContentsDto.Id);

            _productionService.Delete(_currentProduction.Id);
            Close();
        }
    }
}