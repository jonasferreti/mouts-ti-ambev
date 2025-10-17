using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

/// <summary>
/// Query to retrieve a specific Sale by its unique ID.
/// </summary>
/// <param name="Id">The unique identifier of the Sale.</param>
public record GetSaleQuery(Guid Id) : IRequest<GetSaleResult>;
