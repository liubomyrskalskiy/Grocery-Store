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
    ///     Interaction logic for GoodsWriteOffOwnPage.xaml
    /// </summary>
    public partial class GoodsWriteOffOwnPage : Page, IActivable
    {
        private readonly IGoodsInMarketOwnService _goodsInMarketOwnService;
        private readonly IGoodsOwnService _goodsOwnService;
        private readonly IGoodsWriteOffOwnService _goodsWriteOffOwnService;
        private readonly IMapper _mapper;
        private readonly AppSettings _settings;
        private readonly IWriteOffReasonService _writeOffReasonService;
        private EmployeeDTO _currentEmployee;

        public GoodsWriteOffOwnPage(IGoodsWriteOffOwnService goodsWriteOffOwnService,
            IWriteOffReasonService writeOffReasonService,
            IGoodsInMarketOwnService goodsInMarketOwnService,
            IOptions<AppSettings> settings, IMapper mapper, IGoodsOwnService goodsOwnService)
        {
            _goodsWriteOffOwnService = goodsWriteOffOwnService;
            _writeOffReasonService = writeOffReasonService;
            _goodsInMarketOwnService = goodsInMarketOwnService;
            _mapper = mapper;
            _goodsOwnService = goodsOwnService;
            _settings = settings.Value;

            InitializeComponent();

            GoodsInMarketComboBox.IsEnabled = false;
        }

        public List<GoodsWriteOffOwnDTO> GoodsWriteOffOwnDtos { get; set; }
        public List<GoodsWriteOffOwnDTO> FilteredGoodsWriteOffOwnDtos { get; set; }
        public List<WriteOffReasonDTO> WriteOffReasonDtos { get; set; }
        public List<GoodsInMarketOwnDTO> GoodsInMarketOwnDtos { get; set; }

        public Task ActivateAsync(object parameter)
        {
            _currentEmployee = (EmployeeDTO) parameter;
            WriteOffReasonDtos =
                _mapper.Map<List<WriteOffReason>, List<WriteOffReasonDTO>>(_writeOffReasonService.GetAll());
            ReasonComboBox.ItemsSource = WriteOffReasonDtos;
            UpdateDataGrid();
            return Task.CompletedTask;
        }

        private void UpdateDataGrid()
        {
            GoodsWriteOffOwnDtos =
                _mapper.Map<List<GoodsWriteOffOwn>, List<GoodsWriteOffOwnDTO>>(_goodsWriteOffOwnService.GetAll());

            GoodsWriteOffOwnDtos.Sort(delegate (GoodsWriteOffOwnDTO x, GoodsWriteOffOwnDTO y)
            {
                return x.Id.CompareTo(y.Id);
            });

            FilteredGoodsWriteOffOwnDtos = GoodsWriteOffOwnDtos;

            if (Regex.Match(TitleFilterTextBox.Text, @"^\D{1,20}$").Success)
            {
                var tempList = FilteredGoodsWriteOffOwnDtos
                    .Where(item => item.GoodTitle.Contains(TitleFilterTextBox.Text)).ToList();
                FilteredGoodsWriteOffOwnDtos = tempList;
            }

            if (DateFromFilterTextBox.Text != "")
            {
                var tempDate = DateTime.Parse(DateFromFilterTextBox.Text);
                var tempList = FilteredGoodsWriteOffOwnDtos
                    .Where(item => DateTime.Compare(item.Date ?? default, tempDate) >= 0).ToList();
                FilteredGoodsWriteOffOwnDtos = tempList;
            }

            if (DateToFilterTextBox.Text != "")
            {
                var tempDate = DateTime.Parse(DateToFilterTextBox.Text);
                var tempList = FilteredGoodsWriteOffOwnDtos
                    .Where(item => DateTime.Compare(item.Date ?? default, tempDate) <= 0).ToList();
                FilteredGoodsWriteOffOwnDtos = tempList;
            }

            DataGrid.ItemsSource = FilteredGoodsWriteOffOwnDtos;
        }

        private bool ValidateForm()
        {
            if (!Regex.Match(ProductCodeTextBox.Text, @"^\d{5}$").Success)
            {
                MessageBox.Show("Invalid product code! It must contain 5 digits");
                ProductCodeTextBox.Focus();
                return false;
            }

            if (GoodsInMarketComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select manufacture date");
                return false;
            }

            var tempProduction = (GoodsInMarketOwnDTO)GoodsInMarketComboBox.SelectedItem;
            if (!Regex.Match(AmountTextBox.Text, @"^[0-9]*(?:\,[0-9]*)?$").Success || Convert.ToDouble(AmountTextBox.Text) > tempProduction.DoubleAmount)
            {
                MessageBox.Show("Invalid amount! Check the data you've entered! Or you're trying to write off more than it is in stock!");
                AmountTextBox.Focus();
                return false;
            }

            if (ReasonComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select reason");
                return false;
            }

            return true;
        }

        private void CreateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;
            var goodsWriteOffOwn = new GoodsWriteOffOwn
            {
                Id = GoodsWriteOffOwnDtos[^1]?.Id + 1 ?? 1,
                Amount = Convert.ToDouble(AmountTextBox.Text),
                Date = DateTime.Now,
                IdEmployee = _currentEmployee.Id
            };
            var tempGimo = (GoodsInMarketOwnDTO) GoodsInMarketComboBox.SelectedItem;
            goodsWriteOffOwn.IdGoodsInMarketOwn = tempGimo.Id;
            var gimo = _goodsInMarketOwnService.GetId(tempGimo.Id);
            goodsWriteOffOwn.IdProduction = gimo.IdProduction;
            var reason = (WriteOffReasonDTO) ReasonComboBox.SelectedItem;
            goodsWriteOffOwn.IdWriteOffReason = reason.Id;

            _goodsWriteOffOwnService.Create(goodsWriteOffOwn);

            var goodInMarketOwn = _goodsInMarketOwnService.GetId(goodsWriteOffOwn.IdGoodsInMarketOwn ?? default);
            _goodsInMarketOwnService.Refresh(goodInMarketOwn);

            UpdateDataGrid();
        }

        private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            _goodsWriteOffOwnService.Delete(FilteredGoodsWriteOffOwnDtos[DataGrid.SelectedIndex].Id);
            UpdateDataGrid();
        }

        private void ProductCodeTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (Regex.Match(ProductCodeTextBox.Text, @"^\d{5}$").Success)
            {
                GoodsOwnDTO tempGoodsOwnDto;

                if ((tempGoodsOwnDto = _mapper.Map<GoodsOwn, GoodsOwnDTO>(_goodsOwnService.GetAll()
                        .FirstOrDefault(item => item.ProductCode == ProductCodeTextBox.Text))) != null)
                {
                    GoodTitleLabel.Content = "Good: " + tempGoodsOwnDto.Title;
                    CategoryLabel.Content = "Category: " + tempGoodsOwnDto.Category;
                    WeightLabel.Content = "Unit weight: " + tempGoodsOwnDto.Weight;
                    PriceLabel.Content = "Price: " + $"{tempGoodsOwnDto.Price,0:C2}";

                    if ((GoodsInMarketOwnDtos = _mapper.Map<List<GoodsInMarketOwn>, List<GoodsInMarketOwnDTO>>(
                            _goodsInMarketOwnService.GetAll()
                                .Where(item =>
                                    item.IdProductionNavigation.IdGoodsOwn == tempGoodsOwnDto.Id &&
                                    item.IdMarketNavigation.Address == _currentEmployee.MarketAddress)
                                .ToList())).Count > 0)
                    {
                        GoodsInMarketComboBox.ItemsSource = GoodsInMarketOwnDtos;
                        GoodsInMarketComboBox.IsEnabled = true;
                    }
                    else
                    {
                        GoodsInMarketOwnDtos = null;
                        GoodsInMarketComboBox.ItemsSource = null;
                        GoodsInMarketComboBox.IsEnabled = false;
                    }
                }
            }
            else
            {
                GoodTitleLabel.Content = "";
                WeightLabel.Content = "";
                PriceLabel.Content = "";

                GoodsInMarketOwnDtos = null;
                GoodsInMarketComboBox.ItemsSource = null;
                GoodsInMarketComboBox.IsEnabled = false;
            }
        }

        private void GoodsInMarketComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GoodsInMarketComboBox.SelectedItem != null)
            {
                var tempProduction = (GoodsInMarketOwnDTO) GoodsInMarketComboBox.SelectedItem;
                AmountLabel.Content = "In stock: " + tempProduction.Amount;
            }
            else
            {
                AmountLabel.Content = "";
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
    }
}