using System;
using System.IO;
using System.Windows;
using AutoMapper;
using GroceryStore.Core.Abstractions;
using GroceryStore.Core.Abstractions.IServices;
using GroceryStore.Core.Mapping;
using GroceryStore.DAL;
using GroceryStore.Services;
using GroceryStore.Views;
using GroceryStore.Views.LessViews;
using GroceryStore.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GroceryStore
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }

        public IConfiguration Configuration { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();

            var navigationService = ServiceProvider
                .GetRequiredService<SimpleNavigationService>();
            var task = navigationService.ShowAsync<MainWindow>();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AppSettings>(Configuration.GetSection(nameof(AppSettings)));

            var connection = Configuration.GetConnectionString("SqlConnection");

            services.AddDbContext<GroceryStoreDbContext>(e => e.UseSqlServer(connection));

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IBasketService, BasketService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICityService, CityService>();
            services.AddScoped<IClientService, ClientService>();
            services.AddScoped<IConsignmentService, ConsignmentService>();
            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<IDeliveryContentsService, DeliveryContentsService>();
            services.AddScoped<IDeliveryService, DeliveryService>();
            services.AddScoped<IDeliveryShipmentService, DeliveryShipmentService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IGoodsInMarketService, GoodsInMarketService>();
            services.AddScoped<IGoodsService, GoodsService>();
            services.AddScoped<IGoodsWriteOffService, GoodsWriteOffService>();
            services.AddScoped<IBasketOwnService, BasketOwnService>();
            services.AddScoped<IGoodsInMarketOwnService, GoodsInMarketOwnService>();
            services.AddScoped<IGoodsOwnService, GoodsOwnService>();
            services.AddScoped<IGoodsWriteOffOwnService, GoodsWriteOffOwnService>();
            services.AddScoped<IConsignmentService, ConsignmentService>();
            services.AddScoped<IProducerService, ProducerService>();
            services.AddScoped<IProductionContentsService, ProductionContentsService>();
            services.AddScoped<IProductionService, ProductionService>();
            services.AddScoped<IProviderService, ProviderService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<ISaleService, SaleService>();
            services.AddScoped<IMarketService, MarketService>();
            services.AddScoped<IWriteOffReasonService, WriteOffReasonService>();

            services.AddScoped<SimpleNavigationService>();

            var mappingConfig = new MapperConfiguration(mc => { mc.AddProfile(new MappingProfile()); });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddTransient<MainWindow>();
            services.AddTransient<SaleWindow>();
            services.AddTransient<SaleDetailWindow>();
            services.AddTransient<ProductionWindow>();
            services.AddTransient<ProductionDetailWindow>();
            services.AddTransient<DeliveryOrderWindow>();
            services.AddTransient<DeliveryOrderUpdateWindow>();
            services.AddTransient<DeliveryOrderDetailWindow>();

            services.AddTransient<CategoryPage>();
            services.AddTransient<BasketPage>();
            services.AddTransient<CountryPage>();
            services.AddTransient<CityPage>();
            services.AddTransient<ProducerPage>();
            services.AddTransient<ProviderPage>();
            services.AddTransient<DeliveryPage>();
            services.AddTransient<GoodsPage>();
            services.AddTransient<ConsignmentPage>();
            services.AddTransient<DeliveryContentsPage>();
            services.AddTransient<DeliveryShipmentPage>();
            services.AddTransient<WriteOffReasonPage>();
            services.AddTransient<GoodsWriteOffPage>();
            services.AddTransient<GoodsWriteOffOwnPage>();
            services.AddTransient<BasketOwnPage>();
            services.AddTransient<SalePage>();
            services.AddTransient<RolesPage>();
            services.AddTransient<ClientPage>();
            services.AddTransient<MarketPage>();
            services.AddTransient<EmployeePage>();
            services.AddTransient<GoodsInMarketPage>();
            services.AddTransient<GoodsOwnPage>();
            services.AddTransient<ProductionPage>();
            services.AddTransient<GoodsInMarketOwnPage>();
            services.AddTransient<ProductionContentsPage>();

            services.AddTransient<GoodsInMarketOwnLessPage>();
            services.AddTransient<GoodsInMarketLessPage>();
            services.AddTransient<ProductionLessPage>();
            services.AddTransient<MarketLessPage>();
        }
    }
}