using System.Collections.Generic;
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
    ///     Interaction logic for CountryPage.xaml
    /// </summary>
    public partial class CountryPage : Page, IActivable
    {
        private readonly ICountryService _countryService;
        private readonly IMapper _mapper;
        private readonly AppSettings _settings;

        public CountryPage(ICountryService countryService, IOptions<AppSettings> settings, IMapper mapper)
        {
            _countryService = countryService;
            _mapper = mapper;
            _settings = settings.Value;

            InitializeComponent();

            UpdateDataGrid();
        }

        public List<CountryDTO> CountryDtos { get; set; }

        public Task ActivateAsync(object parameter)
        {
            return Task.CompletedTask;
        }

        private void UpdateDataGrid()
        {
            CountryDtos = _mapper.Map<List<Country>, List<CountryDTO>>(_countryService.GetAll());

            CountryDtos.Sort(delegate (CountryDTO x, CountryDTO y)
            {
                return x.Id.CompareTo(y.Id);
            });

            DataGrid.ItemsSource = CountryDtos;
        }

        private bool ValidateForm()
        {
            if (!Regex.Match(TitleTextBox.Text, @"^\D{1,50}$").Success)
            {
                MessageBox.Show("Country title must consist of at least 1 character and not exceed 50 characters!");
                TitleTextBox.Focus();
                return false;
            }

            return true;
        }

        private void CreateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;
            var country = new Country
            {
                Id = CountryDtos[^1]?.Id + 1 ?? 1,
                Title = TitleTextBox.Text
            };

            _countryService.Create(country);
            UpdateDataGrid();
        }

        private void UpdateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex != -1)
            {
                if (!ValidateForm()) return;
                var country = new Country
                {
                    Id = CountryDtos[DataGrid.SelectedIndex].Id,
                    Title = TitleTextBox.Text
                };

                _countryService.Update(country);
                UpdateDataGrid();
            }
        }

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedIndex != -1) TitleTextBox.Text = CountryDtos[DataGrid.SelectedIndex].Title;
        }

        private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            _countryService.Delete(CountryDtos[DataGrid.SelectedIndex].Id);
            UpdateDataGrid();
        }
    }
}