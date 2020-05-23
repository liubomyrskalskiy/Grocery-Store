using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media.Animation;
using AutoMapper;
using GroceryStore.Core.Abstractions.IServices;
using GroceryStore.Core.DTO;
using GroceryStore.Core.Models;
using GroceryStore.Views;
using GroceryStore.Views.LessViews;
using Microsoft.Extensions.Options;

namespace GroceryStore.Windows
{
    public partial class MainWindow : Window
    {
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;
        private readonly SimpleNavigationService _navigationService;
        private readonly AppSettings _settings;
        private readonly Storyboard sb;

        private EmployeeDTO _currentEmployee;

        public MainWindow(SimpleNavigationService navigationService, IOptions<AppSettings> settings,
            IEmployeeService employeeService, IMapper mapper)
        {
            _navigationService = navigationService;
            _employeeService = employeeService;
            _mapper = mapper;
            _settings = settings.Value;


            EmployeeDtos = _mapper.Map<List<Employee>, List<EmployeeDTO>>(_employeeService.GetAll());

            InitializeComponent();

            sb = FindResource("LogInMenuClose") as Storyboard;
            LogOutBtn.Visibility = Visibility.Collapsed;

            HideMenu();
        }

        public List<EmployeeDTO> EmployeeDtos { get; set; }

        private void BtnClose(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private bool ValidateForm()
        {
            if (!Regex.Match(LoginTextBox.Text, @"^\D{6,20}$").Success)
            {
                MessageBox.Show("Login must consist of at least 6 characters and not exceed 20 characters!");
                LoginTextBox.Focus();
                return false;
            }

            if (!Regex.Match(PasswordTextBox.Password, @"^\D{6,20}$").Success)
            {
                MessageBox.Show("Password must consist of at least 6 characters and not exceed 20 characters!");
                LoginTextBox.Focus();
                return false;
            }

            return true;
        }

        private async void SaleBtn_Click(object sender, RoutedEventArgs e)
        {
            var result = await _navigationService.GetPageAsync<SalePage>(_currentEmployee);
            Main.Content = result;
        }

        private async void CategoryBtn_Click(object sender, RoutedEventArgs e)
        {
            var result = await _navigationService.GetPageAsync<CategoryPage>();
            Main.Content = result;
        }

        private async void RoleBtn_Click(object sender, RoutedEventArgs e)
        {
            var result = await _navigationService.GetPageAsync<RolesPage>();
            Main.Content = result;
        }

        private async void ClientBtn_Click(object sender, RoutedEventArgs e)
        {
            var result = await _navigationService.GetPageAsync<ClientPage>();
            Main.Content = result;
        }

        private async void MarketBtn_Click(object sender, RoutedEventArgs e)
        {
            var result = await _navigationService.GetPageAsync<MarketPage>();
            Main.Content = result;
        }

        private async void MarketLessBtn_Click(object sender, RoutedEventArgs e)
        {
            var result = await _navigationService.GetPageAsync<MarketLessPage>();
            Main.Content = result;
        }

        private async void EmployeeBtn_Click(object sender, RoutedEventArgs e)
        {
            var result = await _navigationService.GetPageAsync<EmployeePage>();
            Main.Content = result;
        }

        private async void GoodsInMarketBtn_Click(object sender, RoutedEventArgs e)
        {
            var result = await _navigationService.GetPageAsync<GoodsInMarketPage>(_currentEmployee);
            Main.Content = result;
        }

        private async void GoodsInMarketOwnBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_currentEmployee.RoleTitle.Equals("Адміністратор"))
            {
                var result = await _navigationService.GetPageAsync<GoodsInMarketOwn_AdminPage>(_currentEmployee);
                Main.Content = result;
            }
            else
            {
                var result = await _navigationService.GetPageAsync<GoodsInMarketOwnPage>(_currentEmployee);
                Main.Content = result;
            }
        }

        private async void GoodsInMarketLessBtn_Click(object sender, RoutedEventArgs e)
        {
            var result = await _navigationService.GetPageAsync<GoodsInMarketLessPage>(_currentEmployee);
            Main.Content = result;
        }

        private async void GoodsInMarketOwnLessBtn_Click(object sender, RoutedEventArgs e)
        {
            var result =
                await _navigationService.GetPageAsync<GoodsInMarketOwnLessPage>(_currentEmployee);
            Main.Content = result;
        }

        private async void GoodsOwnBtn_Click(object sender, RoutedEventArgs e)
        {
            var result = await _navigationService.GetPageAsync<GoodsOwnPage>();
            Main.Content = result;
        }

        private async void ProductionBtn_Click(object sender, RoutedEventArgs e)
        {
            var result = await _navigationService.GetPageAsync<ProductionPage>(_currentEmployee);
            Main.Content = result;
        }

        private async void ProductionLessBtn_Click(object sender, RoutedEventArgs e)
        {
            var result = await _navigationService.GetPageAsync<ProductionLessPage>();
            Main.Content = result;
        }

