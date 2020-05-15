using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for CategoryPage.xaml
    /// </summary>
    public partial class CategoryPage : Page, IActivable
    {
        private readonly ICategoryService _categoryService;
        private readonly AppSettings _settings;
        private readonly IMapper _mapper;

        public List<CategoryDTO> CategoryDtos { get; set; }

        public CategoryPage(ICategoryService categoryService, IOptions<AppSettings> settings, IMapper mapper)
        {
            InitializeComponent();

            _categoryService = categoryService;
            _mapper = mapper;
            _settings = settings.Value;

            UpdateDataGrid();
        }

        private void UpdateDataGrid()
        {
            CategoryDtos = _mapper.Map<List<Category>, List<CategoryDTO>>(_categoryService.GetAll());

            DataGrid.ItemsSource = CategoryDtos;
        }

        private bool ValidateForm()
        {
            if (!Regex.Match(TitleTextBox.Text, @"^\D{1,40}$").Success)
            {
                MessageBox.Show("Title must consist of at least 1 character and not exceed 40 characters!");
                TitleTextBox.Focus();
                return false;
            }

            if (!Regex.Match(DescriptionTextBox.Text, @"^\D{1,100}").Success)
            {
                MessageBox.Show("Description must consist of at least 1 character and not exceed 100 characters!");
                DescriptionTextBox.Focus();
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
            Category category = new Category();
            category.Id = CategoryDtos[^1]?.Id + 1 ?? 1;
            category.Title = TitleTextBox.Text;
            category.Description = DescriptionTextBox.Text;

            _categoryService.Create(category);
            UpdateDataGrid();
        }

        private void UpdateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            if (!ValidateForm()) return;
            Category category = new Category();
            category.Id = CategoryDtos[DataGrid.SelectedIndex].Id;
            category.Title = TitleTextBox.Text;
            category.Description = DescriptionTextBox.Text;

            _categoryService.Update(category);
            UpdateDataGrid();
        }

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedIndex != -1)
            {
                TitleTextBox.Text = CategoryDtos[DataGrid.SelectedIndex].Title;
                DescriptionTextBox.Text = CategoryDtos[DataGrid.SelectedIndex].Description;
            }
        }

        private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            _categoryService.Delete(CategoryDtos[DataGrid.SelectedIndex].Id);
            UpdateDataGrid();
        }
    }
}