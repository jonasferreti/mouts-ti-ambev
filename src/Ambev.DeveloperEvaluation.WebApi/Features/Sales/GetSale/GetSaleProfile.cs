using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.WebApi.Helpers;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;

/// <summary>
/// AutoMapper profile for mapping between Application Layer DTOs and API Layer DTOs.
/// </summary>
public class GetSaleProfile : Profile
{
    public GetSaleProfile()
    {
        CreateMap<GetSaleRequest, GetSaleQuery>();

        CreateMap<GetSalesRequest, GetSalesQuery>()
            .ForMember(dest => dest.Criteria, opt => opt.MapFrom(src => new GetSalesCriteria
            {
                SortField = MappingHelpers.ConvertToSaleSortField(src.SortField),
                SortDirection = MappingHelpers.ConvertToSortDirection(src.SortDirection),
                CustomerName = src.CustomerName,
                BranchName = src.BranchName,
                ProductName = src.ProductName
            }));

        // application -> api
        CreateMap<GetSaleItemResult, GetSaleItemResponse>();
        CreateMap<GetSaleResult, GetSaleResponse>();
    }
}