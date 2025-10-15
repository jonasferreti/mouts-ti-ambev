using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// The Aggregate Root for the Sales Domain.
/// It ensures the transactional consistency of all its SaleItems.
/// </summary>
public class Sale : BaseEntity
{
    /// <summary>External reference to the Customer.</summary>
    public ExternalReference Customer { get; private set; } = null!;

    /// <summary>External reference to the Branch.</summary>
    public ExternalReference Branch { get; private set; } = null!;

    /// <summary>Final total value of the sale (sum of all item totals).</summary>
    public Money TotalAmount { get; private set; } = null!;

    /// <summary>Date and time the sale was registered.</summary>
    public DateTime CreatedDate { get; private set; }

    /// <summary>Unique identification number of the sale.</summary>
    public long Number { get; private set; }

    /// <summary>Indicates whether the sale has been cancelled.</summary>
    public bool IsCancelled { get; private set; }

    private readonly List<SaleItem> _items = [];

    /// <summary>Collection of sale items (read-only access).</summary>
    public IReadOnlyCollection<SaleItem> Items => _items.AsReadOnly();

    protected Sale() { }

    public Sale(ExternalReference customer, ExternalReference branch, long number)
    {
        Customer = customer;
        Branch = branch;
        Number = number;
        CreatedDate = DateTime.UtcNow;
        IsCancelled = false;
    }

    /// <summary>
    /// Adds an item to the sale and recalculates the TotalAmount.
    /// </summary>
    /// <param name="item">The sale item to be added.</param>
    public void AddItem(SaleItem item)
    {
        _items.Add(item);
        CalculateTotalAmount();
    }

    /// <summary>
    /// Marks the sale as cancelled, respecting the state invariant.
    /// </summary>
    public void Cancel()
    {
        if (IsCancelled)
            throw new DomainException("The sale is already cancelled.");

        IsCancelled = true;
    }

    /// <summary>
    /// Calcules sale TotalAmount (sum of all item totals).
    /// </summary>
    private void CalculateTotalAmount()
    {
        var total = _items.Sum(i => i.TotalAmount.Value);
        TotalAmount = new Money(total);
    }
}
