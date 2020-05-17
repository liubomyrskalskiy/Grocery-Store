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
    /// Interaction logic for DeliveryPage.xaml
    /// </summary>
    public partial class DeliveryPage : Page, IActivable
    {
        private readonly IDeliveryService _deliveryService;
        private readonly IProviderService _providerService;
        private readonly AppSettings _settings;
        private readonly IMapper _mapper;

        public List<DeliveryDTO> DeliveryDtos { get; set; }

        public DeliveryPage(IDeliveryService deliveryService, IProviderService providerService,
            IOptions<AppSettings> settings, IMapper mapper)
        {
            _deliveryService = deliveryService;
            _providerService = providerService;
            _mapper = mapper;
            _settings = settings.Value;

            InitializeComponent();

            UpdateDataGrid();
        }

        private void UpdateDataGrid()
        {
            DeliveryDtos = _mapper.Map<List<Delivery>, List<DeliveryDTO>>(_deliveryService.GetAll());

            DataGrid.ItemsSource = DeliveryDtos;
        }

        private bool ValidateForm()
        {
            if (!Regex.Match(NumberTextBox.Text, @"^\d{5,20}$").Success)
            {
                MessageBox.Show("Delivery Number must consist of at least 5 digits and not exceed 20 digits!");
                NumberTextBox.Focus();
                return false;
            }

            DateTime dt;
            if (!DateTime.TryParse(DateTextBox.Text, out dt))
            {
                MessageBox.Show("Deliver Date isn't valid! Check data you've entered!");
                DateTextBox.Focus();
                return false;
            }

            if (!Regex.Match(ProviderTextBox.Text, @"^\D{1,50}$").Success)
            {
                MessageBox.Show("Provider title must consist of at least 1 character and not exceed 40 characters!");
                ProviderTextBox.Focus();
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
            Delivery delivery = new Delivery();
            Provider tempProvider;
            delivery.Id = DeliveryDtos[^1]?.Id + 1 ?? 1;
            delivery.DeliveryNumber = NumberTextBox.Text;
            delivery.DeliveryDate = DateTime.Parse(DateTextBox.Text);
            if ((tempProvider = _providerService.GetAll()
                    .FirstOrDefault(provider => provider.CompanyTitle == ProviderTextBox.Text.ToString())) == null)
            {
                MessageBox.Show("There is no such provider title!");
                return;
            }
            else
                delivery.IdProvider = tempProvider.Id;

            _deliveryService.Create(delivery);
            UpdateDataGrid();
        }

        private void UpdateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            if (!ValidateForm()) return;
            Delivery delivery = new Delivery();
            Provider tempProvider;
            delivery.Id = DeliveryDtos[DataGrid.SelectedIndex].Id;
            delivery.DeliveryNumber = NumberTextBox.Text;
            delivery.DeliveryDate = DateTime.Parse(DateTextBox.Text);
            if ((tempProvider = _providerService.GetAll()
                    .FirstOrDefault(provider => provider.CompanyTitle == ProviderTextBox.Text.ToString())) == null)
            {
                MessageBox.Show("There is no such provider title!");
                return;
            }
            else
                delivery.IdProvider = tempProvider.Id;

            _deliveryService.Update(delivery);
            UpdateDataGrid();
        }

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedIndex != -1)
            {
                NumberTextBox.Text = DeliveryDtos[DataGrid.SelectedIndex].DeliveryNumber;
                DateTextBox.Text = DeliveryDtos[DataGrid.SelectedIndex].DeliveryDate.ToString();
                ProviderTextBox.Text = DeliveryDtos[DataGrid.SelectedIndex].ProviderTitle;
            }
        }

        private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            _deliveryService.Delete(DeliveryDtos[DataGrid.SelectedIndex].Id);
            UpdateDataGrid();
        }
    }
}