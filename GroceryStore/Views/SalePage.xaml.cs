using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AutoMapper;
using GroceryStore.Core.Abstractions;
using GroceryStore.Core.Abstractions.IServices;
using GroceryStore.Core.DTO;
using GroceryStore.Core.Models;
using GroceryStore.NavigationTransferObjects;
using GroceryStore.Windows;
using Microsoft.Extensions.Options;

namespace GroceryStore.Views
{
    /// <summary>
    ///     Interaction logic for SalePage.xaml
    /// </summary>
    public partial class SalePage : Page, IActivable
    {
        private readonly IClientService _clientService;
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;
        private readonly IMarketService _marketService;
        private readonly SimpleNavigationService _navigationService;
        private readonly ISaleService _saleService;
        private readonly AppSettings _settings;
        private EmployeeDTO _currentEmployee;

        private SaleNTO _saleNto;

        public SalePage(ISaleService saleService, IClientService clientService, IEmployeeService employeeService,
            IOptions<AppSettings> settings, IMapper mapper, SimpleNavigationService navigationService,
            IMarketService marketService)
        {
            _saleService = saleService;
            _clientService = clientService;
            _employeeService = employeeService;
            _settings = settings.Value;
            _mapper = mapper;
            _navigationService = navigationService;
            _marketService = marketService;

            InitializeComponent();
        }

        public List<SaleDTO> SaleDtos { get; set; }
        public List<SaleDTO> FilteredSaleDtos { get; set; }
        public List<MarketDTO> MarketDtos { get; set; }
        public List<EmployeeDTO> EmployeeDtos { get; set; }

        public Task ActivateAsync(object parameter)
        {
            _currentEmployee = (EmployeeDTO) parameter;
            MarketDtos = _mapper.Map<List<Market>, List<MarketDTO>>(_marketService.GetAll());
            MarketFilterComboBox.ItemsSource = MarketDtos;
            EmployeeDtos = _mapper.Map<List<Employee>, List<EmployeeDTO>>(_employeeService.GetAll());
            EmployeeFilterComboBox.ItemsSource = EmployeeDtos;
            if (_currentEmployee.RoleTitle.Equals("Адміністратор"))
            {
                MarketFilterComboBox.SelectedItem = MarketDtos[0];
                FilterGrid.Visibility = Visibility.Visible;
            }
            else
            {
                MarketFilterComboBox.SelectedItem = MarketDtos.FirstOrDefault(item =>
                    item.CityTitle == _currentEmployee.CityTitle && item.Address == _currentEmployee.MarketAddress);
                FilterGrid.Visibility = Visibility.Collapsed;
            }

            UpdateDataGrid();

            return Task.CompletedTask;
        }

        private void UpdateDataGrid()
        {
            SaleDtos = _mapper.Map<List<Sale>, List<SaleDTO>>(_saleService.GetAll());

            SaleDtos.Sort(delegate (SaleDTO x, SaleDTO y)
            {
                return x.Id.CompareTo(y.Id);
            });

            FilteredSaleDtos = SaleDtos;

            if (MarketFilterComboBox.SelectedItem != null)
            {
                var tempMarket = (MarketDTO) MarketFilterComboBox.SelectedItem;
                var tempList = FilteredSaleDtos.Where(item => item.FullMarketAddress == tempMarket.FullAddress)
                    .ToList();
                FilteredSaleDtos = tempList;
            }

            if (DateFromFilterTextBox.Text != "")
            {
                var tempDate = DateTime.Parse(DateFromFilterTextBox.Text);
                var tempList = FilteredSaleDtos
                    .Where(item => DateTime.Compare(item.Date ?? default, tempDate) >= 0).ToList();
                FilteredSaleDtos = tempList;
            }

            if (DateToFilterTextBox.Text != "")
            {
                var tempDate = DateTime.Parse(DateToFilterTextBox.Text);
                var tempList = FilteredSaleDtos
                    .Where(item => DateTime.Compare(item.Date ?? default, tempDate) <= 0).ToList();
                FilteredSaleDtos = tempList;
            }

            if (EmployeeFilterComboBox.SelectedItem != null)
            {
                var tempEmployee = (EmployeeDTO) EmployeeFilterComboBox.SelectedItem;
                var tempList = FilteredSaleDtos.Where(item => item.FullName == tempEmployee.FullName)
                    .ToList();
                FilteredSaleDtos = tempList;
            }

            DataGrid.ItemsSource = FilteredSaleDtos;
        }

