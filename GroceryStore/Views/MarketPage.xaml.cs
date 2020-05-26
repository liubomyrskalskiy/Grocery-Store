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

namespace GroceryStore.Views
{
    /// <summary>
    ///     Interaction logic for MarketPage.xaml
    /// </summary>
    public partial class MarketPage : Page, IActivable
    {
        private readonly ICityService _cityService;
        private readonly IMapper _mapper;
        private readonly IMarketService _marketService;
        private readonly AppSettings _settings;

        public MarketPage(IMarketService marketService, ICityService cityService, IOptions<AppSettings> settings,
            IMapper mapper)
        {
            _marketService = marketService;
            _cityService = cityService;
            _settings = settings.Value;
            _mapper = mapper;

            InitializeComponent();

            UpdateDataGrid();
        }

        public List<MarketDTO> MarketDtos { get; set; }
        public List<CityDTO> CityDtos { get; set; }

        public Task ActivateAsync(object parameter)
        {
            CityDtos = _mapper.Map<List<City>, List<CityDTO>>(_cityService.GetAll());
            CityComboBox.ItemsSource = CityDtos;
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

        private bool ValidateForm()
        {
            if (!Regex.Match(AddressTextBox.Text, @"^(Вул\.\s\D{1,40}\,\s\d{1,3})$").Success)
            {
                MessageBox.Show("Address must consist of at least 10 character and not exceed 50 characters!");
                AddressTextBox.Focus();
                return false;
            }

            if (!Regex.Match(PhoneNumberTextBox.Text, @"^\d{10}$").Success)
            {
                MessageBox.Show("Phone number must consist of 10 digits!");
                PhoneNumberTextBox.Focus();
                return false;
            }

            return true;
        }

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedIndex != -1)
            {
                AddressTextBox.Text = MarketDtos[DataGrid.SelectedIndex].Address;
                PhoneNumberTextBox.Text = MarketDtos[DataGrid.SelectedIndex].PhoneNumber;
            }
        }

        private void CreateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;
            var market = new Market();
            CityDTO tempCity;
            market.Id = MarketDtos[^1]?.Id + 1 ?? 1;
            market.Address = AddressTextBox.Text;
            market.PhoneNumber = PhoneNumberTextBox.Text;
            if (CityComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select city");
                return;
            }

            tempCity = (CityDTO) CityComboBox.SelectedItem;
            market.IdCity = tempCity.Id;

            _marketService.Create(market);
            UpdateDataGrid();
        }

        private void UpdateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            if (!ValidateForm()) return;
            var market = new Market();
            CityDTO tempCity;
            market.Id = MarketDtos[DataGrid.SelectedIndex].Id;
            market.Address = AddressTextBox.Text;
            market.PhoneNumber = PhoneNumberTextBox.Text;
            if (CityComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select city");
                return;
            }

            tempCity = (CityDTO) CityComboBox.SelectedItem;
            market.IdCity = tempCity.Id;

            _marketService.Update(market);
            UpdateDataGrid();
        }

        private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            _marketService.Delete(MarketDtos[DataGrid.SelectedIndex].Id);
            UpdateDataGrid();
        }
    }
}