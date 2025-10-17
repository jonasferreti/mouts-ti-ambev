using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleCommand : IRequest<UpdateSaleResult>
{
    /// <summary>The sale ID to update</summary>
    public Guid Id { get; set; }

    /// <summary>The Customer ID (external reference).</summary>
    public Guid CustomerId { get; set; }

    /// <summary>Customer Name (denormalization).</summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>The Branch ID (external reference).</summary>
    public Guid BranchId { get; set; }

    /// <summary>Branch Name (denormalization).</summary>
    public string BranchName { get; set; } = string.Empty;

    /// <summary>List of items to be included in this sale.</summary>
    public List<UpdateSaleItemCommand> Items { get; set; } = [];
}
