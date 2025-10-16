namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

/// <summary>
/// API DTO used to receive sale data for the POST /api/sales endpoint.
/// </summary>
public class CreateSaleRequest
{
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;  

    public List<CreateSaleItemRequest> Items { get; set; } = [];
}
