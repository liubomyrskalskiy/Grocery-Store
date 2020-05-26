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
    ///     Interaction logic for GoodsPage.xaml
    /// </summary>
    public partial class GoodsPage : Page, IActivable
    {
        private readonly ICategoryService _categoryService;
        private readonly IGoodsService _goodsService;
        private readonly IMapper _mapper;
        private readonly IProducerService _producerService;
        private readonly AppSettings _settings;

        public GoodsPage(IGoodsService goodsService, ICategoryService categoryService, IProducerService producerService,
            IOptions<AppSettings> settings, IMapper mapper)
        {
            _goodsService = goodsService;
            _categoryService = categoryService;
            _producerService = producerService;
            _mapper = mapper;
            _settings = settings.Value;

            InitializeComponent();
        }

        public List<GoodsDTO> GoodsDtos { get; set; }
        public List<GoodsDTO> FilteredGoodsDtos { get; set; }
        public List<CategoryDTO> CategoryDtos { get; set; }
        public List<ProducerDTO> ProducerDtos { get; set; }

        public Task ActivateAsync(object parameter)
        {
            CategoryDtos = _mapper.Map<List<Category>, List<CategoryDTO>>(_categoryService.GetAll());
            ProducerDtos = _mapper.Map<List<Producer>, List<ProducerDTO>>(_producerService.GetAll());

            ProducerComboBox.ItemsSource = ProducerDtos;
            ProducerFilterComboBox.ItemsSource = ProducerDtos;
            CategoryComboBox.ItemsSource = CategoryDtos;
            CategoryFilterComboBox.ItemsSource = CategoryDtos;
            UpdateDataGrid();

            return Task.CompletedTask;
        }

        private void UpdateDataGrid()
        {
            GoodsDtos = _mapper.Map<List<Goods>, List<GoodsDTO>>(_goodsService.GetAll());

            GoodsDtos.Sort(delegate (GoodsDTO x, GoodsDTO y)
            {
                return x.Id.CompareTo(y.Id);
            });

            FilteredGoodsDtos = GoodsDtos;
            if (Regex.Match(TitleFilterTextBox.Text, @"^\D{1,20}$").Success)
            {
                var tempList = FilteredGoodsDtos.Where(item => item.Title.Contains(TitleFilterTextBox.Text)).ToList();
                FilteredGoodsDtos = tempList;
            }

            if (ProducerFilterComboBox.SelectedItem != null)
            {
                var tempProducer = (ProducerDTO) ProducerFilterComboBox.SelectedItem;
                var tempList = FilteredGoodsDtos.Where(item => item.ProducerTitle == tempProducer.Title).ToList();
                FilteredGoodsDtos = tempList;
            }

            if (CategoryFilterComboBox.SelectedItem != null)
            {
                var tempCategoty = (CategoryDTO) CategoryFilterComboBox.SelectedItem;
                var tempList = FilteredGoodsDtos.Where(item => item.CategoryTitle == tempCategoty.Title).ToList();
                FilteredGoodsDtos = tempList;
            }

            DataGrid.ItemsSource = FilteredGoodsDtos;
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

            if (CategoryComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select category");
                return false;
            }

            if (ProducerComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select categoty");
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

            return true;
        }

        private void CreateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;
            var goods = new Goods();
            CategoryDTO tempCategory;
            ProducerDTO tempProducer;
            goods.Id = GoodsDtos[^1]?.Id + 1 ?? 1;
            goods.Title = TitleTextBox.Text;
            goods.Description = DescriptionTextBox.Text;
            goods.Weight = Convert.ToDouble(WeightTextBox.Text);
            goods.Components = ComponentsTextBox.Text;
            goods.Price = Convert.ToDouble(PriceTextBox.Text);
            goods.ProductCode = goods.Id.ToString("D5");
            tempCategory = (CategoryDTO) CategoryComboBox.SelectedItem;
            goods.IdCategory = tempCategory.Id;
            tempProducer = (ProducerDTO) ProducerComboBox.SelectedItem;
            goods.IdProducer = tempProducer.Id;

            _goodsService.Create(goods);
            UpdateDataGrid();
        }

        private void UpdateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            if (!ValidateForm()) return;
            var goods = new Goods();
            CategoryDTO tempCategory;
            ProducerDTO tempProducer;
            goods.Id = FilteredGoodsDtos[DataGrid.SelectedIndex].Id;
            goods.Title = TitleTextBox.Text;
            goods.Description = DescriptionTextBox.Text;
            goods.Weight = Convert.ToDouble(WeightTextBox.Text);
            goods.Components = ComponentsTextBox.Text;
            goods.Price = Convert.ToDouble(PriceTextBox.Text);
            goods.ProductCode = FilteredGoodsDtos[DataGrid.SelectedIndex].ProductCode;

            tempCategory = (CategoryDTO) CategoryComboBox.SelectedItem;
            goods.IdCategory = tempCategory.Id;

            tempProducer = (ProducerDTO) ProducerComboBox.SelectedItem;
            goods.IdProducer = tempProducer.Id;

            _goodsService.Update(goods);
            UpdateDataGrid();
        }

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedIndex != -1)
            {
                TitleTextBox.Text = FilteredGoodsDtos[DataGrid.SelectedIndex].Title;
                DescriptionTextBox.Text = FilteredGoodsDtos[DataGrid.SelectedIndex].Description;
                WeightTextBox.Text = FilteredGoodsDtos[DataGrid.SelectedIndex].Weight.ToString();
                ComponentsTextBox.Text = FilteredGoodsDtos[DataGrid.SelectedIndex].Components;
                PriceTextBox.Text = FilteredGoodsDtos[DataGrid.SelectedIndex].Price.ToString();
                ProducerComboBox.SelectedItem = ProducerDtos.FirstOrDefault(item =>
                    item.Title == FilteredGoodsDtos[DataGrid.SelectedIndex].ProducerTitle);
                CategoryComboBox.SelectedItem = CategoryDtos.FirstOrDefault(item =>
                    item.Title == FilteredGoodsDtos[DataGrid.SelectedIndex].CategoryTitle);
            }
        }

        private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            _goodsService.Delete(FilteredGoodsDtos[DataGrid.SelectedIndex].Id);
            UpdateDataGrid();
        }

        private void ClearFilterBtn_OnClick(object sender, RoutedEventArgs e)
        {
            ProducerFilterComboBox.SelectedItem = null;
            CategoryFilterComboBox.SelectedItem = null;
            ProducerFilterComboBox.IsEnabled = true;
            CategoryFilterComboBox.IsEnabled = true;
            UpdateDataGrid();
        }

        private void ProducerFilterComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProducerFilterComboBox.SelectedItem != null) UpdateDataGrid();
        }

        private void CategoryFilterComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CategoryFilterComboBox.SelectedItem != null) UpdateDataGrid();
        }

        private void ClearProducerFilterBtn_OnClick(object sender, RoutedEventArgs e)
        {
            ProducerFilterComboBox.SelectedItem = null;
            UpdateDataGrid();
        }

        private void ClearCategoryFilterBtn_OnClick(object sender, RoutedEventArgs e)
        {
            CategoryFilterComboBox.SelectedItem = null;
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

        private void ClearTitleFilterBtn_OnClick(object sender, RoutedEventArgs e)
        {
            TitleFilterTextBox.Text = "";
            UpdateDataGrid();
        }
    }
}