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
    ///     Interaction logic for MarketLessPage.xaml
    /// </summary>
    public partial class MarketLessPage : Page, IActivable
    {
        private readonly IMapper _mapper;
        private readonly IMarketService _marketService;
        private readonly AppSettings _settings;

        public MarketLessPage(IMarketService marketService, IOptions<AppSettings> settings, IMapper mapper)
        {
            _marketService = marketService;
            _settings = settings.Value;
            _mapper = mapper;

            InitializeComponent();

            UpdateDataGrid();
        }

        public List<MarketDTO> MarketDtos { get; set; }

        public Task ActivateAsync(object parameter)
        {
            return Task.CompletedTask;
        }

        private void UpdateDataGrid()
        {
            MarketDtos = _mapper.Map<List<Market>, List<MarketDTO>>(_marketService.GetAll());

            MarketDtos.Sort(delegate (MarketDTO x, MarketDTO y)
            {
                return x.Id.CompareTo(y.Id);
            });

            DataGrid.ItemsSource = MarketDtos;
        }

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedIndex != -1)
            {
                AddressTextBox.Text = MarketDtos[DataGrid.SelectedIndex].Address;
                PhoneNumberTextBox.Text = MarketDtos[DataGrid.SelectedIndex].PhoneNumber;
                CityTitleTextBox.Text = MarketDtos[DataGrid.SelectedIndex].CityTitle;
            }
        }
    }
}