
using Ambev.DeveloperEvaluation.Domain.Shared;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

/// <summary>
/// Groups all filtering and ordering criteria for Sale queries.
/// </summary>
public record GetSalesCriteria
{
    /// <summary>Sale sort by field</summary>
    public SaleSortField? SortField { get; init; }

    /// <summary>Sale sort by field direction</summary>
    public SortDirection? SortDirection { get; init; }

    /// <summary>Filter by CustomerName</summary>
    public string? CustomerName { get; init; }

    /// <summary>Filter by BranchName</summary>
    public string? BranchName { get; init; }

    /// <summary>Filter by ProductName</summary>
    public string? ProductName { get; init; }
}
