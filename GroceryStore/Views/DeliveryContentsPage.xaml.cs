using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AutoMapper;
using GroceryStore.Core.Abstractions;
using GroceryStore.Core.Abstractions.IServices;
using GroceryStore.Core.DTO;
using GroceryStore.Core.Models;
using GroceryStore.Windows;
using Microsoft.Extensions.Options;

namespace GroceryStore.Views
{
    /// <summary>
    ///     Interaction logic for DeliveryContentsPage.xaml
    /// </summary>
    public partial class DeliveryContentsPage : Page, IActivable
    {
        private readonly IConsignmentService _consignmentService;
        private readonly IDeliveryContentsService _deliveryContentsService;
        private readonly IDeliveryService _deliveryService;
        private readonly IGoodsService _goodsService;
        private readonly IMapper _mapper;
        private readonly SimpleNavigationService _navigationService;
        private readonly IProviderService _providerService;
        private readonly AppSettings _settings;

        public DeliveryContentsPage(IDeliveryContentsService deliveryContentsService,
            IConsignmentService consignmentService, IDeliveryService deliveryService, IOptions<AppSettings> settings,
            IMapper mapper, IProviderService providerService, SimpleNavigationService navigationService,
            IGoodsService goodsService)
        {
            _deliveryContentsService = deliveryContentsService;
            _consignmentService = consignmentService;
            _deliveryService = deliveryService;
            _mapper = mapper;
            _providerService = providerService;
            _navigationService = navigationService;
            _goodsService = goodsService;
            _settings = settings.Value;

            InitializeComponent();
        }


        public List<DeliveryContentsDTO> DeliveryContentsDtos { get; set; }
        public List<DeliveryContentsDTO> FilteredDeliveryContentsDtos { get; set; }
        public List<ProviderDTO> ProviderDtos { get; set; }
        public List<DeliveryDTO> DeliveryDtos { get; set; }
        public List<DeliveryDTO> FilteredDeliveryDtos { get; set; }
        public List<GoodsDTO> GoodsDtos { get; set; }

        public Task ActivateAsync(object parameter)
        {
            ProviderDtos = _mapper.Map<List<Provider>, List<ProviderDTO>>(_providerService.GetAll());
            ProviderComboBox.ItemsSource = ProviderDtos;
            ProviderFilterComboBox.ItemsSource = ProviderDtos;

            UpdateDataGrid();
            return Task.CompletedTask;
        }

        private void UpdateDataGrid()
        {
            DeliveryContentsDtos =
                _mapper.Map<List<DeliveryContents>, List<DeliveryContentsDTO>>(_deliveryContentsService.GetAll());

            DeliveryContentsDtos.Sort(delegate (DeliveryContentsDTO x, DeliveryContentsDTO y)
            {
                return x.Id.CompareTo(y.Id);
            });

            DeliveryDtos = _mapper.Map<List<Delivery>, List<DeliveryDTO>>(_deliveryService.GetAll());
            foreach (var deliveryDto in DeliveryDtos)
            {
                var tempList = DeliveryContentsDtos
                    .Where(item => item.DeliveryNumber.Equals(deliveryDto.DeliveryNumber)).ToList();
                deliveryDto.Total = 0.00;
                foreach (var deliveryContentsDto in tempList)
                    deliveryDto.Total += deliveryContentsDto.OrderAmount * deliveryContentsDto.IncomePrice ?? default;

                deliveryDto.StringTotal = deliveryDto.Total.ToString("C2");
            }

            FilteredDeliveryDtos = DeliveryDtos;

            if (DateFromFilterTextBox.Text != "")
            {
                var tempDate = DateTime.Parse(DateFromFilterTextBox.Text);
                var tempList = FilteredDeliveryDtos
                    .Where(item => DateTime.Compare(item.DeliveryDate ?? default, tempDate) >= 0).ToList();
                FilteredDeliveryDtos = tempList;
            }

            if (DateToFilterTextBox.Text != "")
            {
                var tempDate = DateTime.Parse(DateToFilterTextBox.Text);
                var tempList = FilteredDeliveryDtos
                    .Where(item => DateTime.Compare(item.DeliveryDate ?? default, tempDate) <= 0).ToList();
                FilteredDeliveryDtos = tempList;
            }

            if (ProviderFilterComboBox.SelectedItem != null)
            {
                var tempProvider = (ProviderDTO) ProviderFilterComboBox.SelectedItem;
                var tempList = FilteredDeliveryDtos.Where(item => item.ProviderTitle == tempProvider.CompanyTitle)
                    .ToList();
                FilteredDeliveryDtos = tempList;
            }

            DataGrid.ItemsSource = FilteredDeliveryDtos;
        }

        private bool ValidateForm()
        {
            if (ProviderComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select provider!");
                return false;
            }

            return true;
        }

        private async void CreateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;
            var delivery = new Delivery();
            var tempProvider = (ProviderDTO) ProviderComboBox.SelectedItem;
            delivery.Id = DeliveryDtos[^1]?.Id + 1 ?? 1;
            delivery.IdProvider = tempProvider.Id;
            delivery.DeliveryDate = DateTime.Now;
            delivery.DeliveryNumber = delivery.Id.ToString("D12");

            _deliveryService.Create(delivery);

            var result = await _navigationService.ShowDialogAsync<DeliveryOrderWindow>(delivery);

            UpdateDataGrid();
        }

        private async void DataGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            await _navigationService.ShowDialogAsync<DeliveryDetailWindow>(
                FilteredDeliveryDtos[DataGrid.SelectedIndex]);
        }

        private void ProviderFilterComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProviderFilterComboBox.SelectedItem != null) UpdateDataGrid();
        }

        private void ClearProviderFilterBtn_OnClick(object sender, RoutedEventArgs e)
        {
            ProviderFilterComboBox.SelectedItem = null;
            UpdateDataGrid();
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