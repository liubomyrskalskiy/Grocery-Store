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
    /// Interaction logic for MarketLessPage.xaml
    /// </summary>
    public partial class MarketLessPage : Page, IActivable
    {
        private readonly IMarketService _marketService;
        private AppSettings _settings;
        private readonly IMapper _mapper;
        public List<MarketDTO> MarketDtos { get; set; }

        public MarketLessPage(IMarketService marketService, IOptions<AppSettings> settings, IMapper mapper)
        {
            _marketService = marketService;
            _settings = settings.Value;
            _mapper = mapper;

            InitializeComponent();

            UpdateDataGrid();
        }

        private void UpdateDataGrid()
        {
            MarketDtos = _mapper.Map<List<Market>, List<MarketDTO>>(_marketService.GetAll());

            DataGrid.ItemsSource = MarketDtos;
        }

        public Task ActivateAsync(object parameter)
        {
            return Task.CompletedTask;
        }

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedIndex != -1)
            {
                AddressTextBox.Text = MarketDtos[DataGrid.SelectedIndex].Address;
                PhoneNumberTextBox.Text = MarketDtos[DataGrid.SelectedIndex].PhoneNumber;
                CityTitleTextBox.Text = MarketDtos[DataGrid.SelectedIndex].CityTitle;
            }
        }
    }
}