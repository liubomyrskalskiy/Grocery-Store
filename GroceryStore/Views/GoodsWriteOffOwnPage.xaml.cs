using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
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
    /// Interaction logic for GoodsWriteOffOwnPage.xaml
    /// </summary>
    public partial class GoodsWriteOffOwnPage : Page, IActivable
    {
        private readonly IGoodsWriteOffOwnService _goodsWriteOffOwnService;
        private readonly IWriteOffReasonService _writeOffReasonService;
        private readonly IGoodsInMarketOwnService _goodsInMarketOwnService;
        private readonly IGoodsOwnService _goodsOwnService;
        private readonly AppSettings _settings;
        private readonly IMapper _mapper;
        private EmployeeDTO _currentEmployee;

        public List<GoodsWriteOffOwnDTO> GoodsWriteOffOwnDtos { get; set; }
        public List<WriteOffReasonDTO> WriteOffReasonDtos { get; set; }
        public List<GoodsInMarketOwnDTO> GoodsInMarketOwnDtos { get; set; }

        public GoodsWriteOffOwnPage(IGoodsWriteOffOwnService goodsWriteOffOwnService,
            IWriteOffReasonService writeOffReasonService,
            IGoodsInMarketOwnService goodsInMarketOwnService,
            IOptions<AppSettings> settings, IMapper mapper, IGoodsOwnService goodsOwnService)
        {
            _goodsWriteOffOwnService = goodsWriteOffOwnService;
            _writeOffReasonService = writeOffReasonService;
            _goodsInMarketOwnService = goodsInMarketOwnService;
            _mapper = mapper;
            _goodsOwnService = goodsOwnService;
            _settings = settings.Value;

            InitializeComponent();

            GoodsInMarketComboBox.IsEnabled = false;
        }

        private void UpdateDataGrid()
        {
            GoodsWriteOffOwnDtos =
                _mapper.Map<List<GoodsWriteOffOwn>, List<GoodsWriteOffOwnDTO>>(_goodsWriteOffOwnService.GetAll());

            DataGrid.ItemsSource = GoodsWriteOffOwnDtos;
        }

        private bool ValidateForm()
        {
            if (!Regex.Match(ProductCodeTextBox.Text, @"^\d{5}$").Success)
            {
                MessageBox.Show("Invalid product code! It must contain 5 digits");
                ProductCodeTextBox.Focus();
                return false;
            }

            if (GoodsInMarketComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select manufacture date");
                return false;
            }

            if (!Regex.Match(AmountTextBox.Text, @"^[0-9]*(?:\,[0-9]*)?$").Success)
            {
                MessageBox.Show("Invalid amount! Check the data you've entered!");
                AmountTextBox.Focus();
                return false;
            }

            if (ReasonComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select reason");
                return false;
            }

            return true;
        }

        public Task ActivateAsync(object parameter)
        {
            _currentEmployee = (EmployeeDTO)parameter;
            WriteOffReasonDtos =
                _mapper.Map<List<WriteOffReason>, List<WriteOffReasonDTO>>(_writeOffReasonService.GetAll());
            ReasonComboBox.ItemsSource = WriteOffReasonDtos;
            UpdateDataGrid();
            return Task.CompletedTask;
        }

        private void CreateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;
            GoodsWriteOffOwn goodsWriteOffOwn = new GoodsWriteOffOwn();
            goodsWriteOffOwn.Id = GoodsWriteOffOwnDtos[^1]?.Id + 1 ?? 1;
            goodsWriteOffOwn.Amount = Convert.ToDouble(AmountTextBox.Text);
            goodsWriteOffOwn.Date = DateTime.Now;
            goodsWriteOffOwn.IdEmployee = _currentEmployee.Id;
            var tempGimo = (GoodsInMarketOwnDTO)GoodsInMarketComboBox.SelectedItem;
            goodsWriteOffOwn.IdGoodsInMarketOwn = tempGimo.Id;
            var gimo = _goodsInMarketOwnService.GetId(tempGimo.Id);
            goodsWriteOffOwn.IdProduction = gimo.IdProduction;
            var reason = (WriteOffReasonDTO)ReasonComboBox.SelectedItem;
            goodsWriteOffOwn.IdWriteOffReason = reason.Id;

            _goodsWriteOffOwnService.Create(goodsWriteOffOwn);
            UpdateDataGrid();
        }

        private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            _goodsWriteOffOwnService.Delete(GoodsWriteOffOwnDtos[DataGrid.SelectedIndex].Id);
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

                    if ((GoodsInMarketOwnDtos = _mapper.Map<List<GoodsInMarketOwn>, List<GoodsInMarketOwnDTO>>(
                            _goodsInMarketOwnService.GetAll()
                                .Where(item => item.IdProductionNavigation.IdGoodsOwn == tempGoodsOwnDto.Id && item.IdMarketNavigation.Address == _currentEmployee.MarketAddress)
                                .ToList())).Count > 0)
                    {
                        GoodsInMarketComboBox.ItemsSource = GoodsInMarketOwnDtos;
                        GoodsInMarketComboBox.IsEnabled = true;
                    }
                    else
                    {
                        GoodsInMarketOwnDtos = null;
                        GoodsInMarketComboBox.ItemsSource = null;
                        GoodsInMarketComboBox.IsEnabled = false;

                    }
                }
            }
            else
            {
                GoodTitleLabel.Content = "";
                WeightLabel.Content = "";
                PriceLabel.Content = "";

                GoodsInMarketOwnDtos = null;
                GoodsInMarketComboBox.ItemsSource = null;
                GoodsInMarketComboBox.IsEnabled = false;
            }
        }
        private void GoodsInMarketComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GoodsInMarketComboBox.SelectedItem != null)
            {
                GoodsInMarketOwnDTO tempProduction = (GoodsInMarketOwnDTO)GoodsInMarketComboBox.SelectedItem;
                AmountLabel.Content = "Amount: " + tempProduction.Amount;
            }
            else
                AmountLabel.Content = "";
        }
    }
}