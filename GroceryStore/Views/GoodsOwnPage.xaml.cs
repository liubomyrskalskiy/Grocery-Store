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
    ///     Interaction logic for GoodsOwnPage.xaml
    /// </summary>
    public partial class GoodsOwnPage : Page, IActivable
    {
        private readonly ICategoryService _categoryService;
        private readonly IGoodsOwnService _goodsOwnService;
        private readonly IProductionService _productionService;
        private readonly IMapper _mapper;
        private readonly AppSettings _settings;

        public GoodsOwnPage(IGoodsOwnService goodsOwnService, ICategoryService categoryService,
            IOptions<AppSettings> settings, IMapper mapper, IProductionService productionService)
        {
            _goodsOwnService = goodsOwnService;
            _categoryService = categoryService;
            _settings = settings.Value;
            _mapper = mapper;
            _productionService = productionService;
            InitializeComponent();
        }

        public List<GoodsOwnDTO> GoodsOwnDtos { get; set; }
        public List<GoodsOwnDTO> FilteredGoodsOwnDtos { get; set; }
        public List<CategoryDTO> CategoryDtos { get; set; }

        public Task ActivateAsync(object parameter)
        {
            CategoryDtos = _mapper.Map<List<Category>, List<CategoryDTO>>(_categoryService.GetAll());
            CategoryComboBox.ItemsSource = CategoryDtos;
            CategoryFilterComboBox.ItemsSource = CategoryDtos;
            UpdateDataGrid();

            return Task.CompletedTask;
        }

        private void UpdateDataGrid()
        {
            GoodsOwnDtos = _mapper.Map<List<GoodsOwn>, List<GoodsOwnDTO>>(_goodsOwnService.GetAll());

            GoodsOwnDtos.Sort(delegate (GoodsOwnDTO x, GoodsOwnDTO y)
            {
                return x.Id.CompareTo(y.Id);
            });

            FilteredGoodsOwnDtos = GoodsOwnDtos;

            if (Regex.Match(TitleFilterTextBox.Text, @"^\D{1,20}$").Success)
            {
                var tempList = FilteredGoodsOwnDtos.Where(item => item.Title.Contains(TitleFilterTextBox.Text))
                    .ToList();
                FilteredGoodsOwnDtos = tempList;
            }

            if (CategoryFilterComboBox.SelectedItem != null)
            {
                var tempCategoty = (CategoryDTO) CategoryFilterComboBox.SelectedItem;
                var tempList = FilteredGoodsOwnDtos.Where(item => item.Category == tempCategoty.Title).ToList();
                FilteredGoodsOwnDtos = tempList;
            }

            DataGrid.ItemsSource = FilteredGoodsOwnDtos;
        }

        private bool ValidateForm()
        {
            if (!Regex.Match(TitleTextBox.Text, @"^\D{1,20}").Success)
            {
                MessageBox.Show("Title must consist of at least 1 character and not exceed 20 characters!");
                TitleTextBox.Focus();
                return false;
            }

            if (CategoryComboBox.SelectedItem == null)
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

            if (!Regex.Match(ComponentnsTextBox.Text, @"^\D{1,250}$").Success)
            {
                MessageBox.Show("Components must consist of at least 1 character and not exceed 250 characters!");
                ComponentnsTextBox.Focus();
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

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedIndex != -1)
            {
                TitleTextBox.Text = FilteredGoodsOwnDtos[DataGrid.SelectedIndex].Title;
                WeightTextBox.Text = FilteredGoodsOwnDtos[DataGrid.SelectedIndex].Weight.ToString();
                ComponentnsTextBox.Text = FilteredGoodsOwnDtos[DataGrid.SelectedIndex].Components;
                PriceTextBox.Text = FilteredGoodsOwnDtos[DataGrid.SelectedIndex].Price.ToString();
                CategoryComboBox.SelectedItem = CategoryDtos.FirstOrDefault(item =>
                    item.Title == FilteredGoodsOwnDtos[DataGrid.SelectedIndex].Category);
            }
        }

        private void CreateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;
            var goodsOwn = new GoodsOwn();
            CategoryDTO tempCategory;
            goodsOwn.Id = GoodsOwnDtos[^1]?.Id + 1 ?? 1;
            goodsOwn.Title = TitleTextBox.Text;
            goodsOwn.Weight = Convert.ToDouble(WeightTextBox.Text);
            goodsOwn.Components = ComponentnsTextBox.Text;
            goodsOwn.Price = Convert.ToDouble(PriceTextBox.Text);
            goodsOwn.ProductCode = goodsOwn.Id.ToString("D5");
            tempCategory = (CategoryDTO) CategoryComboBox.SelectedItem;
            goodsOwn.IdCategory = tempCategory.Id;

            _goodsOwnService.Create(goodsOwn);
            UpdateDataGrid();
        }

        private void UpdateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            if (!ValidateForm()) return;
            var goodsOwn = new GoodsOwn();
            CategoryDTO tempCategory;
            goodsOwn.Id = FilteredGoodsOwnDtos[DataGrid.SelectedIndex].Id;
            goodsOwn.Title = TitleTextBox.Text;
            goodsOwn.Weight = Convert.ToDouble(WeightTextBox.Text);
            goodsOwn.Components = ComponentnsTextBox.Text;
            goodsOwn.Price = Convert.ToDouble(PriceTextBox.Text);
            goodsOwn.ProductCode = FilteredGoodsOwnDtos[DataGrid.SelectedIndex].ProductCode;
            tempCategory = (CategoryDTO) CategoryComboBox.SelectedItem;
            goodsOwn.IdCategory = tempCategory.Id;

            _goodsOwnService.Update(goodsOwn);
            UpdateDataGrid();
        }

        private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            if(_productionService.GetAll().FirstOrDefault(item => item.IdGoodsOwn == FilteredGoodsOwnDtos[DataGrid.SelectedIndex].Id) != null)
            {
                MessageBox.Show("You can only delete recently added items!");
                return;
            }
            _goodsOwnService.Delete(FilteredGoodsOwnDtos[DataGrid.SelectedIndex].Id);
            UpdateDataGrid();
        }

        private void CategoryFilterComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CategoryFilterComboBox.SelectedItem != null) UpdateDataGrid();
        }

        private void ClearCategoryFilterBtn_OnClick(object sender, RoutedEventArgs e)
        {
            CategoryFilterComboBox.SelectedItem = null;
            UpdateDataGrid();
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
    }
}