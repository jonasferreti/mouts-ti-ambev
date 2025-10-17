namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

/// <summary>
/// Response model for Sale when retrieved via GetSale.
/// </summary>
public record GetSaleResult
{
    /// <summary>Local ID of the sale.</summary>
    public Guid Id { get; init; }

    /// <summary>The unique sale identification number.</summary>
    public long Number { get; init; }

    /// <summary>Date and time of the sale.</summary>
    public DateTime CreatedDate { get; init; }

    /// <summary>Sale cancellation status.</summary>
    public bool IsCancelled { get; init; }

    /// <summary>External Customer ID.</summary>
    public Guid CustomerId { get; init; }

    /// <summary>Customer Name.</summary>
    public string CustomerName { get; init; } = string.Empty;

    /// <summary>External Branch ID.</summary>
    public Guid BranchId { get; init; }

    /// <summary>Branch Name.</summary>
    public string BranchName { get; init; } = string.Empty;

    /// <summary>Final total value of the sale.</summary>
    public decimal TotalAmount { get; init; }

    /// <summary>List of items in this sale.</summary>
    public List<GetSaleItemResult> Items { get; init; } = [];
}
