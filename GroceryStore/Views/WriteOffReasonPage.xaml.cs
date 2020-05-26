using System.Collections.Generic;
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
    ///     Interaction logic for WriteOffReasonPage.xaml
    /// </summary>
    public partial class WriteOffReasonPage : Page, IActivable
    {
        private readonly IMapper _mapper;
        private readonly AppSettings _settings;
        private readonly IWriteOffReasonService _writeOffReasonService;

        public WriteOffReasonPage(IWriteOffReasonService writeOffReasonService, IOptions<AppSettings> settings,
            IMapper mapper)
        {
            _writeOffReasonService = writeOffReasonService;
            _mapper = mapper;
            _settings = settings.Value;

            InitializeComponent();

            UpdateDataGrid();
        }

        public List<WriteOffReasonDTO> WriteOffReasonDtos { get; set; }

        public Task ActivateAsync(object parameter)
        {
            return Task.CompletedTask;
        }

        private void UpdateDataGrid()
        {
            WriteOffReasonDtos =
                _mapper.Map<List<WriteOffReason>, List<WriteOffReasonDTO>>(_writeOffReasonService.GetAll());

            WriteOffReasonDtos.Sort(delegate (WriteOffReasonDTO x, WriteOffReasonDTO y)
            {
                return x.Id.CompareTo(y.Id);
            });

            DataGrid.ItemsSource = WriteOffReasonDtos;
        }

        private bool ValidateForm()
        {
            if (!Regex.Match(DescriptionTextBox.Text, @"^\D{1,150}$").Success)
            {
                MessageBox.Show("Description must consist of at least 6 character and not exceed 150 characters!");
                DescriptionTextBox.Focus();
                return false;
            }

            return true;
        }

        private void CreateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;
            var writeOffReason = new WriteOffReason
            {
                Id = WriteOffReasonDtos[^1]?.Id + 1 ?? 1,
                Description = DescriptionTextBox.Text
            };

            _writeOffReasonService.Create(writeOffReason);
            UpdateDataGrid();
        }

        private void UpdateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            if (!ValidateForm()) return;
            var writeOffReason = new WriteOffReason
            {
                Id = WriteOffReasonDtos[DataGrid.SelectedIndex].Id,
                Description = DescriptionTextBox.Text
            };

            _writeOffReasonService.Update(writeOffReason);
            UpdateDataGrid();
        }

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedIndex != -1)
            {
                var writeOffReasonDto = WriteOffReasonDtos[DataGrid.SelectedIndex];
                DescriptionTextBox.Text = writeOffReasonDto.Description;
            }
        }

        private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1) return;
            _writeOffReasonService.Delete(WriteOffReasonDtos[DataGrid.SelectedIndex].Id);
            UpdateDataGrid();
        }
    }
}