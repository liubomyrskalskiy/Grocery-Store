using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for DeliveryOrderWindow.xaml
    /// </summary>
    public partial class DeliveryOrderWindow : Window, IActivable
    {
        private readonly IConsignmentService _consignmentService;
        private readonly IDeliveryContentsService _deliveryContentsService;
        private readonly IGoodsService _goodsService;
        private readonly IDeliveryService _deliveryService;
        private readonly AppSettings _settings;
        private readonly IMapper _mapper;

        private Delivery _currentDelivery;

        public List<DeliveryContents> DeliveryContentses { get; set; }
        public List<ConsignmentDTO> ConsignmentDtos { get; set; }
        public List<Consignment> CurrentConsignments { get; set; }
        public List<ConsignmentDTO> CurrentConsignmentDtos { get; set; }

        public DeliveryOrderWindow(IConsignmentService consignmentService, IDeliveryContentsService deliveryContentsService, IMapper mapper, IOptions<AppSettings> settings, IGoodsService goodsService, IDeliveryService deliveryService)
        {
            _consignmentService = consignmentService;
            _deliveryContentsService = deliveryContentsService;
            _mapper = mapper;
            _goodsService = goodsService;
            _deliveryService = deliveryService;
            _settings = settings.Value;

            InitializeComponent();
        }

        public Task ActivateAsync(object parameter)
        {
            _currentDelivery = (Delivery) parameter;
            DeliveryContentses = _deliveryContentsService.GetAll();
            DeliveryLabel.Content = "Order number: "+_currentDelivery.DeliveryNumber;
            DateLabel.Content = "Order Date: "+_currentDelivery.DeliveryDate;
            CurrentConsignments = new List<Consignment>();

            UpdateDataGrid();

            return Task.CompletedTask;
        }

        private void UpdateDataGrid()
        {
            ConsignmentDtos = _mapper.Map<List<Consignment>, List<ConsignmentDTO>>(_consignmentService.GetAll());
            CurrentConsignmentDtos = _mapper.Map<List<Consignment>, List<ConsignmentDTO>>(CurrentConsignments);
            DataGrid.ItemsSource = CurrentConsignmentDtos;
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

            if (!Regex.Match(IncomePriceTextBox.Text, @"^[0-9]*(?:\,[0-9]*)?$").Success || IncomePriceTextBox.Text == "")
            {
                MessageBox.Show("Invalid price! Check the data you've entered!");
                IncomePriceTextBox.Focus();
                return false;
            }

            return true;
        }


        private void ProductCodeTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!Regex.Match(ProductCodeTextBox.Text, @"^\d{5}$").Success)
            {
                return;
            }
            else
            {
                GoodsDTO goodsDto;
                Goods tempgood;
                if ((tempgood = _goodsService.GetAll()
                        .FirstOrDefault(gim =>
                            gim.ProductCode == ProductCodeTextBox.Text)) == null)
                {
                    GoodTitleLabel.Content = "";
                    ProducerTitleLabel.Content = "";
                    WeightLabel.Content = "";
                    PriceLabel.Content = "";
                    return;
                }
                else
                {
                    goodsDto = _mapper.Map<Goods, GoodsDTO>(tempgood);
                    GoodTitleLabel.Content = "Good: " + goodsDto.Title;
                    ProducerTitleLabel.Content = "Producer: " + goodsDto.ProducerTitle;
                    WeightLabel.Content = "Unit weight: " + goodsDto.Weight;
                    PriceLabel.Content = "Price: " + $"{goodsDto.Price,0:C2}";
                }

            }
        }

        private void CreateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;
            Consignment consignment = new Consignment();
            Goods tempGoods;
            consignment.Id = ConsignmentDtos[^1]?.Id + 1 ?? 1;
            consignment.ConsignmentNumber = "";
            consignment.Amount = Convert.ToDouble(AmountTextBox.Text);
            if ((tempGoods = _goodsService.GetAll()
                    .FirstOrDefault(goods => goods.ProductCode == ProductCodeTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such product in database!");
                return;
            }
            else
                consignment.IdGoods = tempGoods.Id;
                
            consignment.IncomePrice = Convert.ToDouble(IncomePriceTextBox.Text);

            _consignmentService.Create(consignment);

            consignment.IdGoodsNavigation = tempGoods;

            CurrentConsignments.Add(consignment);
            UpdateDataGrid();
        }

        private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            _consignmentService.Delete(CurrentConsignments[DataGrid.SelectedIndex].Id);
            CurrentConsignments.Remove(CurrentConsignments[DataGrid.SelectedIndex]);
            UpdateDataGrid();
        }

        private void DoneGoodBtn_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (var currentConsignment in CurrentConsignments)
            {
                DeliveryContents deliveryContents = new DeliveryContents();
                deliveryContents.Id = DeliveryContentses[^1]?.Id + 1 ?? 1;
                deliveryContents.IdDelivery = _currentDelivery.Id;
                deliveryContents.IdConsignment = currentConsignment.Id;

                _deliveryContentsService.Create(deliveryContents);
            }
            Close();
        }

        private void BtnClose(object sender, RoutedEventArgs e)
        {
            foreach (var currentConsignment in CurrentConsignments)
            {
                _consignmentService.Delete(currentConsignment.Id);
            }
            _deliveryService.Delete(_currentDelivery.Id);
            Close();
        }
    }
}
