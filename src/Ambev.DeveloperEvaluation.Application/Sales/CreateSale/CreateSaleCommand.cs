using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleCommand : IRequest<CreateSaleResult>
{
    /// <summary>The Customer ID (external reference, Integer type).</summary>
    public Guid CustomerId { get; set; }

    /// <summary>Customer Name (denormalization).</summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>The Branch ID (external reference, Guid type).</summary>
    public Guid BranchId { get; set; }

    /// <summary>Branch Name (denormalization).</summary>
    public string BranchName { get; set; } = string.Empty;

    /// <summary>List of items to be included in this sale.</summary>
    public List<CreateSaleItemCommand> Items { get; set; } = [];
}
