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
    /// Interaction logic for CityPage.xaml
    /// </summary>
    public partial class CityPage : Page, IActivable
    {
        private readonly ICityService _cityService;
        private readonly ICountryService _countryService;
        private readonly AppSettings _settings;
        private readonly IMapper _mapper;

        public List<CityDTO> CityDtos { get; set; }
        public List<CountryDTO> CountryDtos { get; set; }

        public CityPage(ICityService cityService, ICountryService countryService, IOptions<AppSettings> settings,
            IMapper mapper)
        {
            _cityService = cityService;
            _countryService = countryService;
            _mapper = mapper;
            _settings = settings.Value;

            InitializeComponent();

            UpdateDataGrid();
        }

        private void UpdateDataGrid()
        {
            CityDtos = _mapper.Map<List<City>, List<CityDTO>>(_cityService.GetAll());

            DataGrid.ItemsSource = CityDtos;
        }

        private bool ValidateForm()
        {
            if (!Regex.Match(TitleTextBox.Text, @"^\D{1,50}$").Success)
            {
                MessageBox.Show("Title must consist of at least 1 character and not exceed 50 characters!");
                TitleTextBox.Focus();
                return false;
            }

            if (CountryComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select country");
                return false;
            }

            return true;
        }

        public Task ActivateAsync(object parameter)
        {
            CountryDtos = _mapper.Map<List<Country>, List<CountryDTO>>(_countryService.GetAll());
            CountryComboBox.ItemsSource = CountryDtos;
            return Task.CompletedTask;
        }

        private void CreateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;
            City city = new City();
            CountryDTO tempCountry;
            city.Id = CityDtos[^1]?.Id + 1 ?? 1;
            city.Title = TitleTextBox.Text;
            tempCountry = (CountryDTO)CountryComboBox.SelectedItem;
            city.IdCountry = tempCountry.Id;


            _cityService.Create(city);
            UpdateDataGrid();
        }

        private void UpdateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            if (!ValidateForm()) return;
            City city = new City();
            CountryDTO tempCountry;
            city.Id = CityDtos[DataGrid.SelectedIndex].Id;
            city.Title = TitleTextBox.Text;
            tempCountry = (CountryDTO)CountryComboBox.SelectedItem;
            city.IdCountry = tempCountry.Id;

            _cityService.Update(city);
            UpdateDataGrid();
        }

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            TitleTextBox.Text = CityDtos[DataGrid.SelectedIndex].Title;
            CountryComboBox.SelectedItem =
                CountryDtos.FirstOrDefault(item => item.Title == CityDtos[DataGrid.SelectedIndex].CountryTitle);
        }

        private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            _cityService.Delete(CityDtos[DataGrid.SelectedIndex].Id);
            UpdateDataGrid();
        }
    }
}