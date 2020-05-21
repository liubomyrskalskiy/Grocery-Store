using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AutoMapper;
using GroceryStore.Core.Abstractions;
using GroceryStore.Core.Abstractions.IServices;
using GroceryStore.Core.DTO;
using GroceryStore.Core.Models;
using Microsoft.Extensions.Options;

namespace GroceryStore.Windows
{
    /// <summary>
    /// Interaction logic for SaleDetailWindow.xaml
    /// </summary>
    public partial class SaleDetailWindow : Window, IActivable
    {
        private readonly IBasketService _basketService;
        private readonly IBasketOwnService _basketOwnService;
        private AppSettings _settings;
        private readonly IMapper _mapper;
        private string _checkNumber;
        private SaleDTO _currentSale;
        public List<UniversalBasketDTO> UniversalBasketDtos { get; set; }

        public List<UniversalBasketDTO> BasketDtos { get; set; }

        public List<UniversalBasketDTO> BasketOwnDtos { get; set; }

        public SaleDetailWindow(IBasketService basketService, IBasketOwnService basketOwnService,
            IOptions<AppSettings> settings, IMapper mapper)
        {
            _basketService = basketService;
            _basketOwnService = basketOwnService;
            _settings = settings.Value;
            _mapper = mapper;

            InitializeComponent();
        }

        private void BtnClose(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public Task ActivateAsync(object parameter)
        {
            _currentSale = (SaleDTO) parameter;
            //_checkNumber = parameter.ToString();

            UpdateDataGrid();

            return Task.CompletedTask;
        }

        private void UpdateDataGrid()
        {
            BasketDtos = _mapper.Map<List<Basket>, List<UniversalBasketDTO>>(_basketService.GetAll());
            BasketOwnDtos = _mapper.Map<List<BasketOwn>, List<UniversalBasketDTO>>(_basketOwnService.GetAll());
            UniversalBasketDtos = BasketDtos.Where(item => item.CheckNumber == _currentSale.CheckNumber).ToList();
            UniversalBasketDtos.AddRange(BasketOwnDtos.Where(item => item.CheckNumber == _currentSale.CheckNumber).ToList());

            DataGrid.ItemsSource = UniversalBasketDtos;
        }

        private void CheckBtn_OnClick(object sender, RoutedEventArgs e)
        {
            List<string> lines = new List<string>(){ $"Check#          {_currentSale.CheckNumber}", $"Employee:          {_currentSale.FullName}", $"Date:     {_currentSale.Date}" };
            lines.Add("----------------------------------------");
            foreach (var universalBasketDto in UniversalBasketDtos)
            {
                lines.Add("");
                lines.Add($"{universalBasketDto.Title}     {universalBasketDto.Amount}     {universalBasketDto.Price}");
                lines.Add("----------------------------------------");
            }
            lines.Add("----------------------------------------");
            lines.Add($"Total:          {_currentSale.Total}");

            if (File.Exists($"Check#{_currentSale.CheckNumber}.txt"))
            {
                File.Delete($"Check#{_currentSale.CheckNumber}.txt");
            }

            using (StreamWriter file = File.CreateText($"Check#{_currentSale.CheckNumber}.txt"))
            {
                foreach (string line in lines)
                {
                    file.WriteLine(line.PadLeft(40));
                }
            }
            Process.Start("notepad.exe", $"Check#{_currentSale.CheckNumber}.txt");
        }
    }
}