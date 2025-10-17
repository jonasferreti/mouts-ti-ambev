using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Shared;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

/// <summary>
/// Configures mappings for GetSale operations
/// </summary>
public class GetSaleProfile : Profile
{
    public GetSaleProfile()
    {
        // domain -> application
        CreateMap<SaleItem, GetSaleItemResult>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Product.Value))
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Description))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity.Value))
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice.Value))
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount.Value));

        CreateMap<Sale, GetSaleResult>()
            .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Customer.Value))
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Description))
            .ForMember(dest => dest.BranchId, opt => opt.MapFrom(src => src.Branch.Value))
            .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Branch.Description))
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount.Value));

        // application -> domain
        CreateMap<GetSalesCriteria, SaleSearchCriteria>();
    }
}
