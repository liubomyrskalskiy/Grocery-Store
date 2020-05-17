using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Interaction logic for ConsignmentPage.xaml
    /// </summary>
    public partial class ConsignmentPage : Page, IActivable
    {
        private readonly IConsignmentService _consignmentService;
        private readonly IGoodsService _goodsService;
        private readonly AppSettings _settings;
        private readonly IMapper _mapper;

        public List<ConsignmentDTO> ConsignmentDtos { get; set; }

        public ConsignmentPage(IConsignmentService consignmentService, IGoodsService goodsService,
            IOptions<AppSettings> settings, IMapper mapper)
        {
            _consignmentService = consignmentService;
            _goodsService = goodsService;
            _mapper = mapper;
            _settings = settings.Value;

            InitializeComponent();

            UpdateDataGrid();
        }

        private void UpdateDataGrid()
        {
            ConsignmentDtos = _mapper.Map<List<Consignment>, List<ConsignmentDTO>>(_consignmentService.GetAll());

            DataGrid.ItemsSource = ConsignmentDtos;
        }

        private bool ValidateForm()
        {
            if (!Regex.Match(NumberTextBox.Text, @"^\d{5,20}$").Success)
            {
                MessageBox.Show("Consignment Number must consist of at least 5 digits and not exceed 20 digits!");
                NumberTextBox.Focus();
                return false;
            }

            DateTime dt;
            if (!DateTime.TryParse(DateTextBox.Text, out dt))
            {
                MessageBox.Show("Manufacture Date isn't valid! Check data you've entered!");
                DateTextBox.Focus();
                return false;
            }

            if (!DateTime.TryParse(BestDeforeTextBox.Text, out dt))
            {
                MessageBox.Show("Best Before isn't valid! Check data you've entered!");
                BestDeforeTextBox.Focus();
                return false;
            }

            if (!Regex.Match(AmountTextBox.Text, @"^[0-9]*(?:\,[0-9]*)?$").Success)
            {
                MessageBox.Show("Invalid amount! Check the data you've entered!");
                AmountTextBox.Focus();
                return false;
            }

            if (!Regex.Match(IncomePriceTextBox.Text, @"^[0-9]*(?:\,[0-9]*)?$").Success)
            {
                MessageBox.Show("Invalid incoming price! Check the data you've entered!");
                IncomePriceTextBox.Focus();
                return false;
            }

            if (!Regex.Match(ProductCodeTextBox.Text, @"^\d{5}$").Success)
            {
                MessageBox.Show("Invalid product code! It must contain 5 digits");
                AmountTextBox.Focus();
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
            Consignment consignment = new Consignment();
            Goods tempGoods;
            consignment.Id = ConsignmentDtos[^1]?.Id + 1 ?? 1;
            consignment.ConsignmentNumber = NumberTextBox.Text;
            consignment.ManufactureDate = DateTime.Parse(DateTextBox.Text);
            consignment.BestBefore = DateTime.Parse(BestDeforeTextBox.Text);
            consignment.Amount = Convert.ToDouble(AmountTextBox.Text);
            consignment.IncomePrice = Convert.ToDouble(IncomePriceTextBox.Text);
            if ((tempGoods = _goodsService.GetAll()
                    .FirstOrDefault(good => good.ProductCode == ProductCodeTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such good title!");
                return;
            }
            else
                consignment.IdGoods = tempGoods.Id;

            _consignmentService.Create(consignment);
            UpdateDataGrid();
        }

        private void UpdateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            if (!ValidateForm()) return;
            Goods tempGoods;
            Consignment consignment = new Consignment();
            consignment.Id = ConsignmentDtos[DataGrid.SelectedIndex].Id;
            consignment.ConsignmentNumber = NumberTextBox.Text;
            consignment.ManufactureDate = DateTime.Parse(DateTextBox.Text);
            consignment.BestBefore = DateTime.Parse(BestDeforeTextBox.Text);
            consignment.Amount = Convert.ToDouble(AmountTextBox.Text);
            consignment.IncomePrice = Convert.ToDouble(IncomePriceTextBox.Text);
            if ((tempGoods = _goodsService.GetAll()
                    .FirstOrDefault(good => good.ProductCode == ProductCodeTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such good title!");
                return;
            }
            else
                consignment.IdGoods = tempGoods.Id;

            _consignmentService.Update(consignment);
            UpdateDataGrid();
        }

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedIndex != -1)
            {
                NumberTextBox.Text = ConsignmentDtos[DataGrid.SelectedIndex].ConsignmentNumber;
                DateTextBox.Text = ConsignmentDtos[DataGrid.SelectedIndex].ManufactureDate.ToString();
                BestDeforeTextBox.Text = ConsignmentDtos[DataGrid.SelectedIndex].BestBefore.ToString();
                AmountTextBox.Text = ConsignmentDtos[DataGrid.SelectedIndex].Amount.ToString();
                IncomePriceTextBox.Text = ConsignmentDtos[DataGrid.SelectedIndex].IncomePrice.ToString();
                ProductCodeTextBox.Text = ConsignmentDtos[DataGrid.SelectedIndex].ProductCode;
            }
        }

        private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            _consignmentService.Delete(ConsignmentDtos[DataGrid.SelectedIndex].Id);
            UpdateDataGrid();
        }
    }
}