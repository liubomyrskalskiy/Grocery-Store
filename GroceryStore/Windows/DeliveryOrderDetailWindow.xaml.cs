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
    /// Interaction logic for DeliveryOrderDetailWindow.xaml
    /// </summary>
    public partial class DeliveryOrderDetailWindow : Window, IActivable
    {
        private IDeliveryShipmentService _deliveryShipmentService;
        private readonly AppSettings _settings;
        private readonly IMapper _mapper;
        private string _currentConsignment;
        public List<DeliveryShipmentDTO> DeliveryShipmentDtos { get; set; }
        public DeliveryOrderDetailWindow(IDeliveryShipmentService deliveryShipmentService, IMapper mapper, IOptions<AppSettings> settings)
        {
            _deliveryShipmentService = deliveryShipmentService;
            _mapper = mapper;
            _settings = settings.Value;
            InitializeComponent();
        }

        private void UpdateDataGrid()
        {
            DeliveryShipmentDtos =
                _mapper.Map<List<DeliveryShipment>, List<DeliveryShipmentDTO>>(_deliveryShipmentService.GetAll());

            DataGrid.ItemsSource = DeliveryShipmentDtos.Where(item => item.ConsignmentNumber == _currentConsignment).ToList();
        }

        public Task ActivateAsync(object parameter)
        {
            _currentConsignment = parameter.ToString();
            UpdateDataGrid();
            return Task.CompletedTask;
        }

        private void BtnClose(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