        private async void CityBtn_Click(object sender, RoutedEventArgs e)
        {
            var result = await _navigationService.GetPageAsync<CityPage>();
            Main.Content = result;
        }

        private async void CountryBtn_Click(object sender, RoutedEventArgs e)
        {
            var result = await _navigationService.GetPageAsync<CountryPage>();
            Main.Content = result;
        }

        private async void DeliveryContentsBtn_Click(object sender, RoutedEventArgs e)
        {
            var result = await _navigationService.GetPageAsync<DeliveryContentsPage>();
            Main.Content = result;
        }

        private async void GoodsBtn_Click(object sender, RoutedEventArgs e)
        {
            var result = await _navigationService.GetPageAsync<GoodsPage>();
            Main.Content = result;
        }

        private async void GoodsWriteOffOwnBtn_Click(object sender, RoutedEventArgs e)
        {
            var result = await _navigationService.GetPageAsync<GoodsWriteOffOwnPage>(_currentEmployee);
            Main.Content = result;
        }

        private async void GoodsWriteOffBtn_Click(object sender, RoutedEventArgs e)
        {
            var result = await _navigationService.GetPageAsync<GoodsWriteOffPage>(_currentEmployee);
            Main.Content = result;
        }

        private async void ProducerBtn_Click(object sender, RoutedEventArgs e)
        {
            var result = await _navigationService.GetPageAsync<ProducerPage>();
            Main.Content = result;
        }

        private async void ProviderBtn_Click(object sender, RoutedEventArgs e)
        {
            var result = await _navigationService.GetPageAsync<ProviderPage>();
            Main.Content = result;
        }

        private async void WriteOffReasonBtn_Click(object sender, RoutedEventArgs e)
        {
            var result = await _navigationService.GetPageAsync<WriteOffReasonPage>();
            Main.Content = result;
        }

        private void HideMenu()
        {
            Main.Content = null;
            SaleBtn.Visibility = Visibility.Collapsed;
            StoreManagementMenuOpenBtn.Visibility = Visibility.Collapsed;
            StoreManagementMenuCloseBtn.Visibility = Visibility.Collapsed;
            ProductionBtn.Visibility = Visibility.Collapsed;
            ProductionLessBtn.Visibility = Visibility.Collapsed;
            ProductManagementMenuOpenBtn.Visibility = Visibility.Collapsed;
            ProductManagementMenuCloseBtn.Visibility = Visibility.Collapsed;
            StaffManagementMenuOpenBtn.Visibility = Visibility.Collapsed;
            StaffManagementMenuCloseBtn.Visibility = Visibility.Collapsed;
            OtherManagementMenuOpenBtn.Visibility = Visibility.Collapsed;
            OtherManagementMenuCloseBtn.Visibility = Visibility.Collapsed;

            GoodsInMarketLessBtn.Height = 40;
            GoodsInMarketOwnLessBtn.Height = 40;
            ProductionLessBtn.Height = 40;
            MarketLessBtn.Height = 40;
            GoodsInMarketBtn.Height = 40;
            GoodsInMarketOwnBtn.Height = 40;
            MarketBtn.Height = 40;
            ProductionBtn.Height = 40;

            MarketBtn.IsEnabled = true;
            GoodsBtn.IsEnabled = true;
            DeliveryContentsBtn.IsEnabled = true;
            GoodsWriteOffBtn.IsEnabled = true;
            GoodsWriteOffOwnBtn.IsEnabled = true;
            WriteOffReasonBtn.IsEnabled = true;
            GoodsOwnBtn.IsEnabled = true;
            CategotyBtn.IsEnabled = true;
            GoodsInMarketOwnBtn.IsEnabled = true;
            EmployeeBtn.IsEnabled = true;
            RoleBtn.IsEnabled = true;
            ClientBtn.IsEnabled = true;
        }

        private void LogInBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;
            EmployeeDtos = _mapper.Map<List<Employee>, List<EmployeeDTO>>(_employeeService.GetAll());
            if ((_currentEmployee = EmployeeDtos.FirstOrDefault(item =>
                    item.Login == LoginTextBox.Text && item.Password == PasswordTextBox.Password)) == null)
            {
                MessageBox.Show("Incorrect login or password!");
                LoginTextBox.Focus();
                return;
            }

            LogOutBtn.Visibility = Visibility.Visible;

            NameLabel.Content = _currentEmployee.FullName;
            RoleLabel.Content = _currentEmployee.RoleTitle;
            MarketLabel.Content = "Market: " + _currentEmployee.FullMarketAddress;
            MarketLabel.Visibility = Visibility.Visible;
            LoginTextBox.Text = "";
            PasswordTextBox.Password = "";

            if (_currentEmployee.RoleTitle.Equals("Адміністратор"))
            {
                SaleBtn.Visibility = Visibility.Visible;
                StoreManagementMenuOpenBtn.Visibility = Visibility.Visible;
                StoreManagementMenuCloseBtn.Visibility = Visibility.Visible;
                ProductionBtn.Visibility = Visibility.Visible;
                ProductManagementMenuOpenBtn.Visibility = Visibility.Visible;
                ProductManagementMenuCloseBtn.Visibility = Visibility.Visible;
                StaffManagementMenuOpenBtn.Visibility = Visibility.Visible;
                StaffManagementMenuCloseBtn.Visibility = Visibility.Visible;
                OtherManagementMenuOpenBtn.Visibility = Visibility.Visible;
                OtherManagementMenuCloseBtn.Visibility = Visibility.Visible;
                MarketLabel.Visibility = Visibility.Collapsed;

                GoodsInMarketLessBtn.Height = 0;
                GoodsInMarketOwnLessBtn.Height = 0;
                ProductionLessBtn.Height = 0;
                MarketLessBtn.Height = 0;

                sb.Begin();
            }

            if (_currentEmployee.RoleTitle.Equals("Продавець-касир"))
            {
                SaleBtn.Visibility = Visibility.Visible;
                StoreManagementMenuOpenBtn.Visibility = Visibility.Visible;
                StoreManagementMenuCloseBtn.Visibility = Visibility.Visible;
                MarketBtn.IsEnabled = false;
                MarketLessBtn.Height = 0;
                GoodsInMarketBtn.Height = 0;
                GoodsInMarketOwnBtn.Height = 0;

                sb.Begin();
            }

            if (_currentEmployee.RoleTitle.Equals("Продавець-кухар"))
            {
                StoreManagementMenuOpenBtn.Visibility = Visibility.Visible;
                StoreManagementMenuCloseBtn.Visibility = Visibility.Visible;
                MarketBtn.Height = 0;
                GoodsInMarketBtn.Height = 0;
                GoodsInMarketOwnLessBtn.Height = 0;
                ProductionLessBtn.Height = 0;
                ProductionBtn.Visibility = Visibility.Visible;
                ProductManagementMenuOpenBtn.Visibility = Visibility.Visible;
                ProductManagementMenuCloseBtn.Visibility = Visibility.Visible;
                GoodsBtn.IsEnabled = false;
                DeliveryContentsBtn.IsEnabled = false;
                GoodsWriteOffBtn.IsEnabled = false;
                GoodsWriteOffOwnBtn.IsEnabled = false;
                WriteOffReasonBtn.IsEnabled = false;

                sb.Begin();
            }

            if (_currentEmployee.RoleTitle.Equals("Менеджер по списуванню"))
            {
                StoreManagementMenuOpenBtn.Visibility = Visibility.Visible;
                StoreManagementMenuCloseBtn.Visibility = Visibility.Visible;
                GoodsInMarketBtn.Height = 0;
                GoodsInMarketOwnBtn.Height = 0;
                MarketBtn.IsEnabled = false;
                MarketLessBtn.Height = 0;
                ProductManagementMenuOpenBtn.Visibility = Visibility.Visible;
                ProductManagementMenuCloseBtn.Visibility = Visibility.Visible;
                GoodsBtn.IsEnabled = false;
                GoodsOwnBtn.IsEnabled = false;
                CategotyBtn.IsEnabled = false;
                DeliveryContentsBtn.IsEnabled = false;
                ProductionBtn.Visibility = Visibility.Visible;
                ProductionBtn.Height = 0;

                sb.Begin();
            }

            if (_currentEmployee.RoleTitle.Equals("Менеджер по постачанню"))
            {
                StoreManagementMenuOpenBtn.Visibility = Visibility.Visible;
                StoreManagementMenuCloseBtn.Visibility = Visibility.Visible;
                MarketBtn.Height = 0;
                GoodsInMarketLessBtn.Height = 0;
                GoodsInMarketOwnLessBtn.Height = 0;
                GoodsInMarketOwnBtn.IsEnabled = false;
                ProductManagementMenuOpenBtn.Visibility = Visibility.Visible;
                ProductManagementMenuCloseBtn.Visibility = Visibility.Visible;
                GoodsOwnBtn.IsEnabled = false;
                GoodsWriteOffBtn.IsEnabled = false;
                GoodsWriteOffOwnBtn.IsEnabled = false;
                WriteOffReasonBtn.IsEnabled = false;
                StaffManagementMenuOpenBtn.Visibility = Visibility.Visible;
                StaffManagementMenuCloseBtn.Visibility = Visibility.Visible;
                EmployeeBtn.IsEnabled = false;
                RoleBtn.IsEnabled = false;
                ClientBtn.IsEnabled = false;
                OtherManagementMenuOpenBtn.Visibility = Visibility.Visible;
                OtherManagementMenuCloseBtn.Visibility = Visibility.Visible;

                sb.Begin();
            }
        }

        private void LogOutBtn_OnClick(object sender, RoutedEventArgs e)
        {
            HideMenu();
            NameLabel.Content = "";
            RoleLabel.Content = "";
            MarketLabel.Content = "";
            LogOutBtn.Visibility = Visibility.Collapsed;
        }
    }
}