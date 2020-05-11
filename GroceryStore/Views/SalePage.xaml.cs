using System;
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
    /// Interaction logic for SalePage.xaml
    /// </summary>
    public partial class SalePage : Page, IActivable
    {
        private readonly ISaleService _saleService;
        private readonly IClientService _clientService;
        private readonly IEmployeeService _employeeService;
        private AppSettings _settings;
        private readonly IMapper _mapper;
        public List<SaleDTO> SaleDtos { get; set; }

        public SalePage(ISaleService saleService, IClientService clientService, IEmployeeService employeeService, IOptions<AppSettings> settings, IMapper mapper)
        {
            _saleService = saleService;
            _clientService = clientService;
            _employeeService = employeeService;
            _settings = settings.Value;
            _mapper = mapper;

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
            if (!Regex.Match(CheckNumberTextBox.Text, @"^\d{16}$").Success)
            {
                MessageBox.Show("Invalid check number! It must contain 16 digits");
                CheckNumberTextBox.Focus();
                return false;
            }

            if (!Regex.Match(AccountNumberNumberTextBox.Text, @"^\d{0,12}$").Success)
            {
                MessageBox.Show("Account number must consist of 12 digits!");
                AccountNumberNumberTextBox.Focus();
                return false;
            }

            if (!Regex.Match(LoginCodeTextBox.Text, @"^\D{6,20}$").Success)
            {
                MessageBox.Show("Login must consist of at least 6 character and not exceed 20 characters!");
                LoginCodeTextBox.Focus();
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
                CheckNumberTextBox.Text = SaleDtos[DataGrid.SelectedIndex].CheckNumber;
                AccountNumberNumberTextBox.Text = SaleDtos[DataGrid.SelectedIndex].AccountNumber;
                LoginCodeTextBox.Text = SaleDtos[DataGrid.SelectedIndex].Login;
            }
        }

        private void CreateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;
            Sale sale = new Sale();
            Client tempClient;
            Employee tempEmployee;
            sale.Id = SaleDtos[^1]?.Id + 1 ?? 1;
            sale.Date = DateTime.Now;
            sale.CheckNumber = CheckNumberTextBox.Text;
            if (AccountNumberNumberTextBox.Text == "")
                MessageBox.Show("Client account number was not set! Client account set to default");
            else
                if ((tempClient = _clientService.GetAll()
                       .FirstOrDefault(client => client.AccountNumber == AccountNumberNumberTextBox.Text)) == null)
                MessageBox.Show("There is no such client account! Client account set to default");
            else
                sale.IdClient = tempClient.Id;
            if ((tempEmployee = _employeeService.GetAll()
                    .FirstOrDefault(employee => employee.Login == LoginCodeTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such employee!");
                return;
            }
            else
                sale.IdEmployee = tempEmployee.Id;

            _saleService.Create(sale);
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
            if (DataGrid.SelectedIndex == -1) return;
            if (!ValidateForm()) return;
            Sale sale = new Sale();
            Client tempClient = new Client();
            Employee tempEmployee = new Employee();
            sale.Id = SaleDtos[DataGrid.SelectedIndex].Id;
            sale.Date = DateTime.Now;
            sale.CheckNumber = CheckNumberTextBox.Text;
            sale.Total = SaleDtos[DataGrid.SelectedIndex].Total;
            if (AccountNumberNumberTextBox.Text == "")
                MessageBox.Show("Client account number was not set! Client account set to default");
            else
            if ((tempClient = _clientService.GetAll()
                    .FirstOrDefault(client => client.AccountNumber == AccountNumberNumberTextBox.Text)) == null)
                MessageBox.Show("There is no such client account! Client account set to default");
            else
                sale.IdClient = tempClient.Id;
            if ((tempEmployee = _employeeService.GetAll()
                    .FirstOrDefault(employee => employee.Login == LoginCodeTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such employee!");
                return;
            }
            else
                sale.IdEmployee = tempEmployee.Id;

            _saleService.Update(sale);
            UpdateDataGrid();
        }

        private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            _saleService.Delete(SaleDtos[DataGrid.SelectedIndex].Id);
            UpdateDataGrid();
        }
    }
}
