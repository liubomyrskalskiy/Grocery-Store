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
    ///     Interaction logic for ProviderPage.xaml
    /// </summary>
    public partial class ProviderPage : Page, IActivable
    {
        private readonly ICityService _cityService;
        private readonly IMapper _mapper;
        private readonly IProviderService _providerService;
        private readonly AppSettings _settings;

        public ProviderPage(IProviderService providerService, ICityService cityService, IOptions<AppSettings> settings,
            IMapper mapper)
        {
            _providerService = providerService;
            _cityService = cityService;
            _mapper = mapper;
            _settings = settings.Value;

            InitializeComponent();
        }

        public List<ProviderDTO> ProviderDtos { get; set; }
        public List<CityDTO> CityDtos { get; set; }

        public Task ActivateAsync(object parameter)
        {
            CityDtos = _mapper.Map<List<City>, List<CityDTO>>(_cityService.GetAll());
            UpdateDataGrid();
            return Task.CompletedTask;
        }

        private void UpdateDataGrid()
        {
            ProviderDtos = _mapper.Map<List<Provider>, List<ProviderDTO>>(_providerService.GetAll());

            ProviderDtos.Sort(delegate (ProviderDTO x, ProviderDTO y)
            {
                return x.Id.CompareTo(y.Id);
            });

            CityComboBox.ItemsSource = CityDtos;
            DataGrid.ItemsSource = ProviderDtos;
        }

        private bool ValidateForm()
        {
            if (!Regex.Match(TitleTextBox.Text, @"^\D{1,50}$").Success)
            {
                MessageBox.Show("Title must consist of at least 1 character and not exceed 50 characters!");
                TitleTextBox.Focus();
                return false;
            }

            if (!Regex.Match(ContactTextBox.Text, @"^\D{1,50}$").Success)
            {
                MessageBox.Show("Contact person must consist of at least 1 character and not exceed 50 characters!");
                ContactTextBox.Focus();
                return false;
            }

            if (!Regex.Match(PhoneTextBox.Text, @"^\d{10}$").Success)
            {
                MessageBox.Show("Phone number must consist of 10 digits!");
                PhoneTextBox.Focus();
                return false;
            }

            if (!Regex.Match(AddressTextBox.Text, @"^(Вул\.\s\D{1,40}\,\s\d{1,3})$").Success)
            {
                MessageBox.Show("Address must consist of at least 1 character and not exceed 50 characters!");
                AddressTextBox.Focus();
                return false;
            }

            return true;
        }

        private void CreateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;
            var provider = new Provider();
            CityDTO tempCity;
            provider.Id = ProviderDtos[^1]?.Id + 1 ?? 1;
            provider.CompanyTitle = TitleTextBox.Text;
            provider.ContactPerson = ContactTextBox.Text;
            provider.PhoneNumber = PhoneTextBox.Text;
            provider.Address = AddressTextBox.Text;
            if (CityComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select city");
                return;
            }

            tempCity = (CityDTO) CityComboBox.SelectedItem;
            provider.IdCity = tempCity.Id;

            _providerService.Create(provider);
            UpdateDataGrid();
        }

        private void UpdateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            if (!ValidateForm()) return;
            var provider = new Provider();
            CityDTO tempCity;
            provider.Id = ProviderDtos[DataGrid.SelectedIndex].Id;
            provider.CompanyTitle = TitleTextBox.Text;
            provider.ContactPerson = ContactTextBox.Text;
            provider.PhoneNumber = PhoneTextBox.Text;
            provider.Address = AddressTextBox.Text;
            if (CityComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select city");
                return;
            }

            tempCity = (CityDTO) CityComboBox.SelectedItem;
            provider.IdCity = tempCity.Id;

            _providerService.Update(provider);
            UpdateDataGrid();
        }

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedIndex != -1)
            {
                TitleTextBox.Text = ProviderDtos[DataGrid.SelectedIndex].CompanyTitle;
                ContactTextBox.Text = ProviderDtos[DataGrid.SelectedIndex].ContactPerson;
                PhoneTextBox.Text = ProviderDtos[DataGrid.SelectedIndex].PhoneNumber;
                AddressTextBox.Text = ProviderDtos[DataGrid.SelectedIndex].Address;
            }
        }

        private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            _providerService.Delete(ProviderDtos[DataGrid.SelectedIndex].Id);
            UpdateDataGrid();
        }
    }
}