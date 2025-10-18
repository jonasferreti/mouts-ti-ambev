namespace Ambev.DeveloperEvaluation.Domain.Events;

/// <summary>
/// Domain Event that is dispatched when a new Sale is deleted
/// </summary>
/// <param name="SaleId">The unique identifier of the deleted Sale.</param>
public record SaleDeletedEvent(Guid SaleId) : IDomainEvent
{
    public DateTime DateOccurred { get; } = DateTime.UtcNow;
}
