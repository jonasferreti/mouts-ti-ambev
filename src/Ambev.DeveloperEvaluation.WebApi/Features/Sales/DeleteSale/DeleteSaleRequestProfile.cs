using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.DeleteSale;

/// <summary>
/// Profile for mapping DeleteSale feature requests to commands
/// </summary>
public class DeleteSaleRequestProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for DeleteSale feature
    /// </summary>
    public DeleteSaleRequestProfile()
    {
        CreateMap<DeleteSaleRequest, DeleteSaleCommand>();
        CreateMap<DeleteSaleItemRequest, DeleteSaleItemCommand>();

    }
}