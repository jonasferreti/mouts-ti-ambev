namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Response model for CreateSale item operation
/// </summary>
public class CreateSaleItemResult
{
    /// <summary>Id of the sale item.</summary>
    public Guid Id { get; set; }

    /// <summary>External Product ID.</summary>
    public Guid ProductId { get; set; }

    /// <summary>Denormalized Product Name.</summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>Quantity sold.</summary>
    public int Quantity { get; set; }

    /// <summary>Unit price at the time of sale.</summary>
    public decimal UnitPrice { get; set; }

    /// <summary>Percentage discount applied.</summary>
    public decimal DiscountPercentage { get; set; }

    /// <summary>Total amount of the item after discount.</summary>
    public decimal TotalAmount { get; set; }

    /// <summary>Indicates whether the sale item has been cancelled.</summary>
    public bool IsCancelled { get; set; }
}
