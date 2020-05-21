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
using System.Windows.Shapes;
using AutoMapper;
using GroceryStore.Core.Abstractions;
using GroceryStore.Core.Abstractions.IServices;
using GroceryStore.Core.DTO;
using GroceryStore.Core.Models;
using Microsoft.Extensions.Options;

namespace GroceryStore.Windows
{
    /// <summary>
    /// Interaction logic for DeliveryOrderUpdateWindow.xaml
    /// </summary>
    public partial class DeliveryOrderUpdateWindow : Window, IActivable
    {
        private readonly IDeliveryShipmentService _deliveryShipmentService;
        private readonly IGoodsInMarketService _goodsInMarketService;
        private readonly IMarketService _marketService;
        private IConsignmentService _consignmentService;
        private AppSettings _settings;
        private readonly IMapper _mapper;
        private Consignment _currentConsignment;

        public List<MarketDTO> MarketDtos { get; set; }
        public List<DeliveryShipmentDTO> DeliveryShipmentDtos { get; set; }
        public List<DeliveryShipmentDTO> CurrentDeliveryShipmentDtos { get; set; }

        public DeliveryOrderUpdateWindow(IMarketService marketService, IMapper mapper, IOptions<AppSettings> settings, IDeliveryShipmentService deliveryShipmentService, IGoodsInMarketService goodsInMarketService, IConsignmentService consignmentService)
        {
            _marketService = marketService;
            _mapper = mapper;
            _deliveryShipmentService = deliveryShipmentService;
            _goodsInMarketService = goodsInMarketService;
            _consignmentService = consignmentService;
            _settings = settings.Value;
            InitializeComponent();
        }

        private void UpdateDataGrid()
        {
            DeliveryShipmentDtos =
                _mapper.Map<List<DeliveryShipment>, List<DeliveryShipmentDTO>>(_deliveryShipmentService.GetAll());
            CurrentDeliveryShipmentDtos = DeliveryShipmentDtos
                .Where(item => item.ConsignmentNumber == _currentConsignment.ConsignmentNumber).ToList();
            DataGrid.ItemsSource = CurrentDeliveryShipmentDtos;
        }

        public Task ActivateAsync(object parameter)
        {
            _currentConsignment = (Consignment)parameter;
            _currentConsignment.ConsignmentNumber = _currentConsignment.Id.ToString("D12");
            ConsignmentLabel.Content = "Consignment number: " + _currentConsignment.ConsignmentNumber;
            AmountLabel.Content = $"Amount: {_currentConsignment.Amount,0:0.000}";
            MarketDtos = _mapper.Map<List<Market>, List<MarketDTO>>(_marketService.GetAll());
            MarketComboBox.ItemsSource = MarketDtos;

            UpdateDataGrid();
            return Task.CompletedTask;
        }

        private bool ValidateForm()
        {
            if (MarketComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select market");
                return false;
            }
            if (!Regex.Match(AmountTextBox.Text, @"^[0-9]*(?:\,[0-9]*)?$").Success)
            {
                MessageBox.Show("Invalid amount! Check the data you've entered!");
                AmountTextBox.Focus();
                return false;
            }
            else
            {
                double totalAmount = 0;
                foreach (var currentDeliveryShipmentDto in CurrentDeliveryShipmentDtos)
                {
                    totalAmount += currentDeliveryShipmentDto.Amount ?? 0;
                }

                totalAmount += Convert.ToDouble(AmountTextBox.Text);
                if (totalAmount > _currentConsignment.Amount)
                {
                    MessageBox.Show("Invalid amount! You're trying to distribute more goods than actually ordered!");
                    AmountTextBox.Focus();
                    return false;
                }
            }

            DateTime dt;
            if (!DateTime.TryParse(shipmentDateTextBox.Text, out dt))
            {
                MessageBox.Show("Invalid shipment date! Check the data you've entered!");
                shipmentDateTextBox.Focus();
                return false;
            }


            return true;
        }

        private bool ValidateDateForm()
        {
            DateTime dt;
            if (!DateTime.TryParse(ManufactureDateTextBox.Text, out dt))
            {
                MessageBox.Show("Invalid manufacture date! Check the data you've entered!");
                ManufactureDateTextBox.Focus();
                return false;
            }
            if (!DateTime.TryParse(BestBeforeTextBox.Text, out dt))
            {
                MessageBox.Show("Invalid best before date! Check the data you've entered!");
                BestBeforeTextBox.Focus();
                return false;
            }
            return true;
        }

        private void DoneGoodBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateDateForm()) return;
            _currentConsignment.ManufactureDate = DateTime.Parse(ManufactureDateTextBox.Text);
            _currentConsignment.BestBefore = DateTime.Parse(BestBeforeTextBox.Text);

            _consignmentService.Update(_currentConsignment);
            Close();
        }

        private void BtnClose(object sender, RoutedEventArgs e)
        {
            foreach (var currentDeliveryShipmentDto in CurrentDeliveryShipmentDtos)
            {
                _deliveryShipmentService.Delete(currentDeliveryShipmentDto.Id);
            }

            _currentConsignment.ConsignmentNumber = "";
            Close();
        }


        private void CreateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;
            DeliveryShipment deliveryShipment = new DeliveryShipment();
            GoodsInMarket tempGoodsInMarket;
            deliveryShipment.Id = DeliveryShipmentDtos[^1]?.Id + 1 ?? 1;
            deliveryShipment.IdConsignment = _currentConsignment.Id;
            deliveryShipment.Amount = Convert.ToDouble(AmountTextBox.Text);
            deliveryShipment.ShipmentDateTime = DateTime.Parse(shipmentDateTextBox.Text);
            MarketDTO tempMarket = (MarketDTO)MarketComboBox.SelectedItem;
            if ((tempGoodsInMarket = _goodsInMarketService.GetAll()
                    .FirstOrDefault(item =>
                        item.IdGoods == _currentConsignment.IdGoods &&
                        item.IdMarketNavigation.Address == tempMarket.Address &&
                        item.IdMarketNavigation.IdCityNavigation.Title == tempMarket.CityTitle)) ==
                null)
            {
                MessageBox.Show("There is no such goods in this market! Check Goods in Market page");
                return;
            }
            else
                deliveryShipment.IdGoodsInMarket = tempGoodsInMarket.Id;

            _deliveryShipmentService.Create(deliveryShipment);
            UpdateDataGrid();
        }

        private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            _deliveryShipmentService.Delete(CurrentDeliveryShipmentDtos[DataGrid.SelectedIndex].Id);
            UpdateDataGrid();
        }
    }
}
