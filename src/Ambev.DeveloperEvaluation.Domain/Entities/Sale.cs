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
    /// Executes the total cancellation of the sale, setting the IsCancelled flag to true 
    /// and cascading the cancellation to all associated items.
    /// </summary>
    /// <exception cref="DomainException">Thrown if the sale is already cancelled.</exception>
    public void Cancel()
    {
        if (IsCancelled)
            throw new DomainException($"The sale {Number} is already cancelled.");

        foreach (var item in _items)
        {
            item.Cancel();
        }

        IsCancelled = true;
    }

    public void CancelItem(Guid itemId)
    {
        if (IsCancelled)
            throw new DomainException($"Cannot cancel items; Sale {Number} is already fully cancelled.");

        var itemToCancel = this.Items.FirstOrDefault(i => i.Id == itemId)
            ?? throw new DomainException($"Item with ID {itemId} not found in this sale.");

        itemToCancel.Cancel();

        TryCancelIfAllItemsCancelled();
    }

    /// <summary>
    /// Attempts to cancel the parent Sale if all line items in the collection are marked as cancelled.
    /// This method enforces the cascading business rule for partial cancellation.
    /// </summary>
    private void TryCancelIfAllItemsCancelled()
    {
        if (this.IsCancelled) return;

        if (this.Items.All(i => i.IsCancelled))
            this.IsCancelled = true;
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
