using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

/// <summary>
/// Configures mappings for Sale operations, enforcing DDD constructor invariants 
/// using ConstructUsing.
/// </summary>
public class UpdateSaleProfile : Profile
{
    public UpdateSaleProfile()
    {

        // Mapping from Command Item to SaleItem Entity
        CreateMap<UpdateSaleItemCommand, SaleItem>()
            .ConstructUsing(src => new SaleItem(
                new ExternalReference(src.ProductId, src.ProductName),
                new Quantity(src.Quantity),
                new Money(src.UnitPrice)
            ));

        // Mapping from Command to Sale Aggregate Root
        CreateMap<UpdateSaleCommand, Sale>()
            .ConstructUsing(src => new Sale(
                new ExternalReference(src.CustomerId, src.CustomerName),
                new ExternalReference(src.BranchId, src.BranchName),
                0)
            )
            .ForMember(dest => dest.Items, opt => opt.Ignore());


        // Mapping from SaleItem Entity to Result DTO
        CreateMap<SaleItem, UpdateSaleItemResult>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Product.Value))
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Description))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity.Value))
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice.Value))
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount.Value));

        // Mapping from Sale Aggregate to Result DTO
        CreateMap<Sale, UpdateSaleResult>()
           .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Customer.Value))
           .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Description))
           .ForMember(dest => dest.BranchId, opt => opt.MapFrom(src => src.Branch.Value))
           .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Branch.Description))
           .ForMember(dest => dest.TotalSaleAmount, opt => opt.MapFrom(src => src.TotalAmount.Value));
    }
}
