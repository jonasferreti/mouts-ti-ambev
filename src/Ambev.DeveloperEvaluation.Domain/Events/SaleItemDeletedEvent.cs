
namespace Ambev.DeveloperEvaluation.Domain.Events;

/// <summary>
/// Domain Event that is dispatched when a new Sale Item is deleted
/// </summary>
/// <param name="SaleId">The unique identifier of the Sale.</param>
/// <param name="ItemId">The unique identifier of the deleted Sale Item.</param>
public record SaleItemDeletedEvent(Guid SaleId, Guid ItemId)
{
    public DateTime DateOccurred { get; } = DateTime.UtcNow;
}
