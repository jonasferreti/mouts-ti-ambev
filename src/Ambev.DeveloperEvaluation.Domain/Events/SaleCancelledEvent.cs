namespace Ambev.DeveloperEvaluation.Domain.Events;


/// <summary>
/// Domain Event that is dispatched when a new Sale is cancelled
/// </summary>
/// <param name="SaleId">The unique identifier of the cancelled Sale.</param>
public record SaleCancelledEvent(Guid SaleId) : IDomainEvent
{
    public DateTime DateOccurred { get; } = DateTime.UtcNow;
}