using System.Text.Json.Serialization;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

/// <summary>
/// API DTO used to receive sale data for the update sale endpoint.
/// </summary>
public class UpdateSaleRequest
{
    [JsonIgnore]
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;  

    public List<UpdateSaleItemRequest> Items { get; set; } = [];
}
