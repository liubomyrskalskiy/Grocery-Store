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
    /// Interaction logic for ProductionContentsPage.xaml
    /// </summary>
    public partial class ProductionContentsPage : Page, IActivable
    {
        private readonly IProductionContentsService _productionContentsService;
        private readonly IProductionService _productionService;
        private readonly IGoodsInMarketService _goodsInMarketService;
        private AppSettings _settings;
        private readonly IMapper _mapper;

        public List<ProductionContentsDTO> ProductionContentsDtos { get; set; }

        public ProductionContentsPage(IProductionContentsService productionContentsService, IProductionService productionService, IGoodsInMarketService goodsInMarketService, IOptions<AppSettings> settings, IMapper mapper)
        {
            _productionContentsService = productionContentsService;
            _productionService = productionService;
            _goodsInMarketService = goodsInMarketService;
            _settings = settings.Value;
            _mapper = mapper;

            InitializeComponent();

            UpdateDataGrid();
        }

        private void UpdateDataGrid()
        {
            ProductionContentsDtos = _mapper.Map<List<ProductionContents>, List<ProductionContentsDTO>>(_productionContentsService.GetAll());

            DataGrid.ItemsSource = ProductionContentsDtos;
        }

        private bool ValidateForm()
        {
            if (!Regex.Match(ProductCodeTextBox.Text, @"^\d{5}$").Success)
            {
                MessageBox.Show("Invalid product code! It must contain 5 digits");
                ProductCodeTextBox.Focus();
                return false;
            }

            if (!Regex.Match(ProductionCodeTextBox.Text, @"^\d{5,10}$").Success)
            {
                MessageBox.Show("Invalid production code! It must contain at least 5 digits and not exceed 10 digits");
                ProductionCodeTextBox.Focus();
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
                ProductCodeTextBox.Text = ProductionContentsDtos[DataGrid.SelectedIndex].ProductCode;
                ProductionCodeTextBox.Text = ProductionContentsDtos[DataGrid.SelectedIndex].ProductionCode;
                AmountTextBox.Text = ProductionContentsDtos[DataGrid.SelectedIndex].Amount.ToString();
            }
        }

        private void CreateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if(!ValidateForm()) return;
            ProductionContents productionContents = new ProductionContents();
            Production tempProduction;
            GoodsInMarket tempGoodsInMarket;
            productionContents.Id = ProductionContentsDtos[^1]?.Id + 1 ?? 1;
            productionContents.Amount = Convert.ToDouble(AmountTextBox.Text);
            if ((tempProduction = _productionService.GetAll()
                    .FirstOrDefault(production => production.ProductionCode == ProductionCodeTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such production in database!");
                return;
            }
            else
                productionContents.IdProduction = tempProduction.Id;

            if ((tempGoodsInMarket = _goodsInMarketService.GetAll().FirstOrDefault(goodsInMarket =>
                    goodsInMarket.IdGoodsNavigation.ProductCode == ProductCodeTextBox.Text && goodsInMarket.Amount >= Convert.ToDouble(AmountTextBox.Text))) == null)
            {
                MessageBox.Show("There is no such product in database or there is not enough goods in the store!");
                return;
            }
            else
                productionContents.IdGoodsInMarket = tempGoodsInMarket.Id;

            _productionContentsService.Create(productionContents);
            UpdateDataGrid();
        }

        private void UpdateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            if (!ValidateForm()) return;
            ProductionContents productionContents = new ProductionContents();
            Production tempProduction;
            GoodsInMarket tempGoodsInMarket;
            productionContents.Id = ProductionContentsDtos[DataGrid.SelectedIndex].Id;
            productionContents.Amount = Convert.ToDouble(AmountTextBox.Text);
            if ((tempProduction = _productionService.GetAll()
                    .FirstOrDefault(production => production.ProductionCode == ProductionCodeTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such production in database!");
                return;
            }
            else
                productionContents.IdProduction = tempProduction.Id;

            if ((tempGoodsInMarket = _goodsInMarketService.GetAll().FirstOrDefault(goodsInMarket =>
                    goodsInMarket.IdGoodsNavigation.ProductCode == ProductCodeTextBox.Text && goodsInMarket.Amount >= Convert.ToDouble(AmountTextBox.Text))) == null)
            {
                MessageBox.Show("There is no such product in database or there is not enough goods in the store!");
                return;
            }
            else
                productionContents.IdGoodsInMarket = tempGoodsInMarket.Id;

            _productionContentsService.Update(productionContents);
            UpdateDataGrid();
        }

        private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            _productionContentsService.Delete(ProductionContentsDtos[DataGrid.SelectedIndex].Id);
            UpdateDataGrid();
        }

        
    }
}
