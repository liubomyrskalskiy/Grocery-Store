using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using AutoMapper;
using GroceryStore.Core.Abstractions;
using GroceryStore.Core.Abstractions.IServices;
using GroceryStore.Core.DTO;
using GroceryStore.Core.Models;
using Microsoft.Extensions.Options;

namespace GroceryStore.Views
{
    /// <summary>
    /// Interaction logic for GoodsInMarketOwn_AdminPage.xaml
    /// </summary>
    public partial class GoodsInMarketOwn_AdminPage : Page, IActivable
    {
        private readonly IGoodsInMarketOwnService _goodsInMarketOwnService;
        private readonly IMarketService _marketService;
        private readonly IProductionService _productionService;
        private readonly IGoodsOwnService _goodsOwnService;
        private readonly IBasketOwnService _basketOwnService;
        private readonly IGoodsWriteOffOwnService _goodsWriteOffOwnService;
        private readonly AppSettings _settings;
        private readonly IMapper _mapper;
        private EmployeeDTO _currentEmployee;

        public List<GoodsInMarketOwnDTO> GoodsInMarketOwnDtos { get; set; }
        public List<GoodsInMarketOwnDTO> FilteredGoodsInMarketOwnDtos { get; set; }
        public List<MarketDTO> MarketDtos { get; set; }
        public List<ProductionDTO> ProductionDtos { get; set; }

        public GoodsInMarketOwn_AdminPage(IGoodsInMarketOwnService goodsInMarketOwnService, IMarketService marketService,
            IProductionService productionService, IOptions<AppSettings> settings, IMapper mapper,
            IGoodsOwnService goodsOwnService, IBasketOwnService basketOwnService, IGoodsWriteOffOwnService goodsWriteOffOwnService)
        {
            _goodsInMarketOwnService = goodsInMarketOwnService;
            _marketService = marketService;
            _productionService = productionService;
            _settings = settings.Value;
            _mapper = mapper;
            _goodsOwnService = goodsOwnService;
            _basketOwnService = basketOwnService;
            _goodsWriteOffOwnService = goodsWriteOffOwnService;

            InitializeComponent();

            ProductionComboBox.IsEnabled = false;
        }

        private void UpdateDataGrid()
        {
            GoodsInMarketOwnDtos =
                _mapper.Map<List<GoodsInMarketOwn>, List<GoodsInMarketOwnDTO>>(_goodsInMarketOwnService.GetAll());

            MarketComboBox.ItemsSource = MarketDtos;
            FilteredGoodsInMarketOwnDtos = GoodsInMarketOwnDtos;

            if (MarketFilterComboBox.SelectedItem != null)
            {
                var tempMarket = (MarketDTO)MarketFilterComboBox.SelectedItem;
                FilteredGoodsInMarketOwnDtos = GoodsInMarketOwnDtos
                    .Where(item => item.FullAddress == tempMarket.FullAddress).ToList();
            }

            DataGrid.ItemsSource = FilteredGoodsInMarketOwnDtos;
        }

        private bool ValidateForm()
        {
            if (!Regex.Match(AmountTextBox.Text, @"^[0-9]+$").Success)
            {
                MessageBox.Show("Invalid amount! Check the data you've entered!");
                AmountTextBox.Focus();
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
            MarketFilterComboBox.ItemsSource = MarketDtos;

            UpdateDataGrid();
            return Task.CompletedTask;
        }

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedIndex != -1)
            {
                AmountTextBox.Text = GoodsInMarketOwnDtos[DataGrid.SelectedIndex].Amount.ToString();
                ProductionComboBox.SelectedItem = ProductionDtos.FirstOrDefault(item =>
                    item.ProductionCode == GoodsInMarketOwnDtos[DataGrid.SelectedIndex].ProductionCode);
            }
        }

        private void CreateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;
            GoodsInMarketOwn goodsInMarketOwn = new GoodsInMarketOwn();
            ProductionDTO tempProduction;
            MarketDTO tempMarket;

            goodsInMarketOwn.Id = GoodsInMarketOwnDtos[^1]?.Id + 1 ?? 1;
            goodsInMarketOwn.Amount = Convert.ToDouble(AmountTextBox.Text);
            if (ProductionComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select production date");
                return;
            }
            else
            {
                tempProduction = (ProductionDTO)ProductionComboBox.SelectedItem;
                if (tempProduction.Amount >= goodsInMarketOwn.Amount)
                    goodsInMarketOwn.IdProduction = tempProduction.Id;
                else
                {
                    MessageBox.Show("There is not enough goods with such manufacture date");
                    return;
                }
            }

            tempMarket = (MarketDTO) MarketComboBox.SelectedItem;
            goodsInMarketOwn.IdMarket = tempMarket.Id;

            _goodsInMarketOwnService.Create(goodsInMarketOwn);
            Production currentProduction = _productionService.GetId(tempProduction.Id);
            currentProduction.Amount -= Convert.ToInt32(goodsInMarketOwn.Amount);
            _productionService.Update(currentProduction);
            UpdateDataGrid();
        }

        private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            if (_basketOwnService.GetAll().FirstOrDefault(item =>
                    item.IdGoodsInMarketOwn == GoodsInMarketOwnDtos[DataGrid.SelectedIndex].Id) != null)
            {
                MessageBox.Show("You can not delete this row because it is referenced by some Sales!");
                return;
            }

            if (_goodsWriteOffOwnService.GetAll().FirstOrDefault(item =>
                    item.IdGoodsInMarketOwn == GoodsInMarketOwnDtos[DataGrid.SelectedIndex].Id) != null)
            {
                MessageBox.Show("You can not delete this row because it is referenced by some write-off!");
                return;
            }
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
                ProductionDTO tempProduction = (ProductionDTO)ProductionComboBox.SelectedItem;
                AmountLabel.Content = "Amount: " + tempProduction.Amount;
            }
            else
                AmountLabel.Content = "";
        }

        private void MarketFilterComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MarketFilterComboBox.SelectedItem != null)
            {
                UpdateDataGrid();
            }
        }

        private void ClearFilterBtn_OnClick(object sender, RoutedEventArgs e)
        {
            MarketFilterComboBox.SelectedItem = null;
            UpdateDataGrid();
        }
    }
}
