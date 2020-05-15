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
using GroceryStore.Core.Mapping;
using GroceryStore.Core.Models;
using Microsoft.Extensions.Options;

namespace GroceryStore.Views
{
    /// <summary>
    /// Interaction logic for DeliveryShipmentPage.xaml
    /// </summary>
    public partial class DeliveryShipmentPage : Page, IActivable
    {
        private readonly IDeliveryShipmentService _deliveryShipmentService;
        private readonly IConsignmentService _consignmentService;
        private readonly IGoodsInMarketService _goodsInMarketService;
        private readonly AppSettings _settings;
        private readonly IMapper _mapper;

        public List<DeliveryShipmentDTO> DeliveryShipmentDtos { get; set; }

        public DeliveryShipmentPage(IDeliveryShipmentService deliveryShipmentService,
            IConsignmentService consignmentService,
            IGoodsInMarketService goodsInMarketService,
            IOptions<AppSettings> settings, IMapper mapper)
        {
            _deliveryShipmentService = deliveryShipmentService;
            _consignmentService = consignmentService;
            _goodsInMarketService = goodsInMarketService;
            _mapper = mapper;
            _settings = settings.Value;

            InitializeComponent();

            UpdateDataGrid();
        }

        private void UpdateDataGrid()
        {
            DeliveryShipmentDtos =
                _mapper.Map<List<DeliveryShipment>, List<DeliveryShipmentDTO>>(_deliveryShipmentService.GetAll());

            DataGrid.ItemsSource = DeliveryShipmentDtos;
        }

        private bool ValidateForm()
        {
            if (!Regex.Match(ProductCodeTextBox.Text, @"^\d{5}$").Success)
            {
                MessageBox.Show("Invalid product code! It must contain 5 digits");
                AmountTextBox.Focus();
                return false;
            }

            if (!Regex.Match(NumberTextBox.Text, @"^\d{5,20}$").Success)
            {
                MessageBox.Show("Consignment Number must consist of at least 5 digits and not exceed 20 digits!");
                NumberTextBox.Focus();
                return false;
            }

            if (!Regex.Match(AmountTextBox.Text, @"^[0-9]*(?:\,[0-9]*)?$").Success)
            {
                MessageBox.Show("Invalid amount! Check the data you've entered!");
                AmountTextBox.Focus();
                return false;
            }

            DateTime dt;
            if (!DateTime.TryParse(DateTextBox.Text, out dt))
            {
                MessageBox.Show("Shipment date isn't valid! Check data you've entered!");
                DateTextBox.Focus();
                return false;
            }

            return true;
        }

        public Task ActivateAsync(object parameter)
        {
            return Task.CompletedTask;
        }

        private void CreateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;
            DeliveryShipment deliveryShipment = new DeliveryShipment();
            Consignment tempConsignment;
            GoodsInMarket tempGoodsInMarket;
            deliveryShipment.Id = DeliveryShipmentDtos[^1]?.Id + 1 ?? 1;
            deliveryShipment.Amount = Convert.ToDouble(AmountTextBox.Text);
            deliveryShipment.ShipmentDateTime = DateTime.Parse(DateTextBox.Text);
            if ((tempConsignment = _consignmentService.GetAll()
                    .FirstOrDefault(consignment => consignment.ConsignmentNumber == NumberTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such consignment!");
                return;
            }
            else
                deliveryShipment.IdConsignment = tempConsignment.Id;

            if ((tempGoodsInMarket = _goodsInMarketService.GetAll().FirstOrDefault(goodsInMarket =>
                    goodsInMarket.IdGoodsNavigation.ProductCode == ProductCodeTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such good in market!");
                return;
            }
            else
                deliveryShipment.IdGoodsInMarket = tempGoodsInMarket.Id;

            _deliveryShipmentService.Create(deliveryShipment);
            UpdateDataGrid();
        }

        private void UpdateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            if (!ValidateForm()) return;
            DeliveryShipment deliveryShipment = new DeliveryShipment();
            Consignment tempConsignment;
            GoodsInMarket tempGoodsInMarket;
            deliveryShipment.Id = DeliveryShipmentDtos[DataGrid.SelectedIndex].Id;
            deliveryShipment.Amount = Convert.ToDouble(AmountTextBox.Text);
            deliveryShipment.ShipmentDateTime = DateTime.Parse(DateTextBox.Text);
            if ((tempConsignment = _consignmentService.GetAll()
                    .FirstOrDefault(consignment => consignment.ConsignmentNumber == NumberTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such consignment!");
                return;
            }
            else
                deliveryShipment.IdConsignment = tempConsignment.Id;

            if ((tempGoodsInMarket = _goodsInMarketService.GetAll().FirstOrDefault(goodsInMarket =>
                    goodsInMarket.IdGoodsNavigation.ProductCode == ProductCodeTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such good in market!");
                return;
            }
            else
                deliveryShipment.IdGoodsInMarket = tempGoodsInMarket.Id;

            _deliveryShipmentService.Update(deliveryShipment);
            UpdateDataGrid();
        }

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedIndex != -1)
            {
                ProductCodeTextBox.Text = DeliveryShipmentDtos[DataGrid.SelectedIndex].ProductCode;
                NumberTextBox.Text = DeliveryShipmentDtos[DataGrid.SelectedIndex].ConsignmentNumber;
                AmountTextBox.Text = DeliveryShipmentDtos[DataGrid.SelectedIndex].Amount.ToString();
                DateTextBox.Text = DeliveryShipmentDtos[DataGrid.SelectedIndex].ShipmentDateTime.ToString();
            }
        }

        private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            _deliveryShipmentService.Delete(DeliveryShipmentDtos[DataGrid.SelectedIndex].Id);
            UpdateDataGrid();
        }
    }
}