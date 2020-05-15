using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for MarketPage.xaml
    /// </summary>
    public partial class MarketPage : Page, IActivable
    {
        private readonly IMarketService _marketService;
        private readonly ICityService _cityService;
        private AppSettings _settings;
        private readonly IMapper _mapper;
        public List<MarketDTO> MarketDtos { get; set; }
        public List<CityDTO> CityDtos { get; set; }

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

        private void UpdateDataGrid()
        {
            MarketDtos = _mapper.Map<List<Market>, List<MarketDTO>>(_marketService.GetAll());

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

        public Task ActivateAsync(object parameter)
        {
            CityDtos = _mapper.Map<List<City>, List<CityDTO>>(_cityService.GetAll());
            CityComboBox.ItemsSource = CityDtos;
            return Task.CompletedTask;
        }

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedIndex != -1)
            {
                AddressTextBox.Text = MarketDtos[DataGrid.SelectedIndex].Address;
                PhoneNumberTextBox.Text = MarketDtos[DataGrid.SelectedIndex].PhoneNumber;
                //CityTitleTextBox.Text = MarketDtos[DataGrid.SelectedIndex].CityTitle;
            }
        }

        private void CreateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;
            Market market = new Market();
            CityDTO tempCity;
            market.Id = MarketDtos[^1]?.Id + 1 ?? 1;
            market.Address = AddressTextBox.Text;
            market.PhoneNumber = PhoneNumberTextBox.Text;
            if(CityComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select city");
                return;
            }
            else
            {
                tempCity = (CityDTO) CityComboBox.SelectedItem;
                market.IdCity = tempCity.Id;
            }

            _marketService.Create(market);
            UpdateDataGrid();
        }

        private void UpdateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            if (!ValidateForm()) return;
            Market market = new Market();
            CityDTO tempCity;
            market.Id = MarketDtos[DataGrid.SelectedIndex].Id;
            market.Address = AddressTextBox.Text;
            market.PhoneNumber = PhoneNumberTextBox.Text;
            if (CityComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select city");
                return;
            }
            else
            {
                tempCity = (CityDTO)CityComboBox.SelectedItem;
                market.IdCity = tempCity.Id;
            }

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