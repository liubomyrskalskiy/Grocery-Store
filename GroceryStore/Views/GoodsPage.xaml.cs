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
using GroceryStore.Core.Mapping;
using GroceryStore.Core.Models;
using Microsoft.Extensions.Options;

namespace GroceryStore.Views
{
    /// <summary>
    /// Interaction logic for GoodsPage.xaml
    /// </summary>
    public partial class GoodsPage : Page, IActivable
    {
        private readonly IGoodsService _goodsService;
        private readonly ICategoryService _categoryService;
        private readonly IProducerService _producerService;
        private readonly AppSettings _settings;
        private readonly IMapper _mapper;

        public List<GoodsDTO> GoodsDtos { get; set; }

        public GoodsPage(IGoodsService goodsService, ICategoryService categoryService, IProducerService producerService, IOptions<AppSettings> settings, IMapper mapper)
        {
            _goodsService = goodsService;
            _categoryService = categoryService;
            _producerService = producerService;
            _mapper = mapper;
            _settings = settings.Value;

            InitializeComponent();

            UpdateDataGrid();
        }

        private void UpdateDataGrid()
        {
            GoodsDtos = _mapper.Map<List<Goods>, List<GoodsDTO>>(_goodsService.GetAll());

            DataGrid.ItemsSource = GoodsDtos;
        }

        private bool ValidateForm()
        {
            if (!Regex.Match(TitleTextBox.Text, @"^\D{1,20}$").Success)
            {
                MessageBox.Show("Title must consist of at least 1 character and not exceed 20 characters!");
                TitleTextBox.Focus();
                return false;
            }
            if (!Regex.Match(DescriptionTextBox.Text, @"^\D{1,200}$").Success)
            {
                MessageBox.Show("Description must consist of at least 1 character and not exceed 200 characters!");
                DescriptionTextBox.Focus();
                return false;
            }

            if (!Regex.Match(WeightTextBox.Text, @"^[0-9]*(?:\,[0-9]*)?$").Success)
            {
                MessageBox.Show("Invalid weight! Check the data you've entered!");
                WeightTextBox.Focus();
                return false;
            }

            if (!Regex.Match(ComponentsTextBox.Text, @"^\D{1,200}$").Success)
            {
                MessageBox.Show("Components must consist of at least 1 character and not exceed 200 characters!");
                ComponentsTextBox.Focus();
                return false;
            }

            if (!Regex.Match(PriceTextBox.Text, @"^[0-9]*(?:\,[0-9]*)?$").Success)
            {
                MessageBox.Show("Invalid price! Check the data you've entered!");
                PriceTextBox.Focus();
                return false;
            }

            if (!Regex.Match(CodeTextBox.Text, @"^\d{5}$").Success)
            {
                MessageBox.Show("Invalid product code! It must contain 5 digits");
                CodeTextBox.Focus();
                return false;
            }

            if (!Regex.Match(CategoryTextBox.Text, @"^\D{1,50}$").Success)
            {
                MessageBox.Show("Country title must consist of at least 1 character and not exceed 50 characters!");
                CategoryTextBox.Focus();
                return false;
            }

            if (!Regex.Match(ProducerTextBox.Text, @"^\D{1,50}$").Success)
            {
                MessageBox.Show("Producer title must consist of at least 1 character and not exceed 50 characters!");
                CategoryTextBox.Focus();
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
            Goods goods = new Goods();
            Category tempCategory;
            Producer tempProducer;
            goods.Id = GoodsDtos[^1]?.Id + 1 ?? 1;
            goods.Title = TitleTextBox.Text;
            goods.Description = DescriptionTextBox.Text;
            goods.Weight = Convert.ToDouble(WeightTextBox.Text);
            goods.Components = ComponentsTextBox.Text;
            goods.Price = Convert.ToDouble(PriceTextBox.Text);
            goods.ProductCode = CodeTextBox.Text;
            if ((tempCategory = _categoryService.GetAll().FirstOrDefault(category => category.Title == CategoryTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such category!");
                return;
            }
            else
                goods.IdCategory = tempCategory.Id;

            if ((tempProducer = _producerService.GetAll().FirstOrDefault(producer => producer.Title == ProducerTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such producer!");
                return;
            }
            else
                goods.IdProducer = tempProducer.Id;

            _goodsService.Create(goods);
            UpdateDataGrid();
        }

        private void UpdateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            if (!ValidateForm()) return;
            Goods goods = new Goods();
            Category tempCategory;
            Producer tempProducer;
            goods.Id = GoodsDtos[DataGrid.SelectedIndex].Id;
            goods.Title = TitleTextBox.Text;
            goods.Description = DescriptionTextBox.Text;
            goods.Weight = Convert.ToDouble(WeightTextBox.Text);
            goods.Components = ComponentsTextBox.Text;
            goods.Price = Convert.ToDouble(PriceTextBox.Text);
            goods.ProductCode = CodeTextBox.Text;
            if ((tempCategory = _categoryService.GetAll().FirstOrDefault(category => category.Title == CategoryTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such category!");
                return;
            }
            else
                goods.IdCategory = tempCategory.Id;

            if ((tempProducer = _producerService.GetAll().FirstOrDefault(producer => producer.Title == ProducerTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such producer!");
                return;
            }
            else
                goods.IdProducer = tempProducer.Id;

            _goodsService.Update(goods);
            UpdateDataGrid();
        }

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedIndex != -1)
            {
                TitleTextBox.Text = GoodsDtos[DataGrid.SelectedIndex].Title;
                DescriptionTextBox.Text = GoodsDtos[DataGrid.SelectedIndex].Description;
                WeightTextBox.Text = GoodsDtos[DataGrid.SelectedIndex].Weight.ToString();
                ComponentsTextBox.Text = GoodsDtos[DataGrid.SelectedIndex].Components;
                PriceTextBox.Text = GoodsDtos[DataGrid.SelectedIndex].Price.ToString();
                CodeTextBox.Text = GoodsDtos[DataGrid.SelectedIndex].ProductCode;
                CategoryTextBox.Text = GoodsDtos[DataGrid.SelectedIndex].CategoryTitle;
                ProducerTextBox.Text = GoodsDtos[DataGrid.SelectedIndex].ProducerTitle;
            }
        }

        private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            _goodsService.Delete(GoodsDtos[DataGrid.SelectedIndex].Id);
            UpdateDataGrid();
        }
    }
}
