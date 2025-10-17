using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;

/// <summary>
/// API request model for retrieving a paginated list of Sales.
/// </summary>
public record GetSalesRequest
{
    /// <summary>Page number for pagination (default: 1)</summary>
    public int PageNumber { get; init; } = 1;

    /// <summary>Number of items per page (default: 10)</summary>
    public int PageSize { get; init; } = 10;

    /// <summary>Sale sort by field</summary>
    public string? SortField { get; init; }

    /// <summary>Sale sort by field direction</summary>
    public string? SortDirection { get; init; }

    /// <summary>Filter by CustomerName</summary>
    public string? CustomerName { get; init; }

    /// <summary>Filter by BranchName</summary>
    public string? BranchName { get; init; }

    /// <summary>Filter by ProductName</summary>
    public string? ProductName { get; init; }
}
