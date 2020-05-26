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
    ///     Interaction logic for GoodsWriteOffPage.xaml
    /// </summary>
    public partial class GoodsWriteOffPage : Page, IActivable
    {
        private readonly IDeliveryShipmentService _deliveryShipmentService;
        private readonly IGoodsInMarketService _goodsInMarketService;
        private readonly IGoodsService _goodsService;
        private readonly IGoodsWriteOffService _goodsWriteOffService;
        private readonly IMapper _mapper;
        private readonly AppSettings _settings;
        private readonly IWriteOffReasonService _writeOffReasonService;
        private EmployeeDTO _currentEmployee;

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

            ShipmentDateComboBox.IsEnabled = false;
        }

        public List<GoodsWriteOffDTO> GoodsWriteOffDtos { get; set; }
        public List<GoodsWriteOffDTO> FilteredGoodsWriteOffDtos { get; set; }
        public List<WriteOffReasonDTO> WriteOffReasonDtos { get; set; }
        public List<DeliveryShipmentDTO> DeliveryShipmentDtos { get; set; }

        public Task ActivateAsync(object parameter)
        {
            _currentEmployee = (EmployeeDTO) parameter;
            WriteOffReasonDtos =
                _mapper.Map<List<WriteOffReason>, List<WriteOffReasonDTO>>(_writeOffReasonService.GetAll());
            ReasonComboBox.ItemsSource = WriteOffReasonDtos;
            UpdateDataGrid();
            return Task.CompletedTask;
        }

        private void UpdateDataGrid()
        {
            GoodsWriteOffDtos =
                _mapper.Map<List<GoodsWriteOff>, List<GoodsWriteOffDTO>>(_goodsWriteOffService.GetAll());

            GoodsWriteOffDtos.Sort(delegate (GoodsWriteOffDTO x, GoodsWriteOffDTO y)
            {
                return x.Id.CompareTo(y.Id);
            });

            FilteredGoodsWriteOffDtos = GoodsWriteOffDtos;

            if (Regex.Match(TitleFilterTextBox.Text, @"^\D{1,20}$").Success)
            {
                var tempList = FilteredGoodsWriteOffDtos.Where(item => item.GoodTitle.Contains(TitleFilterTextBox.Text))
                    .ToList();
                FilteredGoodsWriteOffDtos = tempList;
            }

            if (DateFromFilterTextBox.Text != "")
            {
                var tempDate = DateTime.Parse(DateFromFilterTextBox.Text);
                var tempList = FilteredGoodsWriteOffDtos
                    .Where(item => DateTime.Compare(item.Date ?? default, tempDate) >= 0).ToList();
                FilteredGoodsWriteOffDtos = tempList;
            }

            if (DateToFilterTextBox.Text != "")
            {
                var tempDate = DateTime.Parse(DateToFilterTextBox.Text);
                var tempList = FilteredGoodsWriteOffDtos
                    .Where(item => DateTime.Compare(item.Date ?? default, tempDate) <= 0).ToList();
                FilteredGoodsWriteOffDtos = tempList;
            }

            DataGrid.ItemsSource = FilteredGoodsWriteOffDtos;
        }

        private bool ValidateForm()
        {
            if (!Regex.Match(ProductCodeTextBox.Text, @"^\d{5}$").Success)
            {
                MessageBox.Show("Invalid product code! It must contain 5 digits");
                ProductCodeTextBox.Focus();
                return false;
            }

            if (ShipmentDateComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select Shipment date!");
                return false;
            }

            var tempDeliveryShipment = (DeliveryShipmentDTO)ShipmentDateComboBox.SelectedItem;
            if (!Regex.Match(AmountTextBox.Text, @"^[0-9]*(?:\,[0-9]*)?$").Success || Convert.ToDouble(AmountTextBox.Text) > tempDeliveryShipment.Amount)
            {
                MessageBox.Show("Invalid amount! Check the data you've entered! Or you're trying to write off more than it is in stock!");
                AmountTextBox.Focus();
                return false;
            }

            if (ReasonComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select reason!");
                return false;
            }

            return true;
        }

        private void CreateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;
            var goodsWriteOff = new GoodsWriteOff
            {
                Id = GoodsWriteOffDtos[^1]?.Id + 1 ?? 1,
                Amount = Convert.ToDouble(AmountTextBox.Text),
                Date = DateTime.Now,
                IdEmployee = _currentEmployee.Id
            };
            var tempShipment = (DeliveryShipmentDTO) ShipmentDateComboBox.SelectedItem;
            goodsWriteOff.IdDeliveryShipment = tempShipment.Id;
            var shipment = _deliveryShipmentService.GetId(tempShipment.Id);
            goodsWriteOff.IdGoodsInMarket = shipment.IdGoodsInMarket;
            var reason = (WriteOffReasonDTO) ReasonComboBox.SelectedItem;
            goodsWriteOff.IdWriteOffReason = reason.Id;

            _goodsWriteOffService.Create(goodsWriteOff);

            var goodsInMarket = _goodsInMarketService.GetId(goodsWriteOff.IdGoodsInMarket ?? default);
            _goodsInMarketService.Refresh(goodsInMarket);
            var deliveryShipment = _deliveryShipmentService.GetId(goodsWriteOff.IdDeliveryShipment ?? default);
            _deliveryShipmentService.Refresh(deliveryShipment);

            UpdateDataGrid();
        }

        private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            _goodsWriteOffService.Delete(FilteredGoodsWriteOffDtos[DataGrid.SelectedIndex].Id);
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

                    var tempGimo = _mapper.Map<GoodsInMarket, GoodsInMarketDTO>(_goodsInMarketService
                        .GetAll().FirstOrDefault(item =>
                            item.IdGoods == tempGoodsDto.Id &&
                            item.IdMarketNavigation.Address == _currentEmployee.MarketAddress));
                    if ((DeliveryShipmentDtos =
                            _mapper.Map<List<DeliveryShipment>, List<DeliveryShipmentDTO>>(_deliveryShipmentService
                                .GetAll()
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

        private void ShipmentDateComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ShipmentDateComboBox.SelectedItem != null)
            {
                var tempDeliveryShipment = (DeliveryShipmentDTO) ShipmentDateComboBox.SelectedItem;
                ShipedAmountLabel.Content = $"Shiped: {tempDeliveryShipment.StringAmount}";
                var tempGoodInMarket = _goodsInMarketService.GetId(_deliveryShipmentService.GetId(tempDeliveryShipment.Id).IdGoodsInMarket ?? 0);
                AmountLabel.Content = $"In stock: {tempGoodInMarket.Amount}";
            }
            else
            {
                AmountLabel.Content = "";
            }
        }

        private void ClearTitleFilterBtn_OnClick(object sender, RoutedEventArgs e)
        {
            TitleFilterTextBox.Text = "";
            UpdateDataGrid();
        }

        private void SearchTitleBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (Regex.Match(TitleFilterTextBox.Text, @"^\D{1,20}$").Success)
            {
                UpdateDataGrid();
            }
            else
            {
                MessageBox.Show("Title must consist of at least 1 character and not exceed 20 characters!");
                TitleFilterTextBox.Focus();
            }
        }

        private void ClearDateFromFilterFilterBtn_OnClick(object sender, RoutedEventArgs e)
        {
            DateFromFilterTextBox.Text = "";
            UpdateDataGrid();
        }

        private void SearchDateFromFilterBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DateTime.TryParse(DateFromFilterTextBox.Text, out _))
            {
                UpdateDataGrid();
            }
            else
            {
                MessageBox.Show("Cannot parse date you've entered! Please check data you've entered");
                DateFromFilterTextBox.Focus();
            }
        }

        private void ClearDateToFilterBtn_OnClick(object sender, RoutedEventArgs e)
        {
            DateToFilterTextBox.Text = "";
            UpdateDataGrid();
        }

        private void SearchDateToFilterBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DateTime.TryParse(DateToFilterTextBox.Text, out _))
            {
                UpdateDataGrid();
            }
            else
            {
                MessageBox.Show("Cannot parse date you've entered! Please check data you've entered");
                DateToFilterTextBox.Focus();
            }
        }
    }
}