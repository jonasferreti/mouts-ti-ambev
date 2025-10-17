using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

/// <summary>
/// Query to retrieve a list of Sales with support for pagination (_page and _size).
/// </summary>
public record GetSalesQuery : IRequest<PaginatedList<GetSaleResult>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;

    public GetSalesCriteria Criteria { get; init; } = new();
}