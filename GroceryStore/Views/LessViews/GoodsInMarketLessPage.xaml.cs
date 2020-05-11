using System.Collections.Generic;
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

        public List<GoodsInMarketDTO> GoodsInMarketDtos { get; set; }

        public GoodsInMarketLessPage(IGoodsInMarketService goodsInMarketService, IOptions<AppSettings> settings, IMapper mapper)
        {
            _goodsInMarketService = goodsInMarketService;
            _mapper = mapper;
            _settings = settings.Value;

            InitializeComponent();

            UpdateDataGrid();
        }

        private void UpdateDataGrid()
        {
            GoodsInMarketDtos = _mapper.Map<List<GoodsInMarket>, List<GoodsInMarketDTO>>(_goodsInMarketService.GetAll());

            DataGrid.ItemsSource = GoodsInMarketDtos;
        }

        public Task ActivateAsync(object parameter)
        {
            return Task.CompletedTask;
        }

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedIndex != -1)
            {
                ProductCodeTextBox.Text = GoodsInMarketDtos[DataGrid.SelectedIndex].ProductCode;
                AmountTextBox.Text = GoodsInMarketDtos[DataGrid.SelectedIndex].Amount.ToString();
                AddressTextBox.Text = GoodsInMarketDtos[DataGrid.SelectedIndex].Address;
            }
        }
    }
}
