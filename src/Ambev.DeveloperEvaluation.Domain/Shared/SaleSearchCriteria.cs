namespace Ambev.DeveloperEvaluation.Domain.Shared;

/// <summary>
/// Groups all filtering and ordering criteria for Sales.
/// </summary>
public record SaleSearchCriteria
{
    /// <summary>Sort by field</summary>
    public SaleSortField? SortField { get; init; }

    /// <summary>Sort by field direction</summary>
    public SortDirection? SortDirection { get; set; }

    /// <summary>Filter by Customer Name.</summary>
    public string? CustomerName { get; init; }

    /// <summary>Filter by Branch Name.</summary>
    public string? BranchName { get; init; }

    /// <summary>Filter by Product Name.</summary>
    public string? ProductName { get; init; }
}
