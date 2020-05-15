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
        private readonly IEmployeeService _employeeService;
        private readonly IWriteOffReasonService _writeOffReasonService;
        private readonly IDeliveryShipmentService _deliveryShipmentService;
        private readonly IConsignmentService _consignmentService;
        private readonly IGoodsInMarketService _goodsInMarketService;
        private readonly IGoodsService _goodsService;
        private readonly AppSettings _settings;
        private readonly IMapper _mapper;

        public List<GoodsWriteOffDTO> GoodsWriteOffDtos { get; set; }

        public GoodsWriteOffPage(IGoodsWriteOffService goodsWriteOffService,
            IEmployeeService employeeService,
            IWriteOffReasonService writeOffReasonService,
            IDeliveryShipmentService deliveryShipmentService,
            IConsignmentService consignmentService,
            IGoodsInMarketService goodsInMarketService,
            IGoodsService goodsService,
            IOptions<AppSettings> settings, IMapper mapper)
        {
            _goodsWriteOffService = goodsWriteOffService;
            _employeeService = employeeService;
            _writeOffReasonService = writeOffReasonService;
            _deliveryShipmentService = deliveryShipmentService;
            _consignmentService = consignmentService;
            _goodsInMarketService = goodsInMarketService;
            _goodsService = goodsService;
            _mapper = mapper;
            _settings = settings.Value;

            InitializeComponent();

            UpdateDataGrid();
        }

        private void UpdateDataGrid()
        {
            GoodsWriteOffDtos =
                _mapper.Map<List<GoodsWriteOff>, List<GoodsWriteOffDTO>>(_goodsWriteOffService.GetAll());

            DataGrid.ItemsSource = GoodsWriteOffDtos;
        }

        private bool ValidateForm()
        {
            if (!Regex.Match(ConsignmentTextBox.Text, @"^\d{5,20}$").Success)
            {
                MessageBox.Show(
                    "Invalid Consignment number! It must contain at least 5 digits and not exceed 20 digits");
                ConsignmentTextBox.Focus();
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
                MessageBox.Show("Date isn't valid! Check data you've entered!");
                DateTextBox.Focus();
                return false;
            }

            if (!Regex.Match(LoginTextBox.Text, @"^\D{6,20}$").Success)
            {
                MessageBox.Show("Login must consist of at least 6 character and not exceed 20 characters!");
                LoginTextBox.Focus();
                return false;
            }

            if (!Regex.Match(ReasonTextBox.Text, @"^\D{1,150}$").Success)
            {
                MessageBox.Show("Reason must consist of at least 6 character and not exceed 150 characters!");
                ReasonTextBox.Focus();
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
            GoodsWriteOff goodsWriteOff = new GoodsWriteOff();
            Employee tempEmployee;
            WriteOffReason tempWriteOffReason;
            DeliveryShipment tempDeliveryShipment;
            goodsWriteOff.Id = GoodsWriteOffDtos[^1]?.Id + 1 ?? 1;
            goodsWriteOff.Amount = Convert.ToDouble(AmountTextBox.Text);
            goodsWriteOff.Date = DateTime.Parse(DateTextBox.Text);

            if ((tempEmployee = _employeeService.GetAll()
                    .FirstOrDefault(employee => employee.Login == LoginTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such employee surname!");
                return;
            }
            else
                goodsWriteOff.IdEmployee = tempEmployee.Id;

            if ((tempWriteOffReason = _writeOffReasonService.GetAll()
                    .FirstOrDefault(reason => reason.Description == ReasonTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such writeo-ff reason!");
                return;
            }
            else
                goodsWriteOff.IdWriteOffReason = tempWriteOffReason.Id;

            if ((tempDeliveryShipment = _deliveryShipmentService.GetAll().FirstOrDefault(deliveryShipment =>
                    deliveryShipment.IdConsignmentNavigation.ConsignmentNumber == ConsignmentTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such consignment!");
                return;
            }
            else
                goodsWriteOff.IdDeliveryShipment = tempDeliveryShipment.Id;

            if ((tempDeliveryShipment = _deliveryShipmentService.GetAll().FirstOrDefault(deliveryShipment =>
                    deliveryShipment.IdConsignmentNavigation.ConsignmentNumber == ConsignmentTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such good in market!");
                return;
            }
            else
                goodsWriteOff.IdGoodsInMarket = tempDeliveryShipment.IdGoodsInMarket;

            _goodsWriteOffService.Create(goodsWriteOff);
            UpdateDataGrid();
        }

        private void UpdateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            if (!ValidateForm()) return;
            GoodsWriteOff goodsWriteOff = new GoodsWriteOff();
            Employee tempEmployee;
            WriteOffReason tempWriteOffReason;
            DeliveryShipment tempDeliveryShipment;
            goodsWriteOff.Id = GoodsWriteOffDtos[DataGrid.SelectedIndex].Id;
            goodsWriteOff.Amount = Convert.ToDouble(AmountTextBox.Text);
            goodsWriteOff.Date = DateTime.Parse(DateTextBox.Text);

            if ((tempEmployee = _employeeService.GetAll()
                    .FirstOrDefault(employee => employee.Login == LoginTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such employee surname!");
                return;
            }
            else
                goodsWriteOff.IdEmployee = tempEmployee.Id;

            if ((tempWriteOffReason = _writeOffReasonService.GetAll()
                    .FirstOrDefault(reason => reason.Description == ReasonTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such writeo-ff reason!");
                return;
            }
            else
                goodsWriteOff.IdWriteOffReason = tempWriteOffReason.Id;

            if ((tempDeliveryShipment = _deliveryShipmentService.GetAll().FirstOrDefault(deliveryShipment =>
                    deliveryShipment.IdConsignmentNavigation.ConsignmentNumber == ConsignmentTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such consignment!");
                return;
            }
            else
                goodsWriteOff.IdDeliveryShipment = tempDeliveryShipment.Id;

            if ((tempDeliveryShipment = _deliveryShipmentService.GetAll().FirstOrDefault(deliveryShipment =>
                    deliveryShipment.IdConsignmentNavigation.ConsignmentNumber == ConsignmentTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such good in market!");
                return;
            }
            else
                goodsWriteOff.IdGoodsInMarket = tempDeliveryShipment.IdGoodsInMarket;

            _goodsWriteOffService.Update(goodsWriteOff);
            UpdateDataGrid();
        }

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedIndex != -1)
            {
                AmountTextBox.Text = GoodsWriteOffDtos[DataGrid.SelectedIndex].Amount.ToString();
                DateTextBox.Text = GoodsWriteOffDtos[DataGrid.SelectedIndex].Date.ToString();
                LoginTextBox.Text = GoodsWriteOffDtos[DataGrid.SelectedIndex].Login;
                ReasonTextBox.Text = GoodsWriteOffDtos[DataGrid.SelectedIndex].Reason;
                ConsignmentTextBox.Text = GoodsWriteOffDtos[DataGrid.SelectedIndex].ConsignmentNumber;
            }
        }

        private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            _goodsWriteOffService.Delete(GoodsWriteOffDtos[DataGrid.SelectedIndex].Id);
            UpdateDataGrid();
        }
    }
}