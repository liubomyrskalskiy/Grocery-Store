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
    /// Interaction logic for EmployeePage.xaml
    /// </summary>
    public partial class EmployeePage : Page, IActivable
    {
        private readonly IEmployeeService _employeeService;
        private readonly ICityService _cityService;
        private readonly IRoleService _roleService;
        private readonly IMarketService _marketService;
        private AppSettings _settings;
        private readonly IMapper _mapper;

        public List<EmployeeDTO> EmployeeDtos { get; set; }
        public EmployeePage(IEmployeeService employeeService, ICityService cityService, IRoleService roleService, IMarketService marketService, IOptions<AppSettings> settings, IMapper mapper)
        {
            _employeeService = employeeService;
            _cityService = cityService;
            _roleService = roleService;
            _marketService = marketService;
            _settings = settings.Value;
            _mapper = mapper;

            InitializeComponent();

            UpdateDataGrid();
        }

        private void UpdateDataGrid()
        {
            EmployeeDtos = _mapper.Map<List<Employee>, List<EmployeeDTO>>(_employeeService.GetAll());

            DataGrid.ItemsSource = EmployeeDtos;
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

            if (!Regex.Match(PhoneNumberTextBox.Text, @"^\d{10}$").Success)
            {
                MessageBox.Show("Phone number must consist of 10 digits!");
                PhoneNumberTextBox.Focus();
                return false;
            }

            if (!Regex.Match(ExperienceTextBox.Text, @"^[0-9]+$").Success)
            {
                MessageBox.Show("Invalid work experience number! It must contain only digits");
                ExperienceTextBox.Focus();
                return false;
            }

            if (!Regex.Match(AddressTextBox.Text, @"^(Вул\.\s\D{1,40}\,\s\d{1,3})$").Success)
            {
                MessageBox.Show("Address must consist of at least 1 character and not exceed 50 characters!");
                AddressTextBox.Focus();
                return false;
            }

            if (!Regex.Match(RoleTextBox.Text, @"^\D{1,25}$").Success)
            {
                MessageBox.Show("Role title must consist of at least 1 character and not exceed 25 characters!");
                RoleTextBox.Focus();
                return false;
            }

            if (!Regex.Match(MarketTextBox.Text, @"^(Вул\.\s\D{1,40}\,\s\d{1,3})$").Success)
            {
                MessageBox.Show("Market address must consist of at least 1 character and not exceed 50 characters!");
                MarketTextBox.Focus();
                return false;
            }

            if(!Regex.Match(CityTitleTextBox.Text, @"^\D{1,50}$").Success)
            {
                MessageBox.Show("City title must consist of at least 1 character and not exceed 50 characters!");
                CityTitleTextBox.Focus();
                return false;
            }

            if (!Regex.Match(LoginTextBox.Text, @"^\D{6,20}$").Success)
            {
                MessageBox.Show("Login must consist of at least 6 character and not exceed 20 characters!");
                LoginTextBox.Focus();
                return false;
            }

            if (!Regex.Match(PasswordTextBox.Text, @"^\D{6,20}$").Success)
            {
                MessageBox.Show("Login must consist of at least 6 character and not exceed 20 characters!");
                PasswordTextBox.Focus();
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
                FirstNameTextBox.Text   = EmployeeDtos[DataGrid.SelectedIndex].FirstName;
                LastNameTextBox.Text    = EmployeeDtos[DataGrid.SelectedIndex].LastName;
                PhoneNumberTextBox.Text = EmployeeDtos[DataGrid.SelectedIndex].PhoneNumber;
                ExperienceTextBox.Text  = EmployeeDtos[DataGrid.SelectedIndex].WorkExperience.ToString();
                AddressTextBox.Text     = EmployeeDtos[DataGrid.SelectedIndex].Address;
                RoleTextBox.Text        = EmployeeDtos[DataGrid.SelectedIndex].RoleTitle;
                MarketTextBox.Text      = EmployeeDtos[DataGrid.SelectedIndex].MarketAddress;
                CityTitleTextBox.Text   = EmployeeDtos[DataGrid.SelectedIndex].CityTitle;
                LoginTextBox.Text       = EmployeeDtos[DataGrid.SelectedIndex].Login;
                PasswordTextBox.Text    = EmployeeDtos[DataGrid.SelectedIndex].Password;
            }
        }

        private void CreateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;
            Employee employee = new Employee();
            Role tempRole = new Role();
            City tempCity = new City();
            Market tempMarket = new Market();
            employee.Id = EmployeeDtos[^1]?.Id + 1 ?? 1;
            employee.FirstName = FirstNameTextBox.Text;
            employee.LastName = LastNameTextBox.Text;
            employee.PhoneNumber = PhoneNumberTextBox.Text;
            employee.WorkExperience = Convert.ToInt32(ExperienceTextBox.Text);
            employee.Address = AddressTextBox.Text;
            employee.Login = LoginTextBox.Text;
            employee.Password = PasswordTextBox.Text;
            if ((tempRole = _roleService.GetAll().FirstOrDefault(role => role.Title == RoleTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such role in database!");
                return;
            }
            else
                employee.IdRole = tempRole.Id;

            if ((tempMarket = _marketService.GetAll().FirstOrDefault(market => market.Address == MarketTextBox.Text)) ==
                null)
            {
                MessageBox.Show("There is no market on such address in database!");
                return;
            }
            else
                employee.IdMarket = tempMarket.Id;

            if ((tempCity = _cityService.GetAll().FirstOrDefault(city => city.Title == CityTitleTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such city in database!");
                return;
            }
            else
                employee.IdCity = tempCity.Id;

            _employeeService.Create(employee);
            UpdateDataGrid();
        }

        private void UpdateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            if (!ValidateForm()) return;
            Employee employee = new Employee();
            Role tempRole = new Role();
            City tempCity = new City();
            Market tempMarket = new Market();
            employee.Id = EmployeeDtos[DataGrid.SelectedIndex].Id;
            employee.FirstName = FirstNameTextBox.Text;
            employee.LastName = LastNameTextBox.Text;
            employee.PhoneNumber = PhoneNumberTextBox.Text;
            employee.WorkExperience = Convert.ToInt32(ExperienceTextBox.Text);
            employee.Address = AddressTextBox.Text;
            employee.Login = LoginTextBox.Text;
            employee.Password = PasswordTextBox.Text;
            if ((tempRole = _roleService.GetAll().FirstOrDefault(role => role.Title == RoleTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such role in database!");
                return;
            }
            else
                employee.IdRole = tempRole.Id;

            if ((tempMarket = _marketService.GetAll().FirstOrDefault(market => market.Address == MarketTextBox.Text)) ==
                null)
            {
                MessageBox.Show("There is no market on such address in database!");
                return;
            }
            else
                employee.IdMarket = tempMarket.Id;

            if ((tempCity = _cityService.GetAll().FirstOrDefault(city => city.Title == CityTitleTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such city in database!");
                return;
            }
            else
                employee.IdCity = tempCity.Id;

            _employeeService.Update(employee);
            UpdateDataGrid();
        }

        private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            _employeeService.Delete(EmployeeDtos[DataGrid.SelectedIndex].Id);
            UpdateDataGrid();
        }
    }
}
