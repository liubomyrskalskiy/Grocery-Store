using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GroceryStore.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace GroceryStore
{
    public class SimpleNavigationService
    {
        private readonly IServiceProvider _serviceProvider;

        public SimpleNavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task ShowAsync<T>(object parameter = null) where T : Window
        {
            var window = _serviceProvider.GetRequiredService<T>();
            if (window is IActivable activableWindow) await activableWindow.ActivateAsync(parameter);

            window.Show();
        }

        public async Task<Page> GetPageAsync<T>(object parameter = null) where T : Page
        {
            var page = _serviceProvider.GetRequiredService<T>();
            if (page is IActivable activableWindow) await activableWindow.ActivateAsync(parameter);

            return page;
        }

        public async Task<bool?> ShowDialogAsync<T>(object parameter = null)
            where T : Window
        {
            var window = _serviceProvider.GetRequiredService<T>();
            if (window is IActivable activableWindow) await activableWindow.ActivateAsync(parameter);

            return window.ShowDialog();
        }
    }
}