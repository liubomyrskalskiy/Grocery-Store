using AutoMapper;
using GroceryStore.Core.DTO;
using GroceryStore.Core.Models;

namespace GroceryStore.Core.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Category, CategoryDTO>().ReverseMap();

            CreateMap<Country, CountryDTO>().ReverseMap();

            CreateMap<Basket, BasketDTO>()
                .ForMember(dest => dest.CheckNumber, opts => opts.MapFrom(item => item.IdSaleNavigation.CheckNumber))
                .ForMember(dest => dest.ProductCode,
                    opts => opts.MapFrom(item => item.IdGoodsInMarketNavigation.IdGoodsNavigation.ProductCode))
                .ForMember(dest => dest.Title,
                    opts => opts.MapFrom(item => item.IdGoodsInMarketNavigation.IdGoodsNavigation.Title));

            CreateMap<Basket, UniversalBasketDTO>()
                .ForMember(dest => dest.Title,
                    opts => opts.MapFrom(item => item.IdGoodsInMarketNavigation.IdGoodsNavigation.Title))
                .ForMember(dest => dest.FullTitle,
                    opts => opts.MapFrom(item =>
                        $"{item.IdGoodsInMarketNavigation.IdGoodsNavigation.IdProducerNavigation.Title} {item.IdGoodsInMarketNavigation.IdGoodsNavigation.Title}"))
                .ForMember(dest => dest.ProductCode,
                    opts => opts.MapFrom(item => item.IdGoodsInMarketNavigation.IdGoodsNavigation.ProductCode))
                .ForMember(dest => dest.Price,
                    opts => opts.MapFrom(item =>
                        $"{item.IdGoodsInMarketNavigation.IdGoodsNavigation.Price * item.Amount,0:C2}"))
                .ForMember(dest => dest.CheckNumber, opts => opts.MapFrom(item => item.IdSaleNavigation.CheckNumber))
                .ForMember(dest => dest.Producer,
                    opts => opts.MapFrom(item =>
                        item.IdGoodsInMarketNavigation.IdGoodsNavigation.IdProducerNavigation.Title))
                .BeforeMap((s, d) => d.IsOwn = false);

            CreateMap<BasketOwn, UniversalBasketDTO>()
                .ForMember(dest => dest.Title,
                    opts => opts.MapFrom(item =>
                        item.IdGoodsInMarketOwnNavigation.IdProductionNavigation.IdGoodsOwnNavigation.Title))
                .ForMember(dest => dest.FullTitle,
                    opts => opts.MapFrom(item =>
                        item.IdGoodsInMarketOwnNavigation.IdProductionNavigation.IdGoodsOwnNavigation.Title))
                .ForMember(dest => dest.ProductCode,
                    opts => opts.MapFrom(item =>
                        item.IdGoodsInMarketOwnNavigation.IdProductionNavigation.IdGoodsOwnNavigation.ProductCode))
                .ForMember(dest => dest.Price,
                    opts => opts.MapFrom(item =>
                        $"{item.IdGoodsInMarketOwnNavigation.IdProductionNavigation.IdGoodsOwnNavigation.Price * item.Amount,0:C2}"))
                .ForMember(dest => dest.CheckNumber, opts => opts.MapFrom(item => item.IdSaleNavigation.CheckNumber))
                .BeforeMap((s, d) => d.Producer = "")
                .BeforeMap((s, d) => d.IsOwn = true);

            CreateMap<City, CityDTO>()
                .ForMember(dest => dest.CountryTitle, opts => opts.MapFrom(item => item.IdCountryNavigation.Title))
                .ForMember(dest => dest.FullTitle,
                    opts => opts.MapFrom(item => $"{item.IdCountryNavigation.Title} {item.Title}"));

            CreateMap<Producer, ProducerDTO>()
                .ForMember(dest => dest.CountryTitle, opts => opts.MapFrom(item => item.IdCountryNavigation.Title));

            CreateMap<Provider, ProviderDTO>()
                .ForMember(dest => dest.CityTitle, opts => opts.MapFrom(item => item.IdCityNavigation.Title))
                .ForMember(dest => dest.FullAddress,
                    opts => opts.MapFrom(item =>
                        $"{item.IdCityNavigation.IdCountryNavigation.Title} {item.IdCityNavigation.Title} {item.Address}"));

            CreateMap<Delivery, DeliveryDTO>()
                .ForMember(dest => dest.ProviderTitle,
                    opts => opts.MapFrom(item => item.IdProviderNavigation.CompanyTitle))
                .ForMember(dest => dest.ContactPerson,
                    opts => opts.MapFrom(item => item.IdProviderNavigation.ContactPerson))
                .ForMember(dest => dest.PhoneNumber,
                    opts => opts.MapFrom(item => item.IdProviderNavigation.PhoneNumber));

            CreateMap<Goods, GoodsDTO>()
                .ForMember(dest => dest.CategoryTitle, opts => opts.MapFrom(item => item.IdCategoryNavigation.Title))
                .ForMember(dest => dest.ProducerTitle, opts => opts.MapFrom(item => item.IdProducerNavigation.Title))
                .ForMember(dest => dest.StringWeight, opts => opts.MapFrom(item => $"{item.Weight,0:0.00} kg"))
                .ForMember(dest => dest.StringPrice, opts => opts.MapFrom(item => $"{item.Price,0:C2}"))
                .ForMember(dest => dest.FullName, opts => opts.MapFrom(item => item.ProductCode + " " + item.Title));

            CreateMap<Consignment, ConsignmentDTO>()
                .ForMember(dest => dest.GoodTitle, opts => opts.MapFrom(item => item.IdGoodsNavigation.Title))
                .ForMember(dest => dest.ProductCode, opts => opts.MapFrom(item => item.IdGoodsNavigation.ProductCode))
                .ForMember(dest => dest.StringAmount, opts => opts.MapFrom(item => $"{item.Amount,0:0.000}"))
                .ForMember(dest => dest.StringIncomePrice, opts => opts.MapFrom(item => $"{item.IncomePrice,0:C2}"))
                .ForMember(dest => dest.Category,
                    opts => opts.MapFrom(item => item.IdGoodsNavigation.IdCategoryNavigation.Title))
                .ForMember(dest => dest.Producer,
                    opts => opts.MapFrom(item => item.IdGoodsNavigation.IdProducerNavigation.Title));

            CreateMap<DeliveryContents, DeliveryContentsDTO>()
                .ForMember(dest => dest.ProductCode,
                    opts => opts.MapFrom(item => item.IdConsignmentNavigation.IdGoodsNavigation.ProductCode))
                .ForMember(dest => dest.GoodTitle,
                    opts => opts.MapFrom(item => item.IdConsignmentNavigation.IdGoodsNavigation.Title))
                .ForMember(dest => dest.ProducerTitle,
                    opts => opts.MapFrom(item =>
                        item.IdConsignmentNavigation.IdGoodsNavigation.IdProducerNavigation.Title))
                .ForMember(dest => dest.DeliveryNumber,
                    opts => opts.MapFrom(item => item.IdDeliveryNavigation.DeliveryNumber))
                .ForMember(dest => dest.ProviderTitle,
                    opts => opts.MapFrom(item => item.IdDeliveryNavigation.IdProviderNavigation.CompanyTitle))
                .ForMember(dest => dest.ContactPerson,
                    opts => opts.MapFrom(item => item.IdDeliveryNavigation.IdProviderNavigation.ContactPerson))
                .ForMember(dest => dest.PhoneNumber,
                    opts => opts.MapFrom(item => item.IdDeliveryNavigation.IdProviderNavigation.PhoneNumber))
                .ForMember(dest => dest.OrderDate, opts => opts.MapFrom(item => item.IdDeliveryNavigation.DeliveryDate))
                .ForMember(dest => dest.ConsignmentNumber,
                    opts => opts.MapFrom(item => item.IdConsignmentNavigation.ConsignmentNumber))
                .ForMember(dest => dest.ManufactureDate,
                    opts => opts.MapFrom(item => item.IdConsignmentNavigation.ManufactureDate))
                .ForMember(dest => dest.BestBefore,
                    opts => opts.MapFrom(item => item.IdConsignmentNavigation.BestBefore))
                .ForMember(dest => dest.OrderAmount, opts => opts.MapFrom(item => item.IdConsignmentNavigation.Amount))
                .ForMember(dest => dest.StringOrderAmount,
                    opts => opts.MapFrom(item => $"{item.IdConsignmentNavigation.Amount,0:0.000}"))
                .ForMember(dest => dest.IncomePrice,
                    opts => opts.MapFrom(item => item.IdConsignmentNavigation.IncomePrice))
                .ForMember(dest => dest.StringIncomePrice,
                    opts => opts.MapFrom(item => $"{item.IdConsignmentNavigation.IncomePrice,0:C2}"));

            CreateMap<DeliveryShipment, DeliveryShipmentDTO>()
                .ForMember(dest => dest.GoodsTitle,
                    opts => opts.MapFrom(item => item.IdGoodsInMarketNavigation.IdGoodsNavigation.Title))
                .ForMember(dest => dest.ProductCode,
                    opts => opts.MapFrom(item => item.IdGoodsInMarketNavigation.IdGoodsNavigation.ProductCode))
                .ForMember(dest => dest.ConsignmentNumber,
                    opts => opts.MapFrom(item => item.IdConsignmentNavigation.ConsignmentNumber))
                .ForMember(dest => dest.StringAmount, opts => opts.MapFrom(item => $"{item.Amount,0:0.000}"))
                .ForMember(dest => dest.Address,
                    opts => opts.MapFrom(item =>
                        $"{item.IdGoodsInMarketNavigation.IdMarketNavigation.IdCityNavigation.Title} {item.IdGoodsInMarketNavigation.IdMarketNavigation.Address}"));

            CreateMap<WriteOffReason, WriteOffReasonDTO>();

            CreateMap<GoodsWriteOff, GoodsWriteOffDTO>()
                .ForMember(dest => dest.Login, opts => opts.MapFrom(item => item.IdEmployeeNavigation.Login))
                .ForMember(dest => dest.FullName,
                    opts => opts.MapFrom(item =>
                        $"{item.IdEmployeeNavigation.LastName} {item.IdEmployeeNavigation.FirstName}"))
                .ForMember(dest => dest.Reason,
                    opts => opts.MapFrom(item => item.IdWriteOffReasonNavigation.Description))
                .ForMember(dest => dest.ConsignmentNumber,
                    opts => opts.MapFrom(item =>
                        item.IdDeliveryShipmentNavigation.IdConsignmentNavigation.ConsignmentNumber))
                .ForMember(dest => dest.GoodTitle,
                    opts => opts.MapFrom(item => item.IdGoodsInMarketNavigation.IdGoodsNavigation.Title))
                .ForMember(dest => dest.ProductCode,
                    opts => opts.MapFrom(item => item.IdGoodsInMarketNavigation.IdGoodsNavigation.ProductCode))
                .ForMember(dest => dest.ShipmentDateTime,
                    opts => opts.MapFrom(item => item.IdDeliveryShipmentNavigation.ShipmentDateTime));

            CreateMap<GoodsWriteOffOwn, GoodsWriteOffOwnDTO>()
                .ForMember(dest => dest.Login, opts => opts.MapFrom(item => item.IdEmployeeNavigation.Login))
                .ForMember(dest => dest.FullName,
                    opts => opts.MapFrom(item =>
                        $"{item.IdEmployeeNavigation.LastName} {item.IdEmployeeNavigation.FirstName}"))
                .ForMember(dest => dest.Reason,
                    opts => opts.MapFrom(item => item.IdWriteOffReasonNavigation.Description))
                .ForMember(dest => dest.GoodTitle,
                    opts => opts.MapFrom(item => item.IdProductionNavigation.IdGoodsOwnNavigation.Title))
                .ForMember(dest => dest.ProductCode,
                    opts => opts.MapFrom(item => item.IdProductionNavigation.IdGoodsOwnNavigation.ProductCode))
                .ForMember(dest => dest.ProductionCode,
                    opts => opts.MapFrom(item => item.IdProductionNavigation.ProductionCode))
                .ForMember(dest => dest.ManufactureDate,
                    opts => opts.MapFrom(item => item.IdProductionNavigation.ManufactureDate));

            CreateMap<BasketOwn, BasketOwnDTO>()
                .ForMember(dest => dest.CheckNumber, opts => opts.MapFrom(item => item.IdSaleNavigation.CheckNumber))
                .ForMember(dest => dest.ProductCode,
                    opts => opts.MapFrom(item =>
                        item.IdGoodsInMarketOwnNavigation.IdProductionNavigation.IdGoodsOwnNavigation.ProductCode))
                .ForMember(dest => dest.Title,
                    opts => opts.MapFrom(item =>
                        item.IdGoodsInMarketOwnNavigation.IdProductionNavigation.IdGoodsOwnNavigation.Title));

            CreateMap<Sale, SaleDTO>()
                .ForMember(dest => dest.AccountNumber,
                    opts => opts.MapFrom(item => item.IdClientNavigation.AccountNumber))
                .ForMember(dest => dest.Login, opts => opts.MapFrom(item => item.IdEmployeeNavigation.Login))
                .ForMember(dest => dest.FullName,
                    opts => opts.MapFrom(item =>
                        $"{item.IdEmployeeNavigation.LastName} {item.IdEmployeeNavigation.FirstName}"))
                .ForMember(dest => dest.Total, opts => opts.MapFrom(item => $"{item.Total,0:C2}"))
                .ForMember(dest => dest.FullMarketAddress,
                    opts => opts.MapFrom(item =>
                        $"{item.IdEmployeeNavigation.IdMarketNavigation.IdCityNavigation.Title} {item.IdEmployeeNavigation.IdMarketNavigation.Address}"));

            CreateMap<Role, RoleDTO>();

            CreateMap<Client, ClientDTO>()
                .ForMember(dest => dest.CityTitle, opts => opts.MapFrom(item => item.IdCityNavigation.Title))
                .ForMember(dest => dest.FullName, opts => opts.MapFrom(item => $"{item.LastName} {item.FirstName}"))
                .ForMember(dest => dest.FullAddress,
                    opts => opts.MapFrom(item =>
                        $"{item.IdCityNavigation.IdCountryNavigation.Title} {item.IdCityNavigation.Title} {item.Address}"));

            CreateMap<Market, MarketDTO>()
                .ForMember(dest => dest.CityTitle, opts => opts.MapFrom(item => item.IdCityNavigation.Title))
                .ForMember(dest => dest.FullAddress,
                    opts => opts.MapFrom(item => $"{item.IdCityNavigation.Title} {item.Address}"));

            CreateMap<Employee, EmployeeDTO>()
                .ForMember(dest => dest.RoleTitle, opts => opts.MapFrom(item => item.IdRoleNavigation.Title))
                .ForMember(dest => dest.CityTitle, opts => opts.MapFrom(item => item.IdCityNavigation.Title))
                .ForMember(dest => dest.MarketAddress, opts => opts.MapFrom(item => item.IdMarketNavigation.Address))
                .ForMember(dest => dest.FullMarketAddress,
                    opts => opts.MapFrom(item =>
                        $"{item.IdMarketNavigation.IdCityNavigation.Title} {item.IdMarketNavigation.Address}"))
                .ForMember(dest => dest.FullAddress,
                    opts => opts.MapFrom(item =>
                        $"{item.IdCityNavigation.IdCountryNavigation.Title} {item.IdCityNavigation.Title} {item.Address}"))
                .ForMember(dest => dest.FullName, opts => opts.MapFrom(item => $"{item.LastName} {item.FirstName}"));

            CreateMap<GoodsInMarket, GoodsInMarketDTO>()
                .ForMember(dest => dest.Good, opts => opts.MapFrom(item => item.IdGoodsNavigation.Title))
                .ForMember(dest => dest.ProductCode, opts => opts.MapFrom(item => item.IdGoodsNavigation.ProductCode))
                .ForMember(dest => dest.MarketAddress, opts => opts.MapFrom(item => item.IdMarketNavigation.Address))
                .ForMember(dest => dest.FullMarketAddress,
                    opts => opts.MapFrom(item =>
                        $"{item.IdMarketNavigation.IdCityNavigation.Title} {item.IdMarketNavigation.Address}"))
                .ForMember(dest => dest.Producer,
                    opts => opts.MapFrom(item => item.IdGoodsNavigation.IdProducerNavigation.Title))
                .ForMember(dest => dest.Price, opts => opts.MapFrom(item => $"{item.IdGoodsNavigation.Price,0:C2}"))
                .ForMember(dest => dest.Amount, opts => opts.MapFrom(item => $"{item.Amount,0:0.000}"))
                .ForMember(dest => dest.Weight,
                    opts => opts.MapFrom(item => $"{item.IdGoodsNavigation.Weight,0:0.000} kg"))
                .ForMember(dest => dest.Category,
                    opts => opts.MapFrom(item => item.IdGoodsNavigation.IdCategoryNavigation.Title));

            CreateMap<GoodsOwn, GoodsOwnDTO>().ForMember(dest => dest.Category,
                    opts => opts.MapFrom(item => item.IdCategoryNavigation.Title))
                .ForMember(dest => dest.StringWeight, opts => opts.MapFrom(item => $"{item.Weight,0:0.000} kg"))
                .ForMember(dest => dest.StringPrice, opts => opts.MapFrom(item => $"{item.Price,0:C2}"));

            CreateMap<Production, ProductionDTO>()
                .ForMember(dest => dest.Login, opts => opts.MapFrom(item => item.IdEmployeeNavigation.Login))
                .ForMember(dest => dest.FullName,
                    opts => opts.MapFrom(item =>
                        $"{item.IdEmployeeNavigation.LastName} {item.IdEmployeeNavigation.FirstName}"))
                .ForMember(dest => dest.ProductCode,
                    opts => opts.MapFrom(item => item.IdGoodsOwnNavigation.ProductCode))
                .ForMember(dest => dest.Title, opts => opts.MapFrom(item => item.IdGoodsOwnNavigation.Title))
                .ForMember(dest => dest.Category,
                    opts => opts.MapFrom(item => item.IdGoodsOwnNavigation.IdCategoryNavigation.Title))
                .ForMember(dest => dest.StringTotalCost, opts => opts.MapFrom(item => $"{item.TotalCost,0:C2}"));

            CreateMap<GoodsInMarketOwn, GoodsInMarketOwnDTO>()
                .ForMember(dest => dest.Good,
                    opts => opts.MapFrom(item => item.IdProductionNavigation.IdGoodsOwnNavigation.Title))
                .ForMember(dest => dest.Address, opts => opts.MapFrom(item => item.IdMarketNavigation.Address))
                .ForMember(dest => dest.FullAddress,
                    opts => opts.MapFrom(item =>
                        $"{item.IdMarketNavigation.IdCityNavigation.Title} {item.IdMarketNavigation.Address}"))
                .ForMember(dest => dest.ProductionCode,
                    opts => opts.MapFrom(item => item.IdProductionNavigation.ProductionCode))
                .ForMember(dest => dest.ProductCode,
                    opts => opts.MapFrom(item => item.IdProductionNavigation.IdGoodsOwnNavigation.ProductCode))
                .ForMember(dest => dest.ManufactureDate,
                    opts => opts.MapFrom(item => item.IdProductionNavigation.ManufactureDate))
                .ForMember(dest => dest.Price,
                    opts => opts.MapFrom(item => $"{item.IdProductionNavigation.IdGoodsOwnNavigation.Price,0:C2}"))
                .ForMember(dest => dest.Amount, opts => opts.MapFrom(item => $"{item.Amount,0:0.000}"))
                .ForMember(dest => dest.DoubleAmount, opts => opts.MapFrom(item => item.Amount))
                .ForMember(dest => dest.Category,
                    opts => opts.MapFrom(item =>
                        item.IdProductionNavigation.IdGoodsOwnNavigation.IdCategoryNavigation.Title))
                .ForMember(dest => dest.Weight,
                    opts => opts.MapFrom(
                        item => $"{item.IdProductionNavigation.IdGoodsOwnNavigation.Weight,0:0.000} kg"));

            CreateMap<ProductionContents, ProductionContentsDTO>()
                .ForMember(dest => dest.ProductionCode,
                    opts => opts.MapFrom(item => item.IdProductionNavigation.ProductionCode))
                .ForMember(dest => dest.ProductCode,
                    opts => opts.MapFrom(item => item.IdGoodsInMarketNavigation.IdGoodsNavigation.ProductCode))
                .ForMember(dest => dest.Category,
                    opts => opts.MapFrom(item =>
                        item.IdGoodsInMarketNavigation.IdGoodsNavigation.IdCategoryNavigation.Title))
                .ForMember(dest => dest.Producer,
                    opts => opts.MapFrom(item =>
                        item.IdGoodsInMarketNavigation.IdGoodsNavigation.IdProducerNavigation.Title))
                .ForMember(dest => dest.Title,
                    opts => opts.MapFrom(item => item.IdGoodsInMarketNavigation.IdGoodsNavigation.Title))
                .ForMember(dest => dest.Price,
                    opts => opts.MapFrom(item => item.IdGoodsInMarketNavigation.IdGoodsNavigation.Price * item.Amount))
                .ForMember(dest => dest.StringPrice,
                    opts => opts.MapFrom(item =>
                        $"{item.IdGoodsInMarketNavigation.IdGoodsNavigation.Price * item.Amount,0:C2}"));
        }
    }
}