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

            CreateMap<City, CityDTO>()
                .ForMember(dest => dest.CountryTitle, opts => opts.MapFrom(item => item.IdCountryNavigation.Title));

            CreateMap<Producer, ProducerDTO>()
                .ForMember(dest => dest.CountryTitle, opts => opts.MapFrom(item => item.IdCountryNavigation.Title));

            CreateMap<Provider, ProviderDTO>()
                .ForMember(dest => dest.CityTitle, opts => opts.MapFrom(item => item.IdCityNavigation.Title));

            CreateMap<Delivery, DeliveryDTO>()
                .ForMember(dest => dest.ProviderTitle,
                    opts => opts.MapFrom(item => item.IdProviderNavigation.CompanyTitle));

            CreateMap<Goods, GoodsDTO>()
                .ForMember(dest => dest.CategoryTitle, opts => opts.MapFrom(item => item.IdCategoryNavigation.Title))
                .ForMember(dest => dest.ProducerTitle, opts => opts.MapFrom(item => item.IdProducerNavigation.Title));

            CreateMap<Consignment, ConsignmentDTO>()
                .ForMember(dest => dest.GoodTitle, opts => opts.MapFrom(item => item.IdGoodsNavigation.Title))
                .ForMember(dest => dest.ProductCode, opts => opts.MapFrom(item => item.IdGoodsNavigation.ProductCode));

            CreateMap<DeliveryContents, DeliveryContentsDTO>()
                .ForMember(dest => dest.ConsignmentNumber,
                    opts => opts.MapFrom(item => item.IdConsignmentNavigation.ConsignmentNumber))
                .ForMember(dest => dest.DeliveryNumber,
                    opts => opts.MapFrom(item => item.IdDeliveryNavigation.DeliveryNumber));

            CreateMap<DeliveryShipment, DeliveryShipmentDTO>()
                .ForMember(dest => dest.GoodsTitle,
                    opts => opts.MapFrom(item => item.IdGoodsInMarketNavigation.IdGoodsNavigation.Title))
                .ForMember(dest => dest.ProductCode,
                    opts => opts.MapFrom(item => item.IdGoodsInMarketNavigation.IdGoodsNavigation.ProductCode))
                .ForMember(dest => dest.ConsignmentNumber,
                    opts => opts.MapFrom(item => item.IdConsignmentNavigation.ConsignmentNumber));

            CreateMap<WriteOffReason, WriteOffReasonDTO>();

            CreateMap<GoodsWriteOff, GoodsWriteOffDTO>()
                .ForMember(dest => dest.Login, opts => opts.MapFrom(item => item.IdEmployeeNavigation.Login))
                .ForMember(dest => dest.Reason,
                    opts => opts.MapFrom(item => item.IdWriteOffReasonNavigation.Description))
                .ForMember(dest => dest.ConsignmentNumber,
                    opts => opts.MapFrom(item =>
                        item.IdDeliveryShipmentNavigation.IdConsignmentNavigation.ConsignmentNumber))
                .ForMember(dest => dest.GoodTitle,
                    opts => opts.MapFrom(item => item.IdGoodsInMarketNavigation.IdGoodsNavigation.Title))
                .ForMember(dest => dest.ProductCode,
                    opts => opts.MapFrom(item => item.IdGoodsInMarketNavigation.IdGoodsNavigation.ProductCode));

            CreateMap<GoodsWriteOffOwn, GoodsWriteOffOwnDTO>()
                .ForMember(dest => dest.Login, opts => opts.MapFrom(item => item.IdEmployeeNavigation.Login))
                .ForMember(dest => dest.Reason,
                    opts => opts.MapFrom(item => item.IdWriteOffReasonNavigation.Description))
                .ForMember(dest => dest.GoodTitle,
                    opts => opts.MapFrom(item => item.IdProductionNavigation.IdGoodsOwnNavigation.Title))
                .ForMember(dest => dest.ProductCode,
                    opts => opts.MapFrom(item => item.IdProductionNavigation.IdGoodsOwnNavigation.ProductCode))
                .ForMember(dest => dest.ProductionCode,
                    opts => opts.MapFrom(item => item.IdProductionNavigation.ProductionCode));

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
                .ForMember(dest => dest.Login, opts => opts.MapFrom(item => item.IdEmployeeNavigation.Login));

            CreateMap<Role, RoleDTO>();

            CreateMap<Client, ClientDTO>().ForMember(dest => dest.CityTitle,
                opts => opts.MapFrom(item => item.IdCityNavigation.Title));

            CreateMap<Market, MarketDTO>().ForMember(dest => dest.CityTitle,
                opts => opts.MapFrom(item => item.IdCityNavigation.Title));

            CreateMap<Employee, EmployeeDTO>()
                .ForMember(dest => dest.RoleTitle, opts => opts.MapFrom(item => item.IdRoleNavigation.Title))
                .ForMember(dest => dest.CityTitle, opts => opts.MapFrom(item => item.IdCityNavigation.Title))
                .ForMember(dest => dest.MarketAddress, opts => opts.MapFrom(item => item.IdMarketNavigation.Address));

            CreateMap<GoodsInMarket, GoodsInMarketDTO>()
                .ForMember(dest => dest.GoodsTitle, opts => opts.MapFrom(item => item.IdGoodsNavigation.Title))
                .ForMember(dest => dest.ProductCode, opts => opts.MapFrom(item => item.IdGoodsNavigation.ProductCode))
                .ForMember(dest => dest.Address, opts => opts.MapFrom(item => item.IdMarketNavigation.Address));

            CreateMap<GoodsOwn, GoodsOwnDTO>().ForMember(dest => dest.Category,
                opts => opts.MapFrom(item => item.IdCategoryNavigation.Title));

            CreateMap<Production, ProductionDTO>()
                .ForMember(dest => dest.Login, opts => opts.MapFrom(item => item.IdEmployeeNavigation.Login))
                .ForMember(dest => dest.ProductCode,
                    opts => opts.MapFrom(item => item.IdGoodsOwnNavigation.ProductCode));

            CreateMap<GoodsInMarketOwn, GoodsInMarketOwnDTO>()
                .ForMember(dest => dest.GoodsTitle,
                    opts => opts.MapFrom(item => item.IdProductionNavigation.IdGoodsOwnNavigation.Title))
                .ForMember(dest => dest.Address, opts => opts.MapFrom(item => item.IdMarketNavigation.Address))
                .ForMember(dest => dest.ProductionCode,
                    opts => opts.MapFrom(item => item.IdProductionNavigation.ProductionCode))
                .ForMember(dest => dest.ProductCode, opts => opts.MapFrom(item => item.IdProductionNavigation.IdGoodsOwnNavigation.ProductCode));

            CreateMap<ProductionContents, ProductionContentsDTO>()
                .ForMember(dest => dest.ProductionCode,
                    opts => opts.MapFrom(item => item.IdProductionNavigation.ProductionCode))
                .ForMember(dest => dest.ProductCode,
                    opts => opts.MapFrom(item => item.IdGoodsInMarketNavigation.IdGoodsNavigation.ProductCode))
                .ForMember(dest => dest.Title,
                    opts => opts.MapFrom(item => item.IdGoodsInMarketNavigation.IdGoodsNavigation.Title));
        }
    }
}