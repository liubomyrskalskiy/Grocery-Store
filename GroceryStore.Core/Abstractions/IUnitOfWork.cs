using System;
using GroceryStore.Core.Abstractions.Repositories;

namespace GroceryStore.Core.Abstractions
{
    public interface IUnitOfWork : IDisposable
    {
        IBasketRepository BasketRepository { get; }
        IBasketOwnRepository BasketOwnRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        ICityRepository CityRepository { get; }
        IClientRepository ClientRepository { get; }
        IConsignmentRepository ConsignmentRepository { get; }
        ICountryRepository CountryRepository { get; }
        IDeliveryRepository DeliveryRepository { get; }
        IDeliveryContentsRepository DeliveryContentsRepository { get; }
        IDeliveryShipmentRepository DeliveryShipmentRepository { get; }
        IEmployeeRepository EmployeeRepository { get; }
        IGoodsRepository GoodsRepository { get; }
        IGoodsInMarketRepository GoodsInMarketRepository { get; }
        IGoodsInMarketOwnRepository GoodsInMarketOwnRepository { get; }
        IGoodsOwnRepository GoodsOwnRepository { get; }
        IGoodsWriteOffRepository GoodsWriteOffRepository { get; }
        IGoodsWriteOffOwnRepository GoodsWriteOffOwnRepository { get; }
        IMarketRepository MarketRepository { get; }
        IProducerRepository ProducerRepository { get; }
        IProductionRepository ProductionRepository { get; }
        IProductionContentsRepository ProductionContentsRepository { get; }
        IProviderRepository ProviderRepository { get; }
        IRoleRepository RoleRepository { get; }
        ISaleRepository SaleRepository { get; }
        IWriteOffReasonRepository WriteOffReasonRepository { get; }

        void SaveChanges();

        void SaveTablesChanges(string tableName);
    }
}