using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
    /// Interaction logic for DeliveryContentsPage.xaml
    /// </summary>
    public partial class DeliveryContentsPage : Page, IActivable
    {
        private readonly IDeliveryContentsService _deliveryContentsService;
        private readonly IConsignmentService _consignmentService;
        private readonly IDeliveryService _deliveryService;
        private readonly IProviderService _providerService;
        private readonly IGoodsService _goodsService;
        private readonly AppSettings _settings;
        private readonly IMapper _mapper;
        private readonly SimpleNavigationService _navigationService;


        public List<DeliveryContentsDTO> DeliveryContentsDtos { get; set; }
        public List<DeliveryContentsDTO> FilteredDeliveryContentsDtos { get; set; }
        public List<ProviderDTO> ProviderDtos { get; set; }
        public List<DeliveryDTO> DeliveryDtos { get; set; }
        public List<GoodsDTO> GoodsDtos { get; set; }

        public DeliveryContentsPage(IDeliveryContentsService deliveryContentsService,
            IConsignmentService consignmentService, IDeliveryService deliveryService, IOptions<AppSettings> settings,
            IMapper mapper, IProviderService providerService, SimpleNavigationService navigationService, IGoodsService goodsService)
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

        private void UpdateDataGrid()
        {
            DeliveryContentsDtos =
                _mapper.Map<List<DeliveryContents>, List<DeliveryContentsDTO>>(_deliveryContentsService.GetAll());
            FilteredDeliveryContentsDtos = DeliveryContentsDtos;

            if (ProviderFilterComboBox.SelectedItem != null)
            {
                ProviderDTO tempProvider = (ProviderDTO) ProviderFilterComboBox.SelectedItem;
                var tempList = FilteredDeliveryContentsDtos.Where(item => item.ProviderTitle == tempProvider.CompanyTitle).ToList();
                FilteredDeliveryContentsDtos = tempList;
            }

            if (DeliveryComboBox.SelectedItem != null)
            {
                DeliveryDTO tempDelivery = (DeliveryDTO) DeliveryComboBox.SelectedItem;
                var tempList = FilteredDeliveryContentsDtos.Where(item => item.OrderDate == tempDelivery.DeliveryDate).ToList();
                FilteredDeliveryContentsDtos = tempList;
            }

            if (GoodComboBox.SelectedItem != null)
            {
                GoodsDTO tempGood = (GoodsDTO) GoodComboBox.SelectedItem;
                var tempList = FilteredDeliveryContentsDtos.Where(item => item.ProductCode == tempGood.ProductCode).ToList();
                FilteredDeliveryContentsDtos = tempList;
            }

            DataGrid.ItemsSource = FilteredDeliveryContentsDtos;
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

        public Task ActivateAsync(object parameter)
        {
            ProviderDtos = _mapper.Map<List<Provider>, List<ProviderDTO>>(_providerService.GetAll());
            ProviderComboBox.ItemsSource = ProviderDtos;
            ProviderFilterComboBox.ItemsSource = ProviderDtos;

            DeliveryDtos = _mapper.Map<List<Delivery>, List<DeliveryDTO>>(_deliveryService.GetAll());
            DeliveryComboBox.ItemsSource = DeliveryDtos;

            GoodsDtos = _mapper.Map<List<Goods>, List<GoodsDTO>>(_goodsService.GetAll());
            GoodComboBox.ItemsSource = GoodsDtos;

            UpdateDataGrid();
            return Task.CompletedTask;
        }

        private async void CreateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;
            Delivery delivery = new Delivery();
            ProviderDTO tempProvider = (ProviderDTO) ProviderComboBox.SelectedItem;
            delivery.Id = DeliveryDtos[^1]?.Id + 1 ?? 1;
            delivery.IdProvider = tempProvider.Id;
            delivery.DeliveryDate = DateTime.Now;
            delivery.DeliveryNumber = delivery.Id.ToString("D12");

            _deliveryService.Create(delivery);

            var result = await _navigationService.ShowDialogAsync<DeliveryOrderWindow>(delivery);

            UpdateDataGrid();

            DeliveryDtos = _mapper.Map<List<Delivery>, List<DeliveryDTO>>(_deliveryService.GetAll());
            DeliveryComboBox.ItemsSource = DeliveryDtos;
        }

        private async void UpdateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            if (FilteredDeliveryContentsDtos[DataGrid.SelectedIndex].ConsignmentNumber != "") return;
            var dc = _deliveryContentsService.GetAll().FirstOrDefault(item => item.Id == FilteredDeliveryContentsDtos[DataGrid.SelectedIndex].Id);
            var result = await _navigationService.ShowDialogAsync<DeliveryOrderUpdateWindow>(dc.IdConsignmentNavigation);
            UpdateDataGrid();
        }

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private async void DataGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            await _navigationService.ShowDialogAsync<DeliveryOrderDetailWindow>(FilteredDeliveryContentsDtos[DataGrid.SelectedIndex].ConsignmentNumber);
        }

        private void ProviderFilterComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProviderFilterComboBox.SelectedItem != null)
            {
                UpdateDataGrid();
            }
        }

        private void DeliveryComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DeliveryComboBox.SelectedItem != null)
            {
                UpdateDataGrid();
            }
        }

        private void GoodComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GoodComboBox.SelectedItem != null)
            {
                UpdateDataGrid();
            }
        }

        private void ClearFilterBtn_OnClick(object sender, RoutedEventArgs e)
        {
            ProviderFilterComboBox.SelectedItem = null;
            DeliveryComboBox.SelectedItem = null;
            GoodComboBox.SelectedItem = null;
            ProviderFilterComboBox.IsEnabled = true;
            DeliveryComboBox.IsEnabled = true;
            GoodComboBox.IsEnabled = true;
            UpdateDataGrid();
        }

        private void ClearProviderFilterBtn_OnClick(object sender, RoutedEventArgs e)
        {
            ProviderFilterComboBox.SelectedItem = null;
            UpdateDataGrid();
        }

        private void ClearDeliveryFilterBtn_OnClick(object sender, RoutedEventArgs e)
        {
            DeliveryComboBox.SelectedItem = null;
            UpdateDataGrid();
        }

        private void ClearGoodFilterBtn_OnClick(object sender, RoutedEventArgs e)
        {
            GoodComboBox.SelectedItem = null;
            UpdateDataGrid();
        }
    }
}