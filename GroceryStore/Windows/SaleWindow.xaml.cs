using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using GroceryStore.NavigationTransferObjects;
using Microsoft.Extensions.Options;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;

namespace GroceryStore.Windows
{
    /// <summary>
    ///     Interaction logic for SaleWindow.xaml
    /// </summary>
    public partial class SaleWindow : Window, IActivable
    {
        private readonly IBasketOwnService _basketOwnService;
        private readonly IBasketService _basketService;
        private readonly IGoodsInMarketOwnService _goodsInMarketOwnService;
        private readonly IGoodsInMarketService _goodsInMarketService;
        private readonly IMapper _mapper;
        private readonly ISaleService _saleService;
        private readonly AppSettings _settings;
        private EmployeeDTO _curreEmployee;
        private Sale _currentSale;

        public SaleWindow(ISaleService saleService, IOptions<AppSettings> settings, IMapper mapper,
            IBasketOwnService basketOwnService, IBasketService basketService,
            IGoodsInMarketService goodsInMarketService, IGoodsInMarketOwnService goodsInMarketOwnService)
        {
            _saleService = saleService;
            _settings = settings.Value;
            _mapper = mapper;
            _basketOwnService = basketOwnService;
            _basketService = basketService;
            _goodsInMarketService = goodsInMarketService;
            _goodsInMarketOwnService = goodsInMarketOwnService;

            InitializeComponent();
        }

        public List<Basket> CurrentBaskets { get; set; }
        public List<BasketOwn> CurrentBasketOwns { get; set; }
        public List<UniversalBasketDTO> UniversalBasketDtos { get; set; }

        public List<UniversalBasketDTO> BasketDtos { get; set; }

        public List<UniversalBasketDTO> BasketOwnDtos { get; set; }

        public Task ActivateAsync(object parameter)
        {
            var data = (SaleNTO) parameter;
            _currentSale = data.Sale;
            _curreEmployee = data.EmployeeDto;
            CheckLabel.Content = "Сheck number: " + _currentSale.CheckNumber;

            CurrentBaskets = new List<Basket>();
            CurrentBasketOwns = new List<BasketOwn>();

            UpdateDataGrid();

            return Task.CompletedTask;
        }

        private void UpdateDataGrid()
        {
            BasketDtos = _mapper.Map<List<Basket>, List<UniversalBasketDTO>>(_basketService.GetAll());

            BasketDtos.Sort(delegate (UniversalBasketDTO x, UniversalBasketDTO y)
            {
                return x.Id.CompareTo(y.Id);
            });

            BasketOwnDtos = _mapper.Map<List<BasketOwn>, List<UniversalBasketDTO>>(_basketOwnService.GetAll());

            BasketOwnDtos.Sort(delegate (UniversalBasketDTO x, UniversalBasketDTO y)
            {
                return x.Id.CompareTo(y.Id);
            });

            UniversalBasketDtos = BasketDtos.Where(item => item.CheckNumber == _currentSale.CheckNumber).ToList();
            UniversalBasketDtos.AddRange(BasketOwnDtos.Where(item => item.CheckNumber == _currentSale.CheckNumber)
                .ToList());
            _saleService.Refresh(_currentSale);
            _currentSale = _saleService.GetId(_currentSale.Id);

            DateLabel.Content = _currentSale.Date.ToString();
            TotalLabel.Content = $"{_currentSale.Total,0:C2}";

            DataGrid.ItemsSource = UniversalBasketDtos;
        }

        private void BtnClose(object sender, RoutedEventArgs e)
        {
            foreach (var basketDto in UniversalBasketDtos)
                if (!basketDto.IsOwn)
                    _basketService.Delete(basketDto.Id);
                else
                    _basketOwnService.Delete(basketDto.Id);

            _saleService.Delete(_currentSale.Id);
            Close();
        }

        private bool ValidateForm()
        {
            if (!Regex.Match(AmountTextBox.Text, @"^[0-9]*(?:\,[0-9]*)?$").Success || AmountTextBox.Text == "")
            {
                MessageBox.Show("Invalid amount! Check the data you've entered!");
                AmountTextBox.Focus();
                return false;
            }

            if (!Regex.Match(ProductCodeTextBox.Text, @"^\d{5}$").Success)
            {
                MessageBox.Show("Invalid product code! It must contain 5 digits");
                ProductCodeTextBox.Focus();
                return false;
            }

            return true;
        }

