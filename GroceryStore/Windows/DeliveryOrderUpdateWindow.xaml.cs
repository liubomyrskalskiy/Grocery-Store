using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using AutoMapper;
using GroceryStore.Core.Abstractions;
using GroceryStore.Core.Abstractions.IServices;
using GroceryStore.Core.DTO;
using GroceryStore.Core.Models;
using Microsoft.Extensions.Options;

namespace GroceryStore.Windows
{
    /// <summary>
    ///     Interaction logic for DeliveryOrderUpdateWindow.xaml
    /// </summary>
    public partial class DeliveryOrderUpdateWindow : Window, IActivable
    {
        private readonly IConsignmentService _consignmentService;
        private readonly IDeliveryShipmentService _deliveryShipmentService;
        private readonly IGoodsInMarketService _goodsInMarketService;
        private readonly IMapper _mapper;
        private readonly IMarketService _marketService;
        private readonly AppSettings _settings;
        private Consignment _currentConsignment;

        public DeliveryOrderUpdateWindow(IMarketService marketService, IMapper mapper, IOptions<AppSettings> settings,
            IDeliveryShipmentService deliveryShipmentService, IGoodsInMarketService goodsInMarketService,
            IConsignmentService consignmentService)
        {
            _marketService = marketService;
            _mapper = mapper;
            _deliveryShipmentService = deliveryShipmentService;
            _goodsInMarketService = goodsInMarketService;
            _consignmentService = consignmentService;
            _settings = settings.Value;
            InitializeComponent();
        }

        public List<MarketDTO> MarketDtos { get; set; }
        public List<DeliveryShipmentDTO> DeliveryShipmentDtos { get; set; }
        public List<DeliveryShipmentDTO> CurrentDeliveryShipmentDtos { get; set; }

        public Task ActivateAsync(object parameter)
        {
            _currentConsignment = (Consignment) parameter;
            _currentConsignment.ConsignmentNumber = _currentConsignment.Id.ToString("D12");
            ConsignmentLabel.Content = "Consignment number: " + _currentConsignment.ConsignmentNumber;
            AmountLabel.Content = $"Amount: {_currentConsignment.Amount,0:0.000}";
            MarketDtos = _mapper.Map<List<Market>, List<MarketDTO>>(_marketService.GetAll());
            MarketComboBox.ItemsSource = MarketDtos;

            UpdateDataGrid();
            return Task.CompletedTask;
        }

        private void UpdateDataGrid()
        {
            DeliveryShipmentDtos =
                _mapper.Map<List<DeliveryShipment>, List<DeliveryShipmentDTO>>(_deliveryShipmentService.GetAll());

            DeliveryShipmentDtos.Sort(delegate (DeliveryShipmentDTO x, DeliveryShipmentDTO y)
            {
                return x.Id.CompareTo(y.Id);
            });

            CurrentDeliveryShipmentDtos = DeliveryShipmentDtos
                .Where(item => item.ConsignmentNumber == _currentConsignment.ConsignmentNumber).ToList();
            DataGrid.ItemsSource = CurrentDeliveryShipmentDtos;
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

            double totalAmount = 0;
            foreach (var currentDeliveryShipmentDto in CurrentDeliveryShipmentDtos)
                totalAmount += currentDeliveryShipmentDto.Amount ?? 0;

            totalAmount += Convert.ToDouble(AmountTextBox.Text);
            if (totalAmount > _currentConsignment.Amount)
            {
                MessageBox.Show("Invalid amount! You're trying to distribute more goods than actually ordered!");
                AmountTextBox.Focus();
                return false;
            }

            if (!DateTime.TryParse(shipmentDateTextBox.Text, out _))
            {
                MessageBox.Show("Invalid shipment date! Check the data you've entered!");
                shipmentDateTextBox.Focus();
                return false;
            }


            return true;
        }

        private bool ValidateDateForm()
        {
            if (!DateTime.TryParse(ManufactureDateTextBox.Text, out _))
            {
                MessageBox.Show("Invalid manufacture date! Check the data you've entered!");
                ManufactureDateTextBox.Focus();
                return false;
            }

            if (!DateTime.TryParse(BestBeforeTextBox.Text, out _))
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
                _deliveryShipmentService.Delete(currentDeliveryShipmentDto.Id);

            _currentConsignment.ConsignmentNumber = "";
            Close();
        }


        private void CreateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;
            var deliveryShipment = new DeliveryShipment();
            GoodsInMarket tempGoodsInMarket;
            deliveryShipment.Id = DeliveryShipmentDtos[^1]?.Id + 1 ?? 1;
            deliveryShipment.IdConsignment = _currentConsignment.Id;
            deliveryShipment.Amount = Convert.ToDouble(AmountTextBox.Text);
            deliveryShipment.ShipmentDateTime = DateTime.Parse(shipmentDateTextBox.Text);
            var tempMarket = (MarketDTO) MarketComboBox.SelectedItem;
            if ((tempGoodsInMarket = _goodsInMarketService.GetAll()
                    .FirstOrDefault(item =>
                        item.IdGoods == _currentConsignment.IdGoods &&
                        item.IdMarketNavigation.Address == tempMarket.Address &&
                        item.IdMarketNavigation.IdCityNavigation.Title == tempMarket.CityTitle)) ==
                null)
            {
                MessageBox.Show("There is no such good in this store! Good added automatically!");
                GoodsInMarket goodsInMarket = new GoodsInMarket
                {
                    IdGoods = _currentConsignment.IdGoods,
                    Amount = 0,
                    IdMarket = tempMarket.Id
                };

                var tempList = _goodsInMarketService.GetAll();
                tempList.Sort(delegate (GoodsInMarket x, GoodsInMarket y) {
                    return x.Id.CompareTo(y.Id);
                });

                goodsInMarket.Id = tempList[^1]?.Id + 1 ?? 1;

                _goodsInMarketService.Create(goodsInMarket);
                tempGoodsInMarket = new GoodsInMarket
                {
                    Id = goodsInMarket.Id
                };
            }

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

        private void MarketComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if(MarketComboBox.SelectedItem != null)
            {
                shipmentDateTextBox.Text = DateTime.Now.ToString();
            }
        }
    }
}