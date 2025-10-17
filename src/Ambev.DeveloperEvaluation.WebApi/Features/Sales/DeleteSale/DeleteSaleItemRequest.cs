namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.DeleteSale;

/// <summary>
/// Request model for deleting a sale item
/// </summary>
public class DeleteSaleItemRequest
{
    /// <summary>
    /// The unique identifier of the sale
    /// </summary>
    public Guid SaleId { get; set; }

    /// <summary>
    /// The unique identifier of the sale item to delete
    /// </summary>
    public Guid ItemId { get; set; }
}

