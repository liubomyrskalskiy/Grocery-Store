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
    /// Interaction logic for GoodsWriteOffPage.xaml
    /// </summary>
    public partial class GoodsWriteOffPage : Page, IActivable
    {
        private readonly IGoodsWriteOffService _goodsWriteOffService;
        private readonly IWriteOffReasonService _writeOffReasonService;
        private readonly IDeliveryShipmentService _deliveryShipmentService;
        private readonly IGoodsInMarketService _goodsInMarketService;
        private readonly IGoodsService _goodsService;
        private readonly AppSettings _settings;
        private readonly IMapper _mapper;
        private EmployeeDTO _currentEmployee;

        public List<GoodsWriteOffDTO> GoodsWriteOffDtos { get; set; }
        public List<WriteOffReasonDTO> WriteOffReasonDtos { get; set; }
        public List<DeliveryShipmentDTO> DeliveryShipmentDtos { get; set; }

        public GoodsWriteOffPage(IGoodsWriteOffService goodsWriteOffService,
            IWriteOffReasonService writeOffReasonService,
            IDeliveryShipmentService deliveryShipmentService,
            IGoodsInMarketService goodsInMarketService,
            IGoodsService goodsService,
            IOptions<AppSettings> settings, IMapper mapper)
        {
            _goodsWriteOffService = goodsWriteOffService;
            _writeOffReasonService = writeOffReasonService;
            _deliveryShipmentService = deliveryShipmentService;
            _goodsInMarketService = goodsInMarketService;
            _goodsService = goodsService;
            _mapper = mapper;
            _settings = settings.Value;

            InitializeComponent();
        }

        private void UpdateDataGrid()
        {
            GoodsWriteOffDtos =
                _mapper.Map<List<GoodsWriteOff>, List<GoodsWriteOffDTO>>(_goodsWriteOffService.GetAll());

            DataGrid.ItemsSource = GoodsWriteOffDtos;
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

            if (ShipmentDateComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select Shipment date!");
                return false;
            }

            if (ReasonComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select reason!");
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
            GoodsWriteOff goodsWriteOff = new GoodsWriteOff();
            goodsWriteOff.Id = GoodsWriteOffDtos[^1]?.Id + 1 ?? 1;
            goodsWriteOff.Amount = Convert.ToDouble(AmountTextBox.Text);
            goodsWriteOff.Date = DateTime.Now;
            goodsWriteOff.IdEmployee = _currentEmployee.Id;
            var tempShipment = (DeliveryShipmentDTO) ShipmentDateComboBox.SelectedItem;
            goodsWriteOff.IdDeliveryShipment = tempShipment.Id;
            var shipment = _deliveryShipmentService.GetId(tempShipment.Id);
            goodsWriteOff.IdGoodsInMarket = shipment.IdGoodsInMarket;
            var reason = (WriteOffReasonDTO) ReasonComboBox.SelectedItem;
            goodsWriteOff.IdWriteOffReason = reason.Id;

            _goodsWriteOffService.Create(goodsWriteOff);
            UpdateDataGrid();
        }

        private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            _goodsWriteOffService.Delete(GoodsWriteOffDtos[DataGrid.SelectedIndex].Id);
            UpdateDataGrid();
        }

        private void ProductCodeTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (Regex.Match(ProductCodeTextBox.Text, @"^\d{5}$").Success)
            {
                GoodsDTO tempGoodsDto;
                if ((tempGoodsDto = _mapper.Map<Goods, GoodsDTO>(_goodsService.GetAll()
                        .FirstOrDefault(item => item.ProductCode == ProductCodeTextBox.Text))) != null)
                {
                    GoodTitleLabel.Content = "Good: " + tempGoodsDto.Title;
                    CategoryLabel.Content = "Category: " + tempGoodsDto.CategoryTitle;
                    WeightLabel.Content = "Unit weight: " + tempGoodsDto.Weight;
                    PriceLabel.Content = "Price: " + $"{tempGoodsDto.Price,0:C2}";

                    GoodsInMarketDTO tempGimo = _mapper.Map<GoodsInMarket, GoodsInMarketDTO>(_goodsInMarketService
                        .GetAll().FirstOrDefault(item =>
                            item.IdGoods == tempGoodsDto.Id &&
                            item.IdMarketNavigation.Address == _currentEmployee.MarketAddress));
                    if ((DeliveryShipmentDtos =
                        _mapper.Map<List<DeliveryShipment>, List<DeliveryShipmentDTO>>(_deliveryShipmentService.GetAll()
                            .Where(item => item.IdGoodsInMarket == tempGimo.Id).ToList())).Count > 0)
                    {
                        ShipmentDateComboBox.ItemsSource = DeliveryShipmentDtos;
                        ShipmentDateComboBox.IsEnabled = true;
                    }
                    else
                    {
                        DeliveryShipmentDtos = null;
                        ShipmentDateComboBox.ItemsSource = null;
                        ShipmentDateComboBox.IsEnabled = false;
                    }
                }
                else
                {
                    GoodTitleLabel.Content = "";
                    CategoryLabel.Content = "";
                    WeightLabel.Content = "";
                    PriceLabel.Content = "";

                    DeliveryShipmentDtos = null;
                    ShipmentDateComboBox.ItemsSource = null;
                    ShipmentDateComboBox.IsEnabled = false;
                }
            }
        }

        private void ShipmentDateComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ShipmentDateComboBox.SelectedItem != null)
            {
                var tempDeliveryShipment = (DeliveryShipmentDTO) ShipmentDateComboBox.SelectedItem;
                AmountLabel.Content = "Amount: " + tempDeliveryShipment.Amount;
            }
            else
                AmountLabel.Content = "";
        }
    }
}