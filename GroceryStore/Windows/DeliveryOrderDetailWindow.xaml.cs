using System.Collections.Generic;
using System.Linq;
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
    ///     Interaction logic for DeliveryOrderDetailWindow.xaml
    /// </summary>
    public partial class DeliveryOrderDetailWindow : Window, IActivable
    {
        private readonly IDeliveryShipmentService _deliveryShipmentService;
        private readonly IMapper _mapper;
        private readonly AppSettings _settings;
        private string _currentConsignment;

        public DeliveryOrderDetailWindow(IDeliveryShipmentService deliveryShipmentService, IMapper mapper,
            IOptions<AppSettings> settings)
        {
            _deliveryShipmentService = deliveryShipmentService;
            _mapper = mapper;
            _settings = settings.Value;
            InitializeComponent();
        }

        public List<DeliveryShipmentDTO> DeliveryShipmentDtos { get; set; }

        public Task ActivateAsync(object parameter)
        {
            _currentConsignment = parameter.ToString();
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

            DataGrid.ItemsSource = DeliveryShipmentDtos.Where(item => item.ConsignmentNumber == _currentConsignment)
                .ToList();
        }

        private void BtnClose(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}