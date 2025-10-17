using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

/// <summary>
/// Response DTO returned to the client after a successful sale update.
/// </summary>
public class UpdateSaleResponse
{
    /// <summary>Local ID of the sale.</summary>
    public Guid Id { get; set; }

    /// <summary>The unique sale identification number.</summary>
    public long Number { get; set; }

    /// <summary>Date and time of the sale.</summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>Sale cancellation status.</summary>
    public bool IsCancelled { get; set; }

    /// <summary>External Customer ID.</summary>
    public Guid CustomerId { get; set; }

    /// <summary>Customer Name.</summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>External Branch ID.</summary>
    public Guid BranchId { get; set; }

    /// <summary>Branch Name.</summary>
    public string BranchName { get; set; } = string.Empty;

    /// <summary>Final total value of the sale.</summary>
    public decimal TotalSaleAmount { get; set; }

    /// <summary>List of items in this sale.</summary>
    public List<UpdateSaleItemResult> Items { get; set; } = [];
}
