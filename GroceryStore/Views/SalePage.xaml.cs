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
    /// Interaction logic for SalePage.xaml
    /// </summary>
    public partial class SalePage : Page, IActivable
    {
        private readonly ISaleService _saleService;
        private readonly IClientService _clientService;
        private readonly IEmployeeService _employeeService;
        private readonly AppSettings _settings;
        private readonly IMapper _mapper;
        private EmployeeDTO _currentEmployee;
        private readonly SimpleNavigationService _navigationService;
        public List<SaleDTO> SaleDtos { get; set; }

        private SaleNTO _saleNto;

        public SalePage(ISaleService saleService, IClientService clientService, IEmployeeService employeeService,
            IOptions<AppSettings> settings, IMapper mapper, SimpleNavigationService navigationService)
        {
            _saleService = saleService;
            _clientService = clientService;
            _employeeService = employeeService;
            _settings = settings.Value;
            _mapper = mapper;
            _navigationService = navigationService;

            InitializeComponent();

            UpdateDataGrid();
        }

        private void UpdateDataGrid()
        {
            SaleDtos = _mapper.Map<List<Sale>, List<SaleDTO>>(_saleService.GetAll());

            DataGrid.ItemsSource = SaleDtos;
        }

        private bool ValidateForm()
        {
            if (!Regex.Match(AccountNumberNumberTextBox.Text, @"^\d{0,12}$").Success)
            {
                MessageBox.Show("Account number must consist of 12 digits!");
                AccountNumberNumberTextBox.Focus();
                return false;
            }

            return true;
        }

        public Task ActivateAsync(object parameter)
        {
            _currentEmployee = (EmployeeDTO) parameter;

            return Task.CompletedTask;
        }

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedIndex != -1)
            {
                AccountNumberNumberTextBox.Text = SaleDtos[DataGrid.SelectedIndex].AccountNumber;
            }
        }

        private async void CreateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            Sale sale = new Sale();
            Client tempClient;
            sale.Id = SaleDtos[^1]?.Id + 1 ?? 1;
            sale.CheckNumber = sale.Id.ToString("D16");
            sale.IdEmployee = _currentEmployee.Id;
            sale.Total = 0;
            sale.Date = DateTime.Now;
            _saleNto = new SaleNTO(sale, _currentEmployee);

            if (AccountNumberNumberTextBox.Text == "")
                MessageBox.Show("Client account number was not set! Client account set to default");
            else if ((tempClient = _clientService.GetAll()
                         .FirstOrDefault(client => client.AccountNumber == AccountNumberNumberTextBox.Text)) == null)
                MessageBox.Show("There is no such client account! Client account set to default");
            else
                sale.IdClient = tempClient.Id;

            _saleService.Create(sale);

            var result = await _navigationService.ShowDialogAsync<SaleWindow>(_saleNto);

            UpdateDataGrid();
        }

        private void UpdateDataBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var sales = _saleService.GetAll();
            foreach (var sale in sales)
            {
                _saleService.Refresh(sale);
            }

            SaleDtos = null;
            SaleDtos = _mapper.Map<List<Sale>, List<SaleDTO>>(_saleService.GetAll());

            DataGrid.ItemsSource = SaleDtos;
        }

        private void UpdateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            //if (DataGrid.SelectedIndex == -1) return;
            //if (!ValidateForm()) return;
            //Sale sale = new Sale();
            //Client tempClient = new Client();
            //Employee tempEmployee = new Employee();
            //sale.Id = SaleDtos[DataGrid.SelectedIndex].Id;
            //sale.Date = DateTime.Now;
            //sale.CheckNumber = CheckNumberTextBox.Text;
            //sale.Total = SaleDtos[DataGrid.SelectedIndex].Total;
            //if (AccountNumberNumberTextBox.Text == "")
            //    MessageBox.Show("Client account number was not set! Client account set to default");
            //else
            //if ((tempClient = _clientService.GetAll()
            //        .FirstOrDefault(client => client.AccountNumber == AccountNumberNumberTextBox.Text)) == null)
            //    MessageBox.Show("There is no such client account! Client account set to default");
            //else
            //    sale.IdClient = tempClient.Id;
            //if ((tempEmployee = _employeeService.GetAll()
            //        .FirstOrDefault(employee => employee.Login == LoginCodeTextBox.Text)) == null)
            //{
            //    MessageBox.Show("There is no such employee!");
            //    return;
            //}
            //else
            //    sale.IdEmployee = tempEmployee.Id;

            //_saleService.Update(sale);
            //UpdateDataGrid();
        }

        private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            _saleService.Delete(SaleDtos[DataGrid.SelectedIndex].Id);
            UpdateDataGrid();
        }

        private async void DataGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            await _navigationService.ShowDialogAsync<SaleDetailWindow>(SaleDtos[DataGrid.SelectedIndex]);
        }
    }
}