        private bool ValidateForm()
        {
            if (!Regex.Match(AccountNumberNumberTextBox.Text, @"^\d{12}$").Success)
            {
                MessageBox.Show("Account number must consist of 12 digits!");
                AccountNumberNumberTextBox.Focus();
                return false;
            }

            return true;
        }

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedIndex != -1)
                AccountNumberNumberTextBox.Text = FilteredSaleDtos[DataGrid.SelectedIndex].AccountNumber;
        }

        private void MarketFilterComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MarketFilterComboBox.SelectedItem != null)
            {
                var tempMarket = (MarketDTO) MarketFilterComboBox.SelectedItem;
                EmployeeFilterComboBox.ItemsSource =
                    EmployeeDtos.Where(item => item.FullMarketAddress == tempMarket.FullAddress).ToList();
                UpdateDataGrid();
            }
        }

        private async void CreateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var sale = new Sale();
            Client tempClient;
            sale.Id = SaleDtos[^1]?.Id + 1 ?? 1;
            sale.CheckNumber = sale.Id.ToString("D16");
            sale.IdEmployee = _currentEmployee.Id;
            sale.Total = 0;
            sale.Date = DateTime.Now;
            _saleNto = new SaleNTO(sale, _currentEmployee);

            if (PhoneCheckBox.IsChecked == true)
            {
                if (AccountNumberNumberTextBox.Text == "")
                    MessageBox.Show("Client phone was not set! Client account set to default");
                else if ((tempClient = _clientService.GetAll()
                             .FirstOrDefault(client => client.PhoneNumber == AccountNumberNumberTextBox.Text)) == null)
                    MessageBox.Show("Phone number must consist of 10 digits! Client account set to default");
                else
                    sale.IdClient = tempClient.Id;
            }
            else
            {
                if (AccountNumberNumberTextBox.Text == "")
                    MessageBox.Show("Client account number was not set! Client account set to default");
                else if ((tempClient = _clientService.GetAll()
                             .FirstOrDefault(client => client.AccountNumber == AccountNumberNumberTextBox.Text)) ==
                         null)
                    MessageBox.Show("Account number must consist of 12 digits! Client account set to default");
                else
                    sale.IdClient = tempClient.Id;
            }

            _saleService.Create(sale);

            var result = await _navigationService.ShowDialogAsync<SaleWindow>(_saleNto);

            UpdateDataGrid();
        }

        private async void DataGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            await _navigationService.ShowDialogAsync<SaleDetailWindow>(FilteredSaleDtos[DataGrid.SelectedIndex]);
        }

        private void ClearDateFromFilterFilterBtn_OnClick(object sender, RoutedEventArgs e)
        {
            DateFromFilterTextBox.Text = "";
            UpdateDataGrid();
        }

        private void SearchDateFromFilterBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DateTime.TryParse(DateFromFilterTextBox.Text, out _))
            {
                UpdateDataGrid();
            }
            else
            {
                MessageBox.Show("Cannot parse date you've entered! Please check data you've entered");
                DateFromFilterTextBox.Focus();
            }
        }

        private void ClearDateToFilterBtn_OnClick(object sender, RoutedEventArgs e)
        {
            DateToFilterTextBox.Text = "";
            UpdateDataGrid();
        }

        private void SearchDateToFilterBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DateTime.TryParse(DateToFilterTextBox.Text, out _))
            {
                UpdateDataGrid();
            }
            else
            {
                MessageBox.Show("Cannot parse date you've entered! Please check data you've entered");
                DateToFilterTextBox.Focus();
            }
        }

        private void AccountNumberNumberTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (PhoneCheckBox.IsChecked == true)
            {
                if (Regex.Match(AccountNumberNumberTextBox.Text, @"^\d{10}$").Success)
                {
                    ClientDTO tempClient;
                    if ((tempClient = _mapper.Map<Client, ClientDTO>(_clientService.GetAll()
                            .FirstOrDefault(item => item.PhoneNumber.Equals(AccountNumberNumberTextBox.Text)))) != null)
                    {
                        FullNameLabel.Content = $"Name: {tempClient.FullName}";
                        PhoneOrAccountLabel.Content = $"Account: {tempClient.AccountNumber}";
                    }
                }
                else
                {
                    FullNameLabel.Content = "";
                    PhoneOrAccountLabel.Content = "";
                }
            }
            else
            {
                if (Regex.Match(AccountNumberNumberTextBox.Text, @"^\d{12}$").Success)
                {
                    ClientDTO tempClient;
                    if ((tempClient = _mapper.Map<Client, ClientDTO>(_clientService.GetAll()
                            .FirstOrDefault(item => item.AccountNumber.Equals(AccountNumberNumberTextBox.Text)))) !=
                        null)
                    {
                        FullNameLabel.Content = $"Name: {tempClient.FullName}";
                        PhoneOrAccountLabel.Content = $"Phone: {tempClient.PhoneNumber}";
                    }
                }
                else
                {
                    FullNameLabel.Content = "";
                    PhoneOrAccountLabel.Content = "";
                }
            }
        }

        private void EmployeeFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EmployeeFilterComboBox.SelectedItem != null) UpdateDataGrid();
        }

        private void ClearEmployeeFilterBtn_OnClick(object sender, RoutedEventArgs e)
        {
            EmployeeFilterComboBox.SelectedItem = null;
            UpdateDataGrid();
        }
    }
}