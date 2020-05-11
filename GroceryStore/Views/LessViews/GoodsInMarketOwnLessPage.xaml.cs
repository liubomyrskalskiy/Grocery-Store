using System.Collections.Generic;
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

namespace GroceryStore.Views.LessViews
{
    /// <summary>
    /// Interaction logic for GoodsInMarketOwnLessPage.xaml
    /// </summary>
    public partial class GoodsInMarketOwnLessPage : Page, IActivable
    {
        private readonly IGoodsInMarketOwnService _goodsInMarketOwnService;
        private AppSettings _settings;
        private readonly IMapper _mapper;

        public List<GoodsInMarketOwnDTO> GoodsInMarketOwnDtos { get; set; }

        public GoodsInMarketOwnLessPage(IGoodsInMarketOwnService goodsInMarketOwnService, IOptions<AppSettings> settings, IMapper mapper)
        {
            _goodsInMarketOwnService = goodsInMarketOwnService;
            _settings = settings.Value;
            _mapper = mapper;

            InitializeComponent();

            UpdateDataGrid();
        }

        private void UpdateDataGrid()
        {
            GoodsInMarketOwnDtos = _mapper.Map<List<GoodsInMarketOwn>, List<GoodsInMarketOwnDTO>>(_goodsInMarketOwnService.GetAll());

            DataGrid.ItemsSource = GoodsInMarketOwnDtos;
        }

        public Task ActivateAsync(object parameter)
        {
            return Task.CompletedTask;
        }

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedIndex != -1)
            {
                ProductCodeTextBox.Text = GoodsInMarketOwnDtos[DataGrid.SelectedIndex].ProductCode;
                ProductionCodeTextBox.Text = GoodsInMarketOwnDtos[DataGrid.SelectedIndex].ProductionCode;
                AmountTextBox.Text = GoodsInMarketOwnDtos[DataGrid.SelectedIndex].Amount.ToString();
                AddressTextBox.Text = GoodsInMarketOwnDtos[DataGrid.SelectedIndex].Address;
            }
        }
    }
}
