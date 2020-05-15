using System;
using System.Collections.Generic;
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
    /// Interaction logic for ProductionDetailWindow.xaml
    /// </summary>
    public partial class ProductionDetailWindow : Window, IActivable
    {
        private readonly IProductionContentsService _productionContentsService;
        private readonly AppSettings _settings;
        private readonly IMapper _mapper;
        private string _productionNumber;
        private List<ProductionContentsDTO> ProductionContentsDtos { get; set; }

        public ProductionDetailWindow(IProductionContentsService productionContentsService, IMapper mapper, IOptions<AppSettings> settings)
        {
            _productionContentsService = productionContentsService;
            _mapper = mapper;
            _settings = settings.Value;
            InitializeComponent();
        }

        public Task ActivateAsync(object parameter)
        {
            _productionNumber = parameter.ToString();

            UpdateDataGrid();

            return Task.CompletedTask;
        }

        private void UpdateDataGrid()
        {
            ProductionContentsDtos =
                _mapper.Map<List<ProductionContents>, List<ProductionContentsDTO>>(_productionContentsService.GetAll());

            DataGrid.ItemsSource = ProductionContentsDtos.Where(item => item.ProductionCode == _productionNumber).ToList();
        }

        private void BtnClose(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
