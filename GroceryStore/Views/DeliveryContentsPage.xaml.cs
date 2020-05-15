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
    /// Interaction logic for DeliveryContentsPage.xaml
    /// </summary>
    public partial class DeliveryContentsPage : Page, IActivable
    {
        private readonly IDeliveryContentsService _deliveryContentsService;
        private readonly IConsignmentService _consignmentService;
        private readonly IDeliveryService _deliveryService;
        private readonly AppSettings _settings;
        private readonly IMapper _mapper;

        public List<DeliveryContentsDTO> DeliveryContentsDtos { get; set; }

        public DeliveryContentsPage(IDeliveryContentsService deliveryContentsService,
            IConsignmentService consignmentService, IDeliveryService deliveryService, IOptions<AppSettings> settings,
            IMapper mapper)
        {
            _deliveryContentsService = deliveryContentsService;
            _consignmentService = consignmentService;
            _deliveryService = deliveryService;
            _mapper = mapper;
            _settings = settings.Value;

            InitializeComponent();

            UpdateDataGrid();
        }

        private void UpdateDataGrid()
        {
            DeliveryContentsDtos =
                _mapper.Map<List<DeliveryContents>, List<DeliveryContentsDTO>>(_deliveryContentsService.GetAll());

            DataGrid.ItemsSource = DeliveryContentsDtos;
        }

        private bool ValidateForm()
        {
            if (!Regex.Match(ConsignmentTextBox.Text, @"^\d{5,20}$").Success)
            {
                MessageBox.Show("Consignment Number must consist of at least 5 digits and not exceed 20 digits!");
                ConsignmentTextBox.Focus();
                return false;
            }

            if (!Regex.Match(DeliveryTextBox.Text, @"^\d{5,20}$").Success)
            {
                MessageBox.Show("Delivery Number must consist of at least 5 digits and not exceed 20 digits!");
                ConsignmentTextBox.Focus();
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
            DeliveryContents deliveryContents = new DeliveryContents();
            Delivery tempDelivery;
            Consignment tempConsignment;
            deliveryContents.Id = DeliveryContentsDtos[^1]?.Id + 1 ?? 1;
            if ((tempConsignment = _consignmentService.GetAll().FirstOrDefault(consignment =>
                    consignment.ConsignmentNumber == ConsignmentTextBox.Text.ToString())) == null)
            {
                MessageBox.Show("There is no such consignment!");
                return;
            }
            else
                deliveryContents.IdConsignment = tempConsignment.Id;

            if ((tempDelivery = _deliveryService.GetAll()
                    .FirstOrDefault(delivery => delivery.DeliveryNumber == DeliveryTextBox.Text.ToString())) == null)
            {
                MessageBox.Show("There is no such delivery!");
                return;
            }
            else
                deliveryContents.IdDelivery = tempDelivery.Id;

            _deliveryContentsService.Create(deliveryContents);
            UpdateDataGrid();
        }

        private void UpdateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            if (!ValidateForm()) return;
            DeliveryContents deliveryContents = new DeliveryContents();
            deliveryContents.Id = DeliveryContentsDtos[DataGrid.SelectedIndex].Id;
            Delivery tempDelivery;
            Consignment tempConsignment;
            if ((tempConsignment = _consignmentService.GetAll().FirstOrDefault(consignment =>
                    consignment.ConsignmentNumber == ConsignmentTextBox.Text.ToString())) == null)
            {
                MessageBox.Show("There is no such consignment!");
                return;
            }
            else
                deliveryContents.IdConsignment = tempConsignment.Id;

            if ((tempDelivery = _deliveryService.GetAll()
                    .FirstOrDefault(delivery => delivery.DeliveryNumber == DeliveryTextBox.Text.ToString())) == null)
            {
                MessageBox.Show("There is no such delivery!");
                return;
            }
            else
                deliveryContents.IdDelivery = tempDelivery.Id;

            _deliveryContentsService.Update(deliveryContents);
            UpdateDataGrid();
        }

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedIndex != -1)
            {
                ConsignmentTextBox.Text = DeliveryContentsDtos[DataGrid.SelectedIndex].ConsignmentNumber;
                DeliveryTextBox.Text = DeliveryContentsDtos[DataGrid.SelectedIndex].DeliveryNumber;
            }
        }

        private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            _deliveryContentsService.Delete(DeliveryContentsDtos[DataGrid.SelectedIndex].Id);
            UpdateDataGrid();
        }
    }
}