        private void CreateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;
            if (OwnCheckBox.IsChecked == true)
            {
                if (UniversalBasketDtos.FirstOrDefault(item =>
                        item.ProductCode == ProductCodeTextBox.Text && item.IsOwn) != null)
                {
                    MessageBox.Show("There is such goods in basket already!");
                    return;
                }

                var basketOwn = new BasketOwn();
                GoodsInMarketOwn tempGimo;
                basketOwn.Id = BasketOwnDtos[^1]?.Id + 1 ?? 1;
                basketOwn.Amount = Convert.ToDouble(AmountTextBox.Text);
                basketOwn.IdSale = _currentSale.Id;

                if ((tempGimo = _goodsInMarketOwnService.GetAll()
                        .FirstOrDefault(gim =>
                            gim.IdProductionNavigation?.IdGoodsOwnNavigation.ProductCode == ProductCodeTextBox.Text &&
                            gim.Amount >= Convert.ToDouble(AmountTextBox.Text) &&
                            gim.IdMarketNavigation.Address == _curreEmployee.MarketAddress)) ==
                    null)
                {
                    MessageBox.Show("There is no such product or there is not enough goods in your store!");
                    return;
                }

                basketOwn.IdGoodsInMarketOwn = tempGimo.Id;

                CurrentBasketOwns.Add(basketOwn);

                _basketOwnService.Create(basketOwn);
                UpdateDataGrid();
            }
            else
            {
                if (UniversalBasketDtos.FirstOrDefault(item =>
                        item.ProductCode == ProductCodeTextBox.Text && item.IsOwn == false) != null)
                {
                    MessageBox.Show("There is such goods in basket already!");
                    return;
                }

                var basket = new Basket();
                GoodsInMarket tempgim;
                basket.Id = BasketDtos[^1]?.Id + 1 ?? 1;
                basket.Amount = Convert.ToDouble(AmountTextBox.Text);
                basket.IdSale = _currentSale.Id;
                if ((tempgim = _goodsInMarketService.GetAll()
                        .FirstOrDefault(gim =>
                            gim.IdGoodsNavigation.ProductCode == ProductCodeTextBox.Text &&
                            gim.Amount >= Convert.ToDouble(AmountTextBox.Text) &&
                            gim.IdMarketNavigation.Address == _curreEmployee.MarketAddress)) == null)
                {
                    MessageBox.Show("There is no such product or there is not enough product in your store!");
                    return;
                }

                basket.IdGoodsInMarket = tempgim.Id;

                CurrentBaskets.Add(basket);

                _basketService.Create(basket);
                UpdateDataGrid();
            }
        }

        private void ProductCodeTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!Regex.Match(ProductCodeTextBox.Text, @"^\d{5}$").Success)
            {
            }
            else
            {
                if (OwnCheckBox.IsChecked == true)
                {
                    GoodsInMarketOwnDTO goodsInMarketOwnDto;
                    GoodsInMarketOwn tempGimo;
                    if ((tempGimo = _goodsInMarketOwnService.GetAll()
                            .FirstOrDefault(gim =>
                                gim.IdProductionNavigation?.IdGoodsOwnNavigation.ProductCode ==
                                ProductCodeTextBox.Text)) ==
                        null)
                    {
                        GoodTitleLabel.Content = "";
                        ProducerTitleLabel.Content = "";
                        WeightLabel.Content = "";
                        PriceLabel.Content = "";
                        AmountLabel.Content = "";
                    }
                    else
                    {
                        goodsInMarketOwnDto = _mapper.Map<GoodsInMarketOwn, GoodsInMarketOwnDTO>(tempGimo);
                        GoodTitleLabel.Content = "Good: " + goodsInMarketOwnDto.Good;
                        ProducerTitleLabel.Content =
                            "Manufacture date: " + goodsInMarketOwnDto.ManufactureDate;
                        WeightLabel.Content = "Unit weight: " + goodsInMarketOwnDto.Weight;
                        PriceLabel.Content = "Price: " + goodsInMarketOwnDto.Price;
                        AmountLabel.Content = "Remains in store: " + goodsInMarketOwnDto.Amount;
                    }
                }
                else
                {
                    GoodsInMarketDTO goodsInMarketDto;
                    GoodsInMarket tempgim;
                    if ((tempgim = _goodsInMarketService.GetAll()
                            .FirstOrDefault(gim =>
                                gim.IdGoodsNavigation.ProductCode == ProductCodeTextBox.Text &&
                                gim.IdMarketNavigation.Address == _curreEmployee.MarketAddress)) == null)
                    {
                        GoodTitleLabel.Content = "";
                        ProducerTitleLabel.Content = "";
                        WeightLabel.Content = "";
                        PriceLabel.Content = "";
                        AmountLabel.Content = "";
                    }
                    else
                    {
                        goodsInMarketDto = _mapper.Map<GoodsInMarket, GoodsInMarketDTO>(tempgim);
                        GoodTitleLabel.Content = "Good: " + goodsInMarketDto.Good;
                        ProducerTitleLabel.Content = "Producer: " + goodsInMarketDto.Producer;
                        WeightLabel.Content = "Unit weight: " + goodsInMarketDto.Weight;
                        PriceLabel.Content = "Price: " + goodsInMarketDto.Price;
                        AmountLabel.Content = "Remains in store: " + goodsInMarketDto.Amount;
                    }
                }
            }
        }

        private void FormCheck()
        {
            var titleLines = new List<string>
            {
                $"Check №{_currentSale.CheckNumber,33}", $"Employee:{_curreEmployee.FullName,31}",
                $"Date:{_currentSale.Date,35}", "----------------------------------------"
            };

            var goodsList = new List<string>();
            foreach (var universalBasketDto in UniversalBasketDtos)
            {
                goodsList.Add("----------------------------------------");
                goodsList.Add($"{universalBasketDto.Producer}");
                goodsList.Add($"{universalBasketDto.Title,10}");
                goodsList.Add($"{universalBasketDto.Amount,20}{universalBasketDto.Price,20}");
            }

            var totalList = new List<string>
                {"----------------------------------------", "----------------------------------------"};
            if (_currentSale.IdClient != null) totalList.Add($"{"Discount:",30}{"5%",10}");

            totalList.Add($"{"Total:",30}{_currentSale.Total,10}");

            if (File.Exists($"Check{_currentSale.CheckNumber}.pdf"))
                File.Delete($"Check{_currentSale.CheckNumber}.pdf");

            var pdf = new PdfDocument();
            var pdfPage = pdf.Pages.Add();
            var graph = XGraphics.FromPdfPage(pdfPage);
            var titleFont = new XFont("Consolas", 8, XFontStyle.Bold);
            var font = new XFont("Consolas", 8, XFontStyle.Regular);
            var tf = new XTextFormatter(graph);
            var yPoint = 0;

            foreach (var titleLine in titleLines)
            {
                tf.Alignment = XParagraphAlignment.Left;
                tf.DrawString(titleLine, titleFont, XBrushes.Black, new XRect(8, yPoint, pdfPage.Width, pdfPage.Height),
                    XStringFormats.TopLeft);
                yPoint += 8;
            }

            foreach (var good in goodsList)
            {
                tf.Alignment = XParagraphAlignment.Left;
                tf.DrawString(good, font, XBrushes.Black, new XRect(8, yPoint, pdfPage.Width, pdfPage.Height),
                    XStringFormats.TopLeft);
                yPoint += 8;
            }

            foreach (var total in totalList)
            {
                tf.Alignment = XParagraphAlignment.Left;
                tf.DrawString(total, titleFont, XBrushes.Black, new XRect(8, yPoint, pdfPage.Width, pdfPage.Height),
                    XStringFormats.TopLeft);
                yPoint += 8;
            }

            var pdfFilename = $"Check{_currentSale.CheckNumber}.pdf";
            pdf.Save(pdfFilename);
            Process.Start("C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe",
                Path.GetFullPath(pdfFilename));
        }

        private void DoneGoodBtn_OnClick(object sender, RoutedEventArgs e)
        {
            _saleService.Refresh(_currentSale);
            _currentSale = _saleService.GetId(_currentSale.Id);
            foreach (var currentBasket in CurrentBaskets)
            {
                var gim = _goodsInMarketService.GetId(currentBasket.IdGoodsInMarket ?? 0);
                gim.Amount -= currentBasket.Amount;
                _goodsInMarketService.Update(gim);
            }

            foreach (var currentBasketOwn in CurrentBasketOwns)
            {
                var gimo = _goodsInMarketOwnService.GetId(currentBasketOwn.IdGoodsInMarketOwn ?? 0);
                gimo.Amount -= currentBasketOwn.Amount;
                _goodsInMarketOwnService.Update(gimo);
            }

            FormCheck();

            Close();
        }

        private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex != -1)
            {
                if (!UniversalBasketDtos[DataGrid.SelectedIndex].IsOwn)
                {
                    Basket tempBasket;
                    tempBasket = CurrentBaskets.FirstOrDefault(item =>
                        item.Id == UniversalBasketDtos[DataGrid.SelectedIndex].Id);
                    CurrentBaskets.Remove(tempBasket);
                    _basketService.Delete(UniversalBasketDtos[DataGrid.SelectedIndex].Id);
                }
                else
                {
                    BasketOwn tempBasket;
                    tempBasket = CurrentBasketOwns.FirstOrDefault(item =>
                        item.Id == UniversalBasketDtos[DataGrid.SelectedIndex].Id);
                    CurrentBasketOwns.Remove(tempBasket);
                    _basketOwnService.Delete(UniversalBasketDtos[DataGrid.SelectedIndex].Id);
                }

                UpdateDataGrid();
            }
        }
    }
}