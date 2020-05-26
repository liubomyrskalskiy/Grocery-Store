using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using AutoMapper;
using GroceryStore.Core.Abstractions;
using GroceryStore.Core.Abstractions.IServices;
using GroceryStore.Core.DTO;
using GroceryStore.Core.Models;
using Microsoft.Extensions.Options;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;

namespace GroceryStore.Windows
{
    /// <summary>
    ///     Interaction logic for SaleDetailWindow.xaml
    /// </summary>
    public partial class SaleDetailWindow : Window, IActivable
    {
        private readonly IBasketOwnService _basketOwnService;
        private readonly IBasketService _basketService;
        private readonly IMapper _mapper;
        private readonly AppSettings _settings;
        private SaleDTO _currentSale;

        public SaleDetailWindow(IBasketService basketService, IBasketOwnService basketOwnService,
            IOptions<AppSettings> settings, IMapper mapper)
        {
            _basketService = basketService;
            _basketOwnService = basketOwnService;
            _settings = settings.Value;
            _mapper = mapper;

            InitializeComponent();
        }

        public List<UniversalBasketDTO> UniversalBasketDtos { get; set; }

        public List<UniversalBasketDTO> BasketDtos { get; set; }

        public List<UniversalBasketDTO> BasketOwnDtos { get; set; }

        public Task ActivateAsync(object parameter)
        {
            _currentSale = (SaleDTO) parameter;

            UpdateDataGrid();

            return Task.CompletedTask;
        }

        private void BtnClose(object sender, RoutedEventArgs e)
        {
            Close();
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

            DataGrid.ItemsSource = UniversalBasketDtos;
        }

        private void CheckBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var titleLines = new List<string>
            {
                $"Check №{_currentSale.CheckNumber,33}", $"Employee:{_currentSale.FullName,31}",
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
            if (_currentSale.AccountNumber != null && _currentSale.AccountNumber != "")
                totalList.Add($"{"Discount:",30}{"5%",10}");

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
    }
}