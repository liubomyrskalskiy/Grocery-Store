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
    ///     Interaction logic for EmployeePage.xaml
    /// </summary>
    public partial class EmployeePage : Page, IActivable
    {
        private readonly ICityService _cityService;
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;
        private readonly IMarketService _marketService;
        private readonly IRoleService _roleService;
        private readonly AppSettings _settings;

        public EmployeePage(IEmployeeService employeeService, ICityService cityService, IRoleService roleService,
            IMarketService marketService, IOptions<AppSettings> settings, IMapper mapper)
        {
            _employeeService = employeeService;
            _cityService = cityService;
            _roleService = roleService;
            _marketService = marketService;
            _settings = settings.Value;
            _mapper = mapper;

            InitializeComponent();
        }

        public List<EmployeeDTO> EmployeeDtos { get; set; }
        public List<EmployeeDTO> FilteredEmployeeDtos { get; set; }
        public List<RoleDTO> RoleDtos { get; set; }
        public List<MarketDTO> MarketDtos { get; set; }
        public List<CityDTO> CityDtos { get; set; }

        public Task ActivateAsync(object parameter)
        {
            RoleDtos = _mapper.Map<List<Role>, List<RoleDTO>>(_roleService.GetAll());
            MarketDtos = _mapper.Map<List<Market>, List<MarketDTO>>(_marketService.GetAll());
            CityDtos = _mapper.Map<List<City>, List<CityDTO>>(_cityService.GetAll());

            MarketFilterComboBox.ItemsSource = MarketDtos;
            RoleFilterComboBox.ItemsSource = RoleDtos;
            UpdateDataGrid();

            return Task.CompletedTask;
        }

        private void UpdateDataGrid()
        {
            EmployeeDtos = _mapper.Map<List<Employee>, List<EmployeeDTO>>(_employeeService.GetAll());

            EmployeeDtos.Sort(delegate (EmployeeDTO x, EmployeeDTO y)
            {
                return x.Id.CompareTo(y.Id);
            });

            RoleComboBox.ItemsSource = RoleDtos;
            MarketComboBox.ItemsSource = MarketDtos;
            CityComboBox.ItemsSource = CityDtos;

            FilteredEmployeeDtos = EmployeeDtos;

            if (Regex.Match(SurnameFilterTextBox.Text, @"^\D{1,30}$").Success)
            {
                var tempList = FilteredEmployeeDtos.Where(item => item.LastName.Contains(SurnameFilterTextBox.Text))
                    .ToList();
                FilteredEmployeeDtos = tempList;
            }

            if (Regex.Match(PhoneFilterTextBox.Text, @"^\d{4,10}$").Success)
            {
                var tempList = FilteredEmployeeDtos.Where(item => item.PhoneNumber.Contains(PhoneFilterTextBox.Text))
                    .ToList();
                FilteredEmployeeDtos = tempList;
            }

            if (MarketFilterComboBox.SelectedItem != null)
            {
                var tempMarket = (MarketDTO) MarketFilterComboBox.SelectedItem;
                var tempList = FilteredEmployeeDtos.Where(item => item.FullMarketAddress == tempMarket.FullAddress)
                    .ToList();
                FilteredEmployeeDtos = tempList;
            }

            if (RoleFilterComboBox.SelectedItem != null)
            {
                var tempRole = (RoleDTO) RoleFilterComboBox.SelectedItem;
                var tempList = FilteredEmployeeDtos.Where(item => item.RoleTitle == tempRole.Title).ToList();
                FilteredEmployeeDtos = tempList;
            }

            DataGrid.ItemsSource = FilteredEmployeeDtos;
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

            if (RoleComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select role");
                return false;
            }

            if (MarketComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select market");
                return false;
            }

            if (CityComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select city");
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

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedIndex != -1)
            {
                FirstNameTextBox.Text = FilteredEmployeeDtos[DataGrid.SelectedIndex].FirstName;
                LastNameTextBox.Text = FilteredEmployeeDtos[DataGrid.SelectedIndex].LastName;
                PhoneNumberTextBox.Text = FilteredEmployeeDtos[DataGrid.SelectedIndex].PhoneNumber;
                ExperienceTextBox.Text = FilteredEmployeeDtos[DataGrid.SelectedIndex].WorkExperience.ToString();
                AddressTextBox.Text = FilteredEmployeeDtos[DataGrid.SelectedIndex].Address;
                LoginTextBox.Text = FilteredEmployeeDtos[DataGrid.SelectedIndex].Login;
                PasswordTextBox.Text = FilteredEmployeeDtos[DataGrid.SelectedIndex].Password;
                RoleComboBox.SelectedItem =
                    RoleDtos.FirstOrDefault(
                        item => item.Title == FilteredEmployeeDtos[DataGrid.SelectedIndex].RoleTitle);
                MarketComboBox.SelectedItem = MarketDtos.FirstOrDefault(item =>
                    item.FullAddress == FilteredEmployeeDtos[DataGrid.SelectedIndex].FullMarketAddress);
                CityComboBox.SelectedItem =
                    CityDtos.FirstOrDefault(
                        item => item.Title == FilteredEmployeeDtos[DataGrid.SelectedIndex].CityTitle);
            }
        }

        private void CreateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;
            var employee = new Employee();
            RoleDTO tempRole;
            CityDTO tempCity;
            MarketDTO tempMarket;
            employee.Id = EmployeeDtos[^1]?.Id + 1 ?? 1;
            employee.FirstName = FirstNameTextBox.Text;
            employee.LastName = LastNameTextBox.Text;
            employee.PhoneNumber = PhoneNumberTextBox.Text;
            employee.WorkExperience = Convert.ToInt32(ExperienceTextBox.Text);
            employee.Address = AddressTextBox.Text;
            if (_employeeService.GetAll().FirstOrDefault(item => item.Login.Equals(LoginTextBox.Text)) != null)
            {
                MessageBox.Show("This login is already captured!");
                return;
            }

            employee.Login = LoginTextBox.Text;

            employee.Password = PasswordTextBox.Text;
            tempRole = (RoleDTO) RoleComboBox.SelectedItem;
            employee.IdRole = tempRole.Id;
            tempMarket = (MarketDTO) MarketComboBox.SelectedItem;
            employee.IdMarket = tempMarket.Id;
            tempCity = (CityDTO) CityComboBox.SelectedItem;
            employee.IdCity = tempCity.Id;

            _employeeService.Create(employee);
            UpdateDataGrid();
        }

        private void UpdateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            if (!ValidateForm()) return;
            var employee = new Employee();
            RoleDTO tempRole;
            CityDTO tempCity;
            MarketDTO tempMarket;
            employee.Id = FilteredEmployeeDtos[DataGrid.SelectedIndex].Id;
            employee.FirstName = FirstNameTextBox.Text;
            employee.LastName = LastNameTextBox.Text;
            employee.PhoneNumber = PhoneNumberTextBox.Text;
            employee.WorkExperience = Convert.ToInt32(ExperienceTextBox.Text);
            employee.Address = AddressTextBox.Text;
            if (_employeeService.GetAll().FirstOrDefault(item => item.Login.Equals(LoginTextBox.Text)) != null)
            {
                MessageBox.Show("This login is already captured!");
                return;
            }

            employee.Login = LoginTextBox.Text;

            employee.Password = PasswordTextBox.Text;
            tempRole = (RoleDTO) RoleComboBox.SelectedItem;
            employee.IdRole = tempRole.Id;
            tempMarket = (MarketDTO) MarketComboBox.SelectedItem;
            employee.IdMarket = tempMarket.Id;
            tempCity = (CityDTO) CityComboBox.SelectedItem;
            employee.IdCity = tempCity.Id;

            _employeeService.Update(employee);
            UpdateDataGrid();
        }

        private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            _employeeService.Delete(FilteredEmployeeDtos[DataGrid.SelectedIndex].Id);
            UpdateDataGrid();
        }

        private void ClearFilterBtn_OnClick(object sender, RoutedEventArgs e)
        {
            MarketFilterComboBox.SelectedItem = null;
            UpdateDataGrid();
        }

        private void MarketFilterComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MarketFilterComboBox.SelectedItem != null) UpdateDataGrid();
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

        private void ClearPhoneFilterBtn_OnClick(object sender, RoutedEventArgs e)
        {
            PhoneFilterTextBox.Text = "";
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

        private void RoleFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RoleFilterComboBox.SelectedItem != null) UpdateDataGrid();
        }

        private void ClearRoleFilterBtn_OnClick(object sender, RoutedEventArgs e)
        {
            RoleFilterComboBox.SelectedItem = null;
            UpdateDataGrid();
        }
    }
}