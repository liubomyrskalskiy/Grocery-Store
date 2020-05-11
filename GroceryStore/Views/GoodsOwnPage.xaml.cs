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
    /// Interaction logic for GoodsOwnPage.xaml
    /// </summary>
    public partial class GoodsOwnPage : Page, IActivable
    {
        private readonly IGoodsOwnService _goodsOwnService;
        private readonly ICategoryService _categoryService;
        private AppSettings _settings;
        private readonly IMapper _mapper;

        public List<GoodsOwnDTO> GoodsOwnDtos { get; set; }

        public GoodsOwnPage(IGoodsOwnService goodsOwnService, ICategoryService categoryService, IOptions<AppSettings> settings, IMapper mapper)
        {
            _goodsOwnService = goodsOwnService;
            _categoryService = categoryService;
            _settings = settings.Value;
            _mapper = mapper;
            InitializeComponent();

            UpdateDataGrid();
        }

        private void UpdateDataGrid()
        {
            GoodsOwnDtos = _mapper.Map<List<GoodsOwn>, List<GoodsOwnDTO>>(_goodsOwnService.GetAll());

            DataGrid.ItemsSource = GoodsOwnDtos;
        }

        private bool ValidateForm()
        {
            if (!Regex.Match(TitleTextBox.Text, @"^\D{1,20}").Success)
            {
                MessageBox.Show("Title must consist of at least 1 character and not exceed 20 characters!");
                TitleTextBox.Focus();
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

            if (!Regex.Match(CategoryTextBox.Text, @"^\D{1,50}$").Success)
            {
                MessageBox.Show("Country title must consist of at least 1 character and not exceed 50 characters!");
                CategoryTextBox.Focus();
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

        public Task ActivateAsync(object parameter)
        {
            return Task.CompletedTask;
        }

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedIndex != -1)
            {
                TitleTextBox.Text = GoodsOwnDtos[DataGrid.SelectedIndex].Title;
                WeightTextBox.Text = GoodsOwnDtos[DataGrid.SelectedIndex].Weight.ToString();
                ComponentnsTextBox.Text = GoodsOwnDtos[DataGrid.SelectedIndex].Components;
                PriceTextBox.Text = GoodsOwnDtos[DataGrid.SelectedIndex].Price.ToString();
                CategoryTextBox.Text = GoodsOwnDtos[DataGrid.SelectedIndex].Category;
                ProductCodeTextBox.Text = GoodsOwnDtos[DataGrid.SelectedIndex].ProductCode;
            }
        }

        private void CreateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;
            GoodsOwn goodsOwn = new GoodsOwn();
            Category tempCategory = new Category();
            goodsOwn.Id = GoodsOwnDtos[^1]?.Id + 1 ?? 1;
            goodsOwn.Title = TitleTextBox.Text;
            goodsOwn.Weight = Convert.ToDouble(WeightTextBox.Text);
            goodsOwn.Components = ComponentnsTextBox.Text;
            goodsOwn.Price = Convert.ToDouble(PriceTextBox.Text);
            goodsOwn.ProductCode = ProductCodeTextBox.Text;
            if ((tempCategory = _categoryService.GetAll()
                    .FirstOrDefault(category => category.Title == CategoryTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such category in database!");
                return;
            }
            else
                goodsOwn.IdCategory = tempCategory.Id;

            _goodsOwnService.Create(goodsOwn);
            UpdateDataGrid();
        }

        private void UpdateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            if (!ValidateForm()) return;
            GoodsOwn goodsOwn = new GoodsOwn();
            Category tempCategory = new Category();
            goodsOwn.Id = GoodsOwnDtos[DataGrid.SelectedIndex].Id;
            goodsOwn.Title = TitleTextBox.Text;
            goodsOwn.Weight = Convert.ToDouble(WeightTextBox.Text);
            goodsOwn.Components = ComponentnsTextBox.Text;
            goodsOwn.Price = Convert.ToDouble(PriceTextBox.Text);
            goodsOwn.ProductCode = ProductCodeTextBox.Text;
            if ((tempCategory = _categoryService.GetAll()
                    .FirstOrDefault(category => category.Title == CategoryTextBox.Text)) == null)
            {
                MessageBox.Show("There is no such category in database!");
                return;
            }
            else
                goodsOwn.IdCategory = tempCategory.Id;

            _goodsOwnService.Update(goodsOwn);
            UpdateDataGrid();
        }

        private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            _goodsOwnService.Delete(GoodsOwnDtos[DataGrid.SelectedIndex].Id);
            UpdateDataGrid();
        }

        
    }
}
