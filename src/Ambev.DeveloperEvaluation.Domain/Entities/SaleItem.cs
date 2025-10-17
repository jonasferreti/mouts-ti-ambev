using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents an individual item within a Sale.
/// This entity is responsible for applying the discount logic based on Quantity.
/// </summary>
public class SaleItem : BaseEntity
{
    private const int minQuantityForDiscount = 4;
    private const int minQuantityForBetterDiscount = 10;

    /// <summary>External reference to the Product.</summary>
    public ExternalReference Product { get; private set; } = null!;

    /// <summary>Quantity of the item.</summary>
    public Quantity Quantity { get; private set; } = null!;

    /// <summary>Unit price of the product at the time of sale.</summary>
    public Money UnitPrice { get; private set; } = null!;

    /// <summary>Total amount of the item after discount is applied.</summary>
    public Money TotalAmount { get; private set; } = null!;

    /// <summary>Percentage discount applied</summary>
    public decimal DiscountPercentage { get; private set; }

    /// <summary>Indicates whether the sale item has been cancelled.</summary>
    public bool IsCancelled { get; private set; }

    protected SaleItem() { }

    public SaleItem(ExternalReference product, Quantity quantity, Money unitPrice)
    {
        Product = product;
        Quantity = quantity;
        UnitPrice = unitPrice;
        IsCancelled = false;

        CalculateTotal();
    }

    /// <summary>
    /// Applies the business discount rules based on quantity and calculates the item's total amount.
    /// </summary>
    public void CalculateTotal()
    {
        DiscountPercentage = CalculateDiscountPercentage();

        var total = Quantity.Value * UnitPrice.Value;
        var discount = total * DiscountPercentage;

        TotalAmount = new Money(total - discount);
    }

    /// <summary>
    /// Calculates the business discount rules based on quantity.
    /// </summary>
    private decimal CalculateDiscountPercentage()
    {
        if (Quantity.Value >= minQuantityForBetterDiscount)
            return 0.20M;

        if (Quantity.Value >= minQuantityForDiscount)
            return 0.10M;

        return 0M;
    }

    /// <summary>
    /// Executes the cancellation of this specific sale item, enforcing the item-level invariant.
    /// </summary>
    /// <exception cref="DomainException">Thrown if the item is already cancelled.</exception>
    public void Cancel()
    {
        if (this.IsCancelled)
            throw new DomainException($"The item {this.Product.Description} is already cancelled.");

        this.IsCancelled = true;
    }
}
