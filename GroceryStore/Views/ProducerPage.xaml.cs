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
    /// Interaction logic for ProducerPage.xaml
    /// </summary>
    public partial class ProducerPage : Page, IActivable
    {
        private readonly IProducerService _producerService;
        private readonly ICountryService _countryService;
        private readonly AppSettings _settings;
        private readonly IMapper _mapper;

        public List<ProducerDTO> ProducerDtos { get; set; }

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

        private void UpdateDataGrid()
        {
            ProducerDtos = _mapper.Map<List<Producer>, List<ProducerDTO>>(_producerService.GetAll());

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

            if (!Regex.Match(CountryTitleTextBox.Text, @"^\D{1,50}$").Success)
            {
                MessageBox.Show("Country title must consist of at least 1 character and not exceed 50 characters!");
                CountryTitleTextBox.Focus();
                return false;
            }

            return true;
        }

        public Task ActivateAsync(object parameter)
        {
            return Task.CompletedTask;
        }

        private void CreateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;
            Producer producer = new Producer();
            Country tempCountry;
            producer.Id = ProducerDtos[^1]?.Id + 1 ?? 1;
            producer.Title = TitleTextBox.Text;
            if ((tempCountry = _countryService.GetAll()
                    .FirstOrDefault(country => country.Title == CountryTitleTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such country title!");
                return;
            }
            else
                producer.IdCountry = tempCountry.Id;

            _producerService.Create(producer);
            UpdateDataGrid();
        }

        private void UpdateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            if (!ValidateForm()) return;
            Producer producer = new Producer();
            Country tempCountry;
            producer.Id = ProducerDtos[DataGrid.SelectedIndex].Id;
            producer.Title = TitleTextBox.Text;
            if ((tempCountry = _countryService.GetAll()
                    .FirstOrDefault(country => country.Title == CountryTitleTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such country title!");
                return;
            }
            else
                producer.IdCountry = tempCountry.Id;

            _producerService.Update(producer);
            UpdateDataGrid();
        }

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedIndex != -1)
            {
                ProducerDTO producerDto = ProducerDtos[DataGrid.SelectedIndex];
                TitleTextBox.Text = producerDto.Title;
                CountryTitleTextBox.Text = producerDto.CountryTitle;
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