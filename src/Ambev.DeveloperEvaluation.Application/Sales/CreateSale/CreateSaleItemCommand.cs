namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Command to create a sale item
/// </summary>
public class CreateSaleItemCommand
{
    /// <summary>The Product ID (external reference).</summary>
    public Guid ProductId { get; set; }

    /// <summary>Product Name/Description (denormalization).</summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>Item Quantity.</summary>
    public int Quantity { get; set; }

    /// <summary>Product Unit Price.</summary>
    public decimal UnitPrice { get; set; }
}