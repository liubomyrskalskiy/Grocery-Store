using GroceryStore.Core.Abstractions;
using GroceryStore.Core.Abstractions.Repositories;
using GroceryStore.DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GroceryStore.DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private IBasketOwnRepository _basketOwnRepository;
        private IBasketRepository _basketRepository;
        private ICategoryRepository _categoryRepository;
        private ICityRepository _cityRepository;
        private IClientRepository _clientRepository;
        private IConsignmentRepository _consignmentRepository;
        private GroceryStoreDbContext _context;
        private ICountryRepository _countryRepository;
        private IDeliveryContentsRepository _deliveryContentsRepository;
        private IDeliveryRepository _deliveryRepository;
        private IDeliveryShipmentRepository _deliveryShipmentRepository;
        private IEmployeeRepository _employeeRepository;
        private IGoodsInMarketOwnRepository _goodsInMarketOwnRepository;
        private IGoodsInMarketRepository _goodsInMarketRepository;
        private IGoodsOwnRepository _goodsOwnRepository;
        private IGoodsRepository _goodsRepository;
        private IGoodsWriteOffOwnRepository _goodsWriteOffOwnRepository;
        private IGoodsWriteOffRepository _goodsWriteOffRepository;
        private IMarketRepository _marketRepository;
        private IProducerRepository _producerRepository;
        private IProductionContentsRepository _productionContentsRepository;
        private IProductionRepository _productionRepository;
        private IProviderRepository _providerRepository;
        private IRoleRepository _roleRepository;
        private ISaleRepository _saleRepository;
        private IWriteOffReasonRepository _writeOffReasonRepository;

        public UnitOfWork(GroceryStoreDbContext context)
        {
            _context = context;
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public void SaveTablesChanges(string tableName)
        {
            _context.Database.BeginTransaction();
            _context.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT {tableName} ON");
            _context.SaveChanges();
            _context.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT {tableName} ON");
            _context.Database.CommitTransaction();
        }

        public void Dispose()
        {
            if (_context != null)
            {
                _context.Dispose();
                _context = null;
            }
        }

        public IBasketRepository BasketRepository
        {
            get { return _basketRepository ??= new BasketRepository(_context); }
        }

        public IBasketOwnRepository BasketOwnRepository
        {
            get { return _basketOwnRepository ??= new BasketOwnRepository(_context); }
        }

        public ICategoryRepository CategoryRepository
        {
            get { return _categoryRepository ??= new CategoryRepository(_context); }
        }

        public ICityRepository CityRepository
        {
            get { return _cityRepository ??= new CityRepository(_context); }
        }

        public IClientRepository ClientRepository
        {
            get { return _clientRepository ??= new ClientRepository(_context); }
        }

        public IConsignmentRepository ConsignmentRepository
        {
            get { return _consignmentRepository ??= new ConsignmentRepository(_context); }
        }

        public ICountryRepository CountryRepository
        {
            get { return _countryRepository ??= new CountryRepository(_context); }
        }

        public IDeliveryRepository DeliveryRepository
        {
            get { return _deliveryRepository ??= new DeliveryRepository(_context); }
        }

        public IDeliveryContentsRepository DeliveryContentsRepository
        {
            get { return _deliveryContentsRepository ??= new DeliveryContentsRepository(_context); }
        }

        public IDeliveryShipmentRepository DeliveryShipmentRepository
        {
            get { return _deliveryShipmentRepository ??= new DeliveryShipmentRepository(_context); }
        }

        public IEmployeeRepository EmployeeRepository
        {
            get { return _employeeRepository ??= new EmployeeRepository(_context); }
        }

        public IGoodsRepository GoodsRepository
        {
            get { return _goodsRepository ??= new GoodsRepository(_context); }
        }

        public IGoodsInMarketRepository GoodsInMarketRepository
        {
            get { return _goodsInMarketRepository ??= new GoodsInMarketRepository(_context); }
        }

        public IGoodsInMarketOwnRepository GoodsInMarketOwnRepository
        {
            get { return _goodsInMarketOwnRepository ??= new GoodsInMarketOwnRepository(_context); }
        }

        public IGoodsOwnRepository GoodsOwnRepository
        {
            get { return _goodsOwnRepository ??= new GoodsOwnRepository(_context); }
        }

        public IGoodsWriteOffRepository GoodsWriteOffRepository
        {
            get { return _goodsWriteOffRepository ??= new GoodsWriteOffRepository(_context); }
        }

        public IGoodsWriteOffOwnRepository GoodsWriteOffOwnRepository
        {
            get { return _goodsWriteOffOwnRepository ??= new GoodsWriteOffOwnRepository(_context); }
        }

        public IMarketRepository MarketRepository
        {
            get { return _marketRepository ??= new MarketRepository(_context); }
        }

        public IProducerRepository ProducerRepository
        {
            get { return _producerRepository ??= new ProducerRepository(_context); }
        }

        public IProductionRepository ProductionRepository
        {
            get { return _productionRepository ??= new ProductionRepository(_context); }
        }

        public IProductionContentsRepository ProductionContentsRepository
        {
            get { return _productionContentsRepository ??= new ProductionContentsRepository(_context); }
        }

        public IProviderRepository ProviderRepository
        {
            get { return _providerRepository ??= new ProviderRepository(_context); }
        }

        public IRoleRepository RoleRepository
        {
            get { return _roleRepository ??= new RoleRepository(_context); }
        }

        public ISaleRepository SaleRepository
        {
            get { return _saleRepository ??= new SaleRepository(_context); }
        }

        public IWriteOffReasonRepository WriteOffReasonRepository
        {
            get { return _writeOffReasonRepository ??= new WriteOffReasonRepository(_context); }
        }
    }
}