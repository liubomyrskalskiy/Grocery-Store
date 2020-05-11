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
    /// Interaction logic for ClientPage.xaml
    /// </summary>
    public partial class ClientPage : Page, IActivable
    {
        private readonly IClientService _clientService;
        private readonly ICityService _cityService;
        private AppSettings _settings;
        private readonly IMapper _mapper;

        public List<ClientDTO> ClientDtos { get; set; }

        public ClientPage(IClientService clientService, ICityService cityService, IOptions<AppSettings> settings, IMapper mapper)
        {
            _clientService = clientService;
            _cityService = cityService;
            _settings = settings.Value;
            _mapper = mapper;

            InitializeComponent();

            UpdateDataGrid();
        }

        private void UpdateDataGrid()
        {
            ClientDtos = _mapper.Map<List<Client>, List<ClientDTO>>(_clientService.GetAll());

            DataGrid.ItemsSource = ClientDtos;
        }

        private bool ValidateForm()
        {
            if(!Regex.Match(FirstNameTextBox.Text, @"^\D{1,30}$").Success)
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

            if(!Regex.Match(AccountNumberTextBox.Text, @"^\d{12}$").Success)
            {
                MessageBox.Show("Account number must consist of 12 digits!");
                AccountNumberTextBox.Focus();
                return false;
            }

            if (!Regex.Match(CityTitleTextBox.Text, @"^\D{1,50}$").Success)
            {
                MessageBox.Show("City title must consist of at least 1 character and not exceed 50 characters!");
                CityTitleTextBox.Focus();
                return false;
            }
            return true;
        }

        public Task ActivateAsync(object parameter)
        {
            return Task.CompletedTask;
        }

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedIndex != -1)
            {
                FirstNameTextBox.Text = ClientDtos[DataGrid.SelectedIndex].FirstName;
                LastNameTextBox.Text = ClientDtos[DataGrid.SelectedIndex].LastName;
                AddressTextBox.Text = ClientDtos[DataGrid.SelectedIndex].Address;
                PhoneNumberTextBox.Text = ClientDtos[DataGrid.SelectedIndex].PhoneNumber;
                AccountNumberTextBox.Text = ClientDtos[DataGrid.SelectedIndex].AccountNumber;
                CityTitleTextBox.Text = ClientDtos[DataGrid.SelectedIndex].CityTitle;
            }
        }

        private void CreateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;
            Client client = new Client();
            City tempCity;
            client.Id = ClientDtos[^1]?.Id + 1 ?? 1;
            client.FirstName = FirstNameTextBox.Text;
            client.LastName = LastNameTextBox.Text;
            client.Address = AddressTextBox.Text;
            client.Bonuses = 0;
            client.PhoneNumber = PhoneNumberTextBox.Text;
            client.AccountNumber = AccountNumberTextBox.Text;
            if ((tempCity = _cityService.GetAll().FirstOrDefault(city => city.Title == CityTitleTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such city in database!");
                return;
            }

            client.IdCity = tempCity.Id;
            _clientService.Create(client);
            UpdateDataGrid();
        }

        private void UpdateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            if (!ValidateForm()) return;
            Client client = new Client();
            City tempCity;
            client.Id = ClientDtos[DataGrid.SelectedIndex].Id;
            client.FirstName = FirstNameTextBox.Text;
            client.LastName = LastNameTextBox.Text;
            client.Address = AddressTextBox.Text;
            client.Bonuses = ClientDtos[DataGrid.SelectedIndex].Bonuses;
            client.PhoneNumber = PhoneNumberTextBox.Text;
            client.AccountNumber = AccountNumberTextBox.Text;
            if ((tempCity = _cityService.GetAll().FirstOrDefault(city => city.Title == CityTitleTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such city in database!");
                return;
            }

            client.IdCity = tempCity.Id;
            _clientService.Update(client);
            UpdateDataGrid();
        }

        private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            _clientService.Delete(ClientDtos[DataGrid.SelectedIndex].Id);
            UpdateDataGrid();
        }
    }
}
