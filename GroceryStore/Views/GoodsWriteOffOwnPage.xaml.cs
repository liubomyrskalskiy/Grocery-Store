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
    /// Interaction logic for GoodsWriteOffOwnPage.xaml
    /// </summary>
    public partial class GoodsWriteOffOwnPage : Page, IActivable
    {
        private readonly IGoodsWriteOffOwnService _goodsWriteOffOwnService;
        private readonly IEmployeeService _employeeService;
        private readonly IWriteOffReasonService _writeOffReasonService;
        private readonly IProductionService _productionService;
        private readonly IGoodsInMarketOwnService _goodsInMarketOwnService;
        private readonly AppSettings _settings;
        private readonly IMapper _mapper;

        public List<GoodsWriteOffOwnDTO> GoodsWriteOffOwnDtos { get; set; }

        public GoodsWriteOffOwnPage(IGoodsWriteOffOwnService goodsWriteOffOwnService,
            IEmployeeService employeeService,
            IWriteOffReasonService writeOffReasonService,
            IProductionService productionService,
            IGoodsInMarketOwnService goodsInMarketOwnService,
            IOptions<AppSettings> settings, IMapper mapper)
        {
            _goodsWriteOffOwnService = goodsWriteOffOwnService;
            _employeeService = employeeService;
            _writeOffReasonService = writeOffReasonService;
            _productionService = productionService;
            _goodsInMarketOwnService = goodsInMarketOwnService;
            _mapper = mapper;
            _settings = settings.Value;

            InitializeComponent();

            UpdateDataGrid();
        }

        private void UpdateDataGrid()
        {
            GoodsWriteOffOwnDtos =
                _mapper.Map<List<GoodsWriteOffOwn>, List<GoodsWriteOffOwnDTO>>(_goodsWriteOffOwnService.GetAll());

            DataGrid.ItemsSource = GoodsWriteOffOwnDtos;
        }

        private bool ValidateForm()
        {
            if (!Regex.Match(ProductionCodeTextBox.Text, @"^\d{5,10}$").Success)
            {
                MessageBox.Show("Invalid production code! It must contain at least 5 digits and not exceed 10 digits");
                ProductionCodeTextBox.Focus();
                return false;
            }

            if (!Regex.Match(AmountTextBox.Text, @"^[0-9]*(?:\,[0-9]*)?$").Success)
            {
                MessageBox.Show("Invalid amount! Check the data you've entered!");
                AmountTextBox.Focus();
                return false;
            }

            DateTime dt;
            if (!DateTime.TryParse(DateTextBox.Text, out dt))
            {
                MessageBox.Show("Date isn't valid! Check data you've entered!");
                DateTextBox.Focus();
                return false;
            }

            if (!Regex.Match(LoginTextBox.Text, @"^\D{6,20}$").Success)
            {
                MessageBox.Show("Login must consist of at least 6 character and not exceed 20 characters!");
                LoginTextBox.Focus();
                return false;
            }

            if (!Regex.Match(ReasonTextBox.Text, @"^\D{1,150}$").Success)
            {
                MessageBox.Show("Reason must consist of at least 6 character and not exceed 150 characters!");
                ReasonTextBox.Focus();
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
            GoodsWriteOffOwn goodsWriteOffOwn = new GoodsWriteOffOwn();
            Employee tempEmployee;
            WriteOffReason tempWriteOffReason;
            Production tempProduction;
            GoodsInMarketOwn tempGoodsInMarketOwn;
            goodsWriteOffOwn.Id = GoodsWriteOffOwnDtos[^1]?.Id + 1 ?? 1;
            goodsWriteOffOwn.Amount = Convert.ToDouble(AmountTextBox.Text);
            goodsWriteOffOwn.Date = DateTime.Parse(DateTextBox.Text);

            if ((tempEmployee = _employeeService.GetAll()
                    .FirstOrDefault(employee => employee.Login == LoginTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such employee in database!");
                return;
            }
            else
                goodsWriteOffOwn.IdEmployee = tempEmployee.Id;

            if ((tempWriteOffReason = _writeOffReasonService.GetAll()
                    .FirstOrDefault(reason => reason.Description == ReasonTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such write-off reason!");
                return;
            }
            else
                goodsWriteOffOwn.IdWriteOffReason = tempWriteOffReason.Id;

            if ((tempProduction = _productionService.GetAll()
                    .FirstOrDefault(production => production.ProductionCode == ProductionCodeTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such production!");
                return;
            }
            else
                goodsWriteOffOwn.IdProduction = tempProduction.Id;

            if ((tempGoodsInMarketOwn = _goodsInMarketOwnService.GetAll().FirstOrDefault(goodsInMarketOwn =>
                    goodsInMarketOwn.IdProductionNavigation.ProductionCode == ProductionCodeTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such own good in market!");
                return;
            }
            else
                goodsWriteOffOwn.IdGoodsInMarketOwn = tempGoodsInMarketOwn.Id;

            _goodsWriteOffOwnService.Create(goodsWriteOffOwn);
            UpdateDataGrid();
        }

        private void UpdateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            if (!ValidateForm()) return;
            GoodsWriteOffOwn goodsWriteOffOwn = new GoodsWriteOffOwn();
            Employee tempEmployee;
            WriteOffReason tempWriteOffReason;
            Production tempProduction;
            GoodsInMarketOwn tempGoodsInMarketOwn;
            goodsWriteOffOwn.Id = GoodsWriteOffOwnDtos[DataGrid.SelectedIndex].Id;
            goodsWriteOffOwn.Amount = Convert.ToDouble(AmountTextBox.Text.ToString());
            goodsWriteOffOwn.Date = DateTime.Parse(DateTextBox.Text.ToString());

            if ((tempEmployee = _employeeService.GetAll()
                    .FirstOrDefault(employee => employee.Login == LoginTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such employee in database!");
                return;
            }
            else
                goodsWriteOffOwn.IdEmployee = tempEmployee.Id;

            if ((tempWriteOffReason = _writeOffReasonService.GetAll()
                    .FirstOrDefault(reason => reason.Description == ReasonTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such write-off reason!");
                return;
            }
            else
                goodsWriteOffOwn.IdWriteOffReason = tempWriteOffReason.Id;

            if ((tempProduction = _productionService.GetAll()
                    .FirstOrDefault(production => production.ProductionCode == ProductionCodeTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such production!");
                return;
            }
            else
                goodsWriteOffOwn.IdProduction = tempProduction.Id;

            if ((tempGoodsInMarketOwn = _goodsInMarketOwnService.GetAll().FirstOrDefault(goodsInMarketOwn =>
                    goodsInMarketOwn.IdProductionNavigation.ProductionCode == ProductionCodeTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such own good in market!");
                return;
            }
            else
                goodsWriteOffOwn.IdGoodsInMarketOwn = tempGoodsInMarketOwn.Id;

            _goodsWriteOffOwnService.Update(goodsWriteOffOwn);
            UpdateDataGrid();
        }

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedIndex != -1)
            {
                ProductionCodeTextBox.Text = GoodsWriteOffOwnDtos[DataGrid.SelectedIndex].ProductionCode;
                AmountTextBox.Text = GoodsWriteOffOwnDtos[DataGrid.SelectedIndex].Amount.ToString();
                DateTextBox.Text = GoodsWriteOffOwnDtos[DataGrid.SelectedIndex].Date.ToString();
                LoginTextBox.Text = GoodsWriteOffOwnDtos[DataGrid.SelectedIndex].Login;
                ReasonTextBox.Text = GoodsWriteOffOwnDtos[DataGrid.SelectedIndex].Reason;
            }
        }

        private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            _goodsWriteOffOwnService.Delete(GoodsWriteOffOwnDtos[DataGrid.SelectedIndex].Id);
            UpdateDataGrid();
        }
    }
}