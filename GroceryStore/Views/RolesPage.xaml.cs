using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using AutoMapper;
using GroceryStore.Core.Abstractions;
using GroceryStore.Core.Abstractions.IServices;
using GroceryStore.Core.DTO;
using GroceryStore.Core.Mapping;
using GroceryStore.Core.Models;
using Microsoft.Extensions.Options;

namespace GroceryStore.Views
{
    /// <summary>
    /// Interaction logic for RolesPage.xaml
    /// </summary>
    public partial class RolesPage : Page, IActivable
    {
        private readonly IRoleService _roleService;
        private AppSettings _settings;
        private readonly IMapper _mapper;

        public List<RoleDTO> RoleDtos { get; set; }

        public RolesPage(IRoleService roleService, IOptions<AppSettings> settings, IMapper mapper)
        {
            _roleService = roleService;
            _mapper = mapper;
            _settings = settings.Value;

            InitializeComponent();

            UpdateDataGrid();
        }

        private void UpdateDataGrid()
        {
            RoleDtos = _mapper.Map<List<Role>, List<RoleDTO>>(_roleService.GetAll());

            DataGrid.ItemsSource = RoleDtos;
        }

        private bool ValidateForm()
        {
            if (!Regex.Match(TitleTextBox.Text, @"^\D{1,25}$").Success)
            {
                MessageBox.Show("Title must consist of at least 1 character and nod exceed 25 characters!");
                TitleTextBox.Focus();
                return false;
            }

            if (!Regex.Match(SalaryTextBox.Text, @"^[0-9]*(?:\,[0-9]*)?$").Success)
            {
                MessageBox.Show("Invalid salary! Check the data you've entered!");
                SalaryTextBox.Focus();
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
                TitleTextBox.Text = RoleDtos[DataGrid.SelectedIndex].Title;
                SalaryTextBox.Text = RoleDtos[DataGrid.SelectedIndex].Salary.ToString();
            }
        }

        private void CreateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;
            Role role = new Role();
            role.Id = RoleDtos[^1]?.Id + 1 ?? 1;
            role.Title = TitleTextBox.Text;
            role.Salary = Convert.ToInt32(SalaryTextBox.Text);
            _roleService.Create(role);
            UpdateDataGrid();
        }

        private void UpdateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            if (!ValidateForm()) return;
            Role role = new Role();
            role.Id = RoleDtos[DataGrid.SelectedIndex].Id;
            role.Title = TitleTextBox.Text;
            role.Salary = Convert.ToInt32(SalaryTextBox.Text);
            _roleService.Update(role);
            UpdateDataGrid();
        }

        private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if(DataGrid.SelectedIndex == -1) return;
            _roleService.Delete(RoleDtos[DataGrid.SelectedIndex].Id);
            UpdateDataGrid();
        }

        
    }
}
