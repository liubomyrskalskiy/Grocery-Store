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

namespace GroceryStore.Views.LessViews
{
    /// <summary>
    ///     Interaction logic for ProductionLessPage.xaml
    /// </summary>
    public partial class ProductionLessPage : Page, IActivable
    {
        private readonly IMapper _mapper;
        private readonly IProductionService _productionService;
        private readonly AppSettings _settings;

        public ProductionLessPage(IProductionService productionService, IOptions<AppSettings> settings, IMapper mapper)
        {
            _productionService = productionService;
            _mapper = mapper;
            _settings = settings.Value;
            InitializeComponent();

            UpdateDataGrid();
        }

        private List<ProductionDTO> ProductionDtos { get; set; }
        private List<ProductionDTO> FilteredProductionDtos { get; set; }

        public Task ActivateAsync(object parameter)
        {
            return Task.CompletedTask;
        }

        private void UpdateDataGrid()
        {
            ProductionDtos = _mapper.Map<List<Production>, List<ProductionDTO>>(_productionService.GetAll());

            ProductionDtos.Sort(delegate (ProductionDTO x, ProductionDTO y)
            {
                return x.Id.CompareTo(y.Id);
            });

            FilteredProductionDtos = ProductionDtos;

            if (Regex.Match(TitleFilterTextBox.Text, @"^\D{1,20}$").Success)
            {
                var tempList = FilteredProductionDtos.Where(item => item.Title.Contains(TitleFilterTextBox.Text))
                    .ToList();
                FilteredProductionDtos = tempList;
            }

            if (DateFromFilterTextBox.Text != "")
            {
                var tempDate = DateTime.Parse(DateFromFilterTextBox.Text);
                var tempList = FilteredProductionDtos
                    .Where(item => DateTime.Compare(item.ManufactureDate ?? default, tempDate) >= 0).ToList();
                FilteredProductionDtos = tempList;
            }

            if (DateToFilterTextBox.Text != "")
            {
                var tempDate = DateTime.Parse(DateToFilterTextBox.Text);
                var tempList = FilteredProductionDtos
                    .Where(item => DateTime.Compare(item.ManufactureDate ?? default, tempDate) <= 0).ToList();
                FilteredProductionDtos = tempList;
            }

            if (CategoryFilterComboBox.SelectedItem != null)
            {
                var tempCategoty = (CategoryDTO) CategoryFilterComboBox.SelectedItem;
                var tempList = FilteredProductionDtos.Where(item => item.Category == tempCategoty.Title).ToList();
                FilteredProductionDtos = tempList;
            }

            DataGrid.ItemsSource = FilteredProductionDtos;
        }

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedIndex != -1)
            {
                ProductCodeTextBox.Text = FilteredProductionDtos[DataGrid.SelectedIndex].ProductCode;
                ProductionCodeTextBox.Text = FilteredProductionDtos[DataGrid.SelectedIndex].ProductionCode;
                AmountTextBox.Text = FilteredProductionDtos[DataGrid.SelectedIndex].Amount.ToString();
                LoginTextBox.Text = FilteredProductionDtos[DataGrid.SelectedIndex].Login;
                TotalCostTextBox.Text = FilteredProductionDtos[DataGrid.SelectedIndex].TotalCost.ToString();
            }
        }

        private void ClearTitleFilterBtn_OnClick(object sender, RoutedEventArgs e)
        {
            TitleFilterTextBox.Text = "";
            UpdateDataGrid();
        }

        private void SearchTitleBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (Regex.Match(TitleFilterTextBox.Text, @"^\D{1,20}$").Success)
            {
                UpdateDataGrid();
            }
            else
            {
                MessageBox.Show("Title must consist of at least 1 character and not exceed 20 characters!");
                TitleFilterTextBox.Focus();
            }
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

        private void CategoryFilterComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CategoryFilterComboBox.SelectedItem != null) UpdateDataGrid();
        }

        private void ClearCategoryFilterBtn_OnClick(object sender, RoutedEventArgs e)
        {
            CategoryFilterComboBox.SelectedItem = null;
            UpdateDataGrid();
        }
    }
}