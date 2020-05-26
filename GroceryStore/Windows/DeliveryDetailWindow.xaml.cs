using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using AutoMapper;
using GroceryStore.Core.Abstractions;
using GroceryStore.Core.Abstractions.IServices;
using GroceryStore.Core.DTO;
using GroceryStore.Core.Models;
using Microsoft.Extensions.Options;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;

namespace GroceryStore.Windows
{
    /// <summary>
    ///     Interaction logic for DeliveryDetailWindow.xaml
    /// </summary>
    public partial class DeliveryDetailWindow : Window, IActivable
    {
        private readonly IDeliveryContentsService _deliveryContentsService;
        private readonly IMapper _mapper;
        private readonly SimpleNavigationService _navigationService;
        private readonly AppSettings _settings;

        private DeliveryDTO _currentDelivery;

        public DeliveryDetailWindow(IDeliveryContentsService deliveryContentsService, IMapper mapper,
            SimpleNavigationService navigationService, IOptions<AppSettings> settings)
        {
            _deliveryContentsService = deliveryContentsService;
            _mapper = mapper;
            _navigationService = navigationService;
            _settings = settings.Value;
            InitializeComponent();
        }

        public List<DeliveryContentsDTO> DeliveryContentsDtos { get; set; }
        public List<DeliveryContentsDTO> FilteredDeliveryContentsDtos { get; set; }

        public Task ActivateAsync(object parameter)
        {
            _currentDelivery = (DeliveryDTO) parameter;
            DeliveryLabel.Content = "Order number: " + _currentDelivery.DeliveryNumber;
            DateLabel.Content = "Order Date: " + _currentDelivery.DeliveryDate;
            TotalLabel.Content = _currentDelivery.StringTotal;
            UpdateDataGrid();
            return Task.CompletedTask;
        }

        private void UpdateDataGrid()
        {
            DeliveryContentsDtos =
                _mapper.Map<List<DeliveryContents>, List<DeliveryContentsDTO>>(_deliveryContentsService.GetAll());
            DeliveryContentsDtos.Sort(delegate (DeliveryContentsDTO x, DeliveryContentsDTO y)
            {
                return x.Id.CompareTo(y.Id);
            });
            FilteredDeliveryContentsDtos = DeliveryContentsDtos
                .Where(item => item.DeliveryNumber == _currentDelivery.DeliveryNumber).ToList();
            DataGrid.ItemsSource = FilteredDeliveryContentsDtos;
        }

        private void BtnClose(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void DataGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            if (FilteredDeliveryContentsDtos[DataGrid.SelectedIndex].ConsignmentNumber != "")
            {
                await _navigationService.ShowDialogAsync<DeliveryOrderDetailWindow>(
                    FilteredDeliveryContentsDtos[DataGrid.SelectedIndex].ConsignmentNumber);
            }
            else
            {
                var dc = _deliveryContentsService.GetAll().FirstOrDefault(item =>
                    item.Id == FilteredDeliveryContentsDtos[DataGrid.SelectedIndex].Id);
                var result =
                    await _navigationService.ShowDialogAsync<DeliveryOrderUpdateWindow>(dc.IdConsignmentNavigation);
                UpdateDataGrid();
            }
        }

        private void BtnPrintInvoice_Click(object sender, RoutedEventArgs e)
        {
            var titleLines = new List<string>
            {
                $"|Invoice №{_currentDelivery.DeliveryNumber,39}|Order date:{_currentDelivery.DeliveryDate,78}|",
                $"|Provider:{_currentDelivery.ProviderTitle,39}|Contact person:{_currentDelivery.ContactPerson,40}|Phone:{_currentDelivery.PhoneNumber,27}|",
                "--------------------------------------------------------------------------------------------------------------------------------------------"
            };

            var consgnmentsList = new List<string>();
            double totalPrice = 0;
            foreach (var deliveryContent in FilteredDeliveryContentsDtos)
                if (deliveryContent.ConsignmentNumber != null && deliveryContent.ConsignmentNumber != "")
                {
                    consgnmentsList.Add(
                        "--------------------------------------------------------------------------------------------------------------------------------------------");
                    consgnmentsList.Add(
                        $"|Code:{deliveryContent.ProductCode,10}|Producer:{deliveryContent.ProducerTitle,20}|Good:{deliveryContent.GoodTitle,20}|Consignment:{deliveryContent.ConsignmentNumber,15}|Amount:{deliveryContent.StringOrderAmount,12}|Price:{deliveryContent.StringIncomePrice,12}|");
                    totalPrice += deliveryContent.OrderAmount * deliveryContent.IncomePrice ?? 0;
                }

            var stringTotalPrice = $"{totalPrice,0:C2}";

            var totalList = new List<string>
            {
                "--------------------------------------------------------------------------------------------------------------------------------------------",
                $"|Total:{stringTotalPrice,12}|"
            };

            if (File.Exists($"Invoice{_currentDelivery.DeliveryNumber}.pdf"))
                File.Delete($"Invoice{_currentDelivery.DeliveryNumber}.pdf");

            var pdf = new PdfDocument();
            var pdfPage = pdf.Pages.Add();
            pdfPage.Orientation = PageOrientation.Landscape;
            var graph = XGraphics.FromPdfPage(pdfPage);
            var titleFont = new XFont("Consolas", 10, XFontStyle.Bold);
            var font = new XFont("Consolas", 10, XFontStyle.Regular);
            var tf = new XTextFormatter(graph);
            var yPoint = 0;

            foreach (var titleLine in titleLines)
            {
                tf.Alignment = XParagraphAlignment.Right;
                tf.DrawString(titleLine, titleFont, XBrushes.Black,
                    new XRect(10, yPoint, pdfPage.Width - 10, pdfPage.Height), XStringFormats.TopLeft);
                yPoint += 10;
            }

            foreach (var consignment in consgnmentsList)
            {
                tf.Alignment = XParagraphAlignment.Right;
                tf.DrawString(consignment, font, XBrushes.Black,
                    new XRect(10, yPoint, pdfPage.Width - 10, pdfPage.Height), XStringFormats.TopLeft);
                yPoint += 10;
            }

            foreach (var total in totalList)
            {
                tf.Alignment = XParagraphAlignment.Right;
                if (totalList.IndexOf(total) == totalList.Count - 1)
                {
                    tf.Alignment = XParagraphAlignment.Right;
                    tf.DrawString(total, titleFont, XBrushes.Black,
                        new XRect(10, yPoint, pdfPage.Width - 10, pdfPage.Height), XStringFormats.TopLeft);
                }
                else
                {
                    tf.DrawString(total, titleFont, XBrushes.Black,
                        new XRect(10, yPoint, pdfPage.Width - 10, pdfPage.Height), XStringFormats.TopLeft);
                }

                yPoint += 10;
            }

            var pdfFilename = $"Invoice{_currentDelivery.DeliveryNumber}.pdf";
            pdf.Save(pdfFilename);
            Process.Start("C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe",
                Path.GetFullPath(pdfFilename));
        }
    }
}