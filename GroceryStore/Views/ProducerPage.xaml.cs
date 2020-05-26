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
    ///     Interaction logic for ProducerPage.xaml
    /// </summary>
    public partial class ProducerPage : Page, IActivable
    {
        private readonly ICountryService _countryService;
        private readonly IMapper _mapper;
        private readonly IProducerService _producerService;
        private readonly AppSettings _settings;

        public ProducerPage(IProducerService producerService, ICountryService countryService,
            IOptions<AppSettings> settings, IMapper mapper)
        {
            _producerService = producerService;
            _countryService = countryService;
            _mapper = mapper;
            _settings = settings.Value;

            InitializeComponent();

            UpdateDataGrid();
        }

        public List<ProducerDTO> ProducerDtos { get; set; }
        public List<CountryDTO> CountryDtos { get; set; }

        public Task ActivateAsync(object parameter)
        {
            CountryDtos = _mapper.Map<List<Country>, List<CountryDTO>>(_countryService.GetAll());
            CountryComboBox.ItemsSource = CountryDtos;
            return Task.CompletedTask;
        }

        private void UpdateDataGrid()
        {
            ProducerDtos = _mapper.Map<List<Producer>, List<ProducerDTO>>(_producerService.GetAll());

            ProducerDtos.Sort(delegate (ProducerDTO x, ProducerDTO y)
            {
                return x.Id.CompareTo(y.Id);
            });

            DataGrid.ItemsSource = ProducerDtos;
        }

        private bool ValidateForm()
        {
            if (!Regex.Match(TitleTextBox.Text, @"^\D{1,50}$").Success)
            {
                MessageBox.Show("Producer title must consist of t least 1 character and not exceed 50 characters!");
                TitleTextBox.Focus();
                return false;
            }

            if (CountryComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select country");
                return false;
            }

            return true;
        }

        private void CreateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;
            var producer = new Producer();
            CountryDTO tempCountry;
            producer.Id = ProducerDtos[^1]?.Id + 1 ?? 1;
            producer.Title = TitleTextBox.Text;
            tempCountry = (CountryDTO) CountryComboBox.SelectedItem;
            producer.IdCountry = tempCountry.Id;

            _producerService.Create(producer);
            UpdateDataGrid();
        }

        private void UpdateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            if (!ValidateForm()) return;
            var producer = new Producer();
            CountryDTO tempCountry;
            producer.Id = ProducerDtos[DataGrid.SelectedIndex].Id;
            producer.Title = TitleTextBox.Text;
            tempCountry = (CountryDTO) CountryComboBox.SelectedItem;
            producer.IdCountry = tempCountry.Id;

            _producerService.Update(producer);
            UpdateDataGrid();
        }

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedIndex != -1)
            {
                var producerDto = ProducerDtos[DataGrid.SelectedIndex];
                TitleTextBox.Text = producerDto.Title;
            }
        }

        private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            _producerService.Delete(ProducerDtos[DataGrid.SelectedIndex].Id);
            UpdateDataGrid();
        }
    }
}