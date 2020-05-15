using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using AutoMapper;
using GroceryStore.Core.Abstractions;
using GroceryStore.Core.Abstractions.IServices;
using GroceryStore.Core.DTO;
using GroceryStore.Core.Models;
using Microsoft.Extensions.Options;

namespace GroceryStore.Views.LessViews
{
    /// <summary>
    /// Interaction logic for GoodsInMarketLessPage.xaml
    /// </summary>
    public partial class GoodsInMarketLessPage : Page, IActivable
    {
        private readonly IGoodsInMarketService _goodsInMarketService;
        private AppSettings _settings;
        private readonly IMapper _mapper;
        private EmployeeDTO _currentEmployee;

        public List<GoodsInMarketDTO> GoodsInMarketDtos { get; set; }
        public List<GoodsInMarketDTO> GoodsInCurrentMarketDtos { get; set; }

        public GoodsInMarketLessPage(IGoodsInMarketService goodsInMarketService, IOptions<AppSettings> settings,
            IMapper mapper)
        {
            _goodsInMarketService = goodsInMarketService;
            _mapper = mapper;
            _settings = settings.Value;

            InitializeComponent();
        }

        private void UpdateDataGrid()
        {
            GoodsInMarketDtos =
                _mapper.Map<List<GoodsInMarket>, List<GoodsInMarketDTO>>(_goodsInMarketService.GetAll());
            GoodsInCurrentMarketDtos = GoodsInMarketDtos.Where(item => item.Address == _currentEmployee.MarketAddress)
                .ToList();
            DataGrid.ItemsSource = GoodsInCurrentMarketDtos;
        }

        public Task ActivateAsync(object parameter)
        {
            _currentEmployee = (EmployeeDTO)parameter;
            UpdateDataGrid();
            return Task.CompletedTask;
        }

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedIndex != -1)
            {
                TitleTextBox.Text = GoodsInCurrentMarketDtos[DataGrid.SelectedIndex].GoodsTitle;
                ProducerTextBox.Text = GoodsInCurrentMarketDtos[DataGrid.SelectedIndex].ProducerTitle;
                ProductCodeTextBox.Text = GoodsInCurrentMarketDtos[DataGrid.SelectedIndex].ProductCode;
            }
        }
    }
}