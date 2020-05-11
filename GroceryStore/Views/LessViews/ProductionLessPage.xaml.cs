using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AutoMapper;
using GroceryStore.Core.Abstractions;
using GroceryStore.Core.Abstractions.IServices;
using GroceryStore.Core.DTO;
using GroceryStore.Core.Models;
using Microsoft.Extensions.Options;

namespace GroceryStore.Views.LessViews
{
    /// <summary>
    /// Interaction logic for ProductionLessPage.xaml
    /// </summary>
    public partial class ProductionLessPage : Page, IActivable
    {
        private readonly IProductionService _productionService;
        private AppSettings _settings;
        private readonly IMapper _mapper;

        private List<ProductionDTO> ProductionDtos { get; set; }
        public ProductionLessPage(IProductionService productionService, IOptions<AppSettings> settings, IMapper mapper)
        {
            _productionService = productionService;
            _mapper = mapper;
            _settings = settings.Value;
            InitializeComponent();

            UpdateDataGrid();
        }

        private void UpdateDataGrid()
        {
            ProductionDtos = _mapper.Map<List<Production>, List<ProductionDTO>>(_productionService.GetAll());

            DataGrid.ItemsSource = ProductionDtos;
        }

        public Task ActivateAsync(object parameter)
        {
            return Task.CompletedTask;
        }

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedIndex != -1)
            {
                ProductCodeTextBox.Text = ProductionDtos[DataGrid.SelectedIndex].ProductCode;
                ProductionCodeTextBox.Text = ProductionDtos[DataGrid.SelectedIndex].ProductionCode;
                AmountTextBox.Text = ProductionDtos[DataGrid.SelectedIndex].Amount.ToString();
                LoginTextBox.Text = ProductionDtos[DataGrid.SelectedIndex].Login;
                TotalCostTextBox.Text = ProductionDtos[DataGrid.SelectedIndex].TotalCost.ToString();
            }
        }
    }
}
