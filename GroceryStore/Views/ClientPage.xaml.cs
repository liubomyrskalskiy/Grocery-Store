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
    ///     Interaction logic for ClientPage.xaml
    /// </summary>
    public partial class ClientPage : Page, IActivable
    {
        private readonly ICityService _cityService;
        private readonly IClientService _clientService;
        private readonly IMapper _mapper;
        private readonly AppSettings _settings;

        public ClientPage(IClientService clientService, ICityService cityService, IOptions<AppSettings> settings,
            IMapper mapper)
        {
            _clientService = clientService;
            _cityService = cityService;
            _settings = settings.Value;
            _mapper = mapper;

            InitializeComponent();
        }

        public List<ClientDTO> ClientDtos { get; set; }
        public List<ClientDTO> FilteredClientDtos { get; set; }
        public List<CityDTO> CityDtos { get; set; }

        public Task ActivateAsync(object parameter)
        {
            CityDtos = _mapper.Map<List<City>, List<CityDTO>>(_cityService.GetAll());
            CityComboBox.ItemsSource = CityDtos;
            CityFilterComboBox.ItemsSource = CityDtos;
            UpdateDataGrid();

            return Task.CompletedTask;
        }

        private void UpdateDataGrid()
        {
            ClientDtos = _mapper.Map<List<Client>, List<ClientDTO>>(_clientService.GetAll());

            ClientDtos.Sort(delegate (ClientDTO x, ClientDTO y)
            {
                return x.Id.CompareTo(y.Id);
            });

            FilteredClientDtos = ClientDtos;

            if (Regex.Match(SurnameFilterTextBox.Text, @"^\D{1,30}$").Success)
            {
                var tempList = FilteredClientDtos.Where(item => item.LastName.Contains(SurnameFilterTextBox.Text))
                    .ToList();
                FilteredClientDtos = tempList;
            }

            if (Regex.Match(PhoneFilterTextBox.Text, @"^\d{4,10}$").Success)
            {
                var tempList = FilteredClientDtos.Where(item => item.PhoneNumber.Contains(PhoneFilterTextBox.Text))
                    .ToList();
                FilteredClientDtos = tempList;
            }

            if (CityFilterComboBox.SelectedItem != null)
            {
                var tempCity = (CityDTO) CityFilterComboBox.SelectedItem;
                var tempList = FilteredClientDtos.Where(item => item.CityTitle == tempCity.Title).ToList();
                FilteredClientDtos = tempList;
            }

            DataGrid.ItemsSource = FilteredClientDtos;
        }

        private bool ValidateForm()
        {
            if (!Regex.Match(FirstNameTextBox.Text, @"^\D{1,30}$").Success)
            {
                MessageBox.Show("First name must consist of at least 1 character and not exceed 30 characters!");
                FirstNameTextBox.Focus();
                return false;
            }

            if (!Regex.Match(LastNameTextBox.Text, @"^\D{1,30}$").Success)
            {
                MessageBox.Show("Last name must consist of at least 1 character and not exceed 30 characters!");
                LastNameTextBox.Focus();
                return false;
            }

            if (!Regex.Match(AddressTextBox.Text, @"^(Вул\.\s\D{1,40}\,\s\d{1,3})$").Success)
            {
                MessageBox.Show("Address must consist of at least 1 character and not exceed 50 characters!");
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
                FirstNameTextBox.Text = FilteredClientDtos[DataGrid.SelectedIndex].FirstName;
                LastNameTextBox.Text = FilteredClientDtos[DataGrid.SelectedIndex].LastName;
                AddressTextBox.Text = FilteredClientDtos[DataGrid.SelectedIndex].Address;
                PhoneNumberTextBox.Text = FilteredClientDtos[DataGrid.SelectedIndex].PhoneNumber;
                CityComboBox.SelectedItem =
                    CityDtos.FirstOrDefault(item => item.Title == FilteredClientDtos[DataGrid.SelectedIndex].CityTitle);
            }
        }

        private void CreateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;
            var client = new Client();
            CityDTO tempCity;
            client.Id = ClientDtos[^1]?.Id + 1 ?? 1;
            client.FirstName = FirstNameTextBox.Text;
            client.LastName = LastNameTextBox.Text;
            client.Address = AddressTextBox.Text;
            client.Bonuses = 0;
            client.PhoneNumber = PhoneNumberTextBox.Text;
            client.AccountNumber = client.Id.ToString("D12");
            if (CityComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select city");
                return;
            }

            tempCity = (CityDTO) CityComboBox.SelectedItem;
            client.IdCity = tempCity.Id;

            _clientService.Create(client);
            UpdateDataGrid();
        }

        private void UpdateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            if (!ValidateForm()) return;
            var client = new Client();
            CityDTO tempCity;
            client.Id = FilteredClientDtos[DataGrid.SelectedIndex].Id;
            client.FirstName = FirstNameTextBox.Text;
            client.LastName = LastNameTextBox.Text;
            client.Address = AddressTextBox.Text;
            client.Bonuses = FilteredClientDtos[DataGrid.SelectedIndex].Bonuses;
            client.PhoneNumber = PhoneNumberTextBox.Text;
            client.AccountNumber = FilteredClientDtos[DataGrid.SelectedIndex].AccountNumber;
            if (CityComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select city");
                return;
            }

            tempCity = (CityDTO) CityComboBox.SelectedItem;
            client.IdCity = tempCity.Id;

            _clientService.Update(client);
            UpdateDataGrid();
        }

        private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            _clientService.Delete(FilteredClientDtos[DataGrid.SelectedIndex].Id);
            UpdateDataGrid();
        }

        private void CityFilterComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CityFilterComboBox.SelectedItem != null) UpdateDataGrid();
        }

        private void ClearFilterBtn_OnClick(object sender, RoutedEventArgs e)
        {
            CityFilterComboBox.SelectedItem = null;
            UpdateDataGrid();
        }

        private void SurnameFilterTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!SurnameFilterTextBox.Text.Equals(""))
            {
                PhoneFilterTextBox.Text = "";
                PhoneFilterTextBox.IsEnabled = false;
            }
            else
            {
                PhoneFilterTextBox.IsEnabled = true;
            }
        }

        private void ClearSurnameFilterBtn_OnClick(object sender, RoutedEventArgs e)
        {
            SurnameFilterTextBox.Text = "";
            UpdateDataGrid();
        }

        private void SearchSurnameBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (Regex.Match(SurnameFilterTextBox.Text, @"^\D{1,30}$").Success)
            {
                UpdateDataGrid();
            }
            else
            {
                MessageBox.Show("Last name must consist of at least 1 character and not exceed 30 characters!");
                SurnameFilterTextBox.Focus();
            }
        }

        private void PhoneFilterTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!PhoneFilterTextBox.Text.Equals(""))
            {
                SurnameFilterTextBox.Text = "";
                SurnameFilterTextBox.IsEnabled = false;
            }
            else
            {
                SurnameFilterTextBox.IsEnabled = true;
            }
        }

        private void ClearPhoneFilterBtn_OnClick(object sender, RoutedEventArgs e)
        {
            PhoneFilterTextBox.Text = "";
            UpdateDataGrid();
        }

        private void SearchPhoneBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (Regex.Match(PhoneFilterTextBox.Text, @"^\d{4,10}$").Success)
            {
                UpdateDataGrid();
            }
            else
            {
                MessageBox.Show(
                    "To search employee by phone, it must consist of at least 4 digits and not exceed 10 digits");
                PhoneFilterTextBox.Focus();
            }
        }
    }
}