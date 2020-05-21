using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Interaction logic for DeliveryDetailWindow.xaml
    /// </summary>
    public partial class DeliveryDetailWindow : Window,IActivable
    {
        private readonly IDeliveryContentsService _deliveryContentsService;
        private readonly AppSettings _settings;
        private readonly IMapper _mapper;
        private readonly SimpleNavigationService _navigationService;

        private DeliveryDTO _currentDelivery;

        public List<DeliveryContentsDTO> DeliveryContentsDtos { get; set; }
        public List<DeliveryContentsDTO> FilteredDeliveryContentsDtos { get; set; }

        public DeliveryDetailWindow(IDeliveryContentsService deliveryContentsService, IMapper mapper, SimpleNavigationService navigationService, IOptions<AppSettings> settings)
        {
            _deliveryContentsService = deliveryContentsService;
            _mapper = mapper;
            _navigationService = navigationService;
            _settings = settings.Value;
            InitializeComponent();
        }

        private void UpdateDataGrid()
        {
            DeliveryContentsDtos =
                _mapper.Map<List<DeliveryContents>, List<DeliveryContentsDTO>>(_deliveryContentsService.GetAll());
            FilteredDeliveryContentsDtos = DeliveryContentsDtos
                .Where(item => item.DeliveryNumber == _currentDelivery.DeliveryNumber).ToList();
            DataGrid.ItemsSource = FilteredDeliveryContentsDtos;
        }

        private void BtnClose(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public Task ActivateAsync(object parameter)
        {
            _currentDelivery = (DeliveryDTO) parameter;
            DeliveryLabel.Content = "Order number: "+ _currentDelivery.DeliveryNumber;
            DateLabel.Content = "Order Date: "+_currentDelivery.DeliveryDate;
            TotalLabel.Content = _currentDelivery.StringTotal;
            UpdateDataGrid();
            return Task.CompletedTask;
        }

        private async void DataGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            if (FilteredDeliveryContentsDtos[DataGrid.SelectedIndex].ConsignmentNumber != "")
            {
                await _navigationService.ShowDialogAsync<DeliveryOrderDetailWindow>(FilteredDeliveryContentsDtos[DataGrid.SelectedIndex].ConsignmentNumber);
            }
            else
            {
                var dc = _deliveryContentsService.GetAll().FirstOrDefault(item =>
                    item.Id == FilteredDeliveryContentsDtos[DataGrid.SelectedIndex].Id);
                var result =
                    await _navigationService.ShowDialogAsync<DeliveryOrderUpdateWindow>(dc.IdConsignmentNavigation);
                UpdateDataGrid();
            }
        }
    }
}
