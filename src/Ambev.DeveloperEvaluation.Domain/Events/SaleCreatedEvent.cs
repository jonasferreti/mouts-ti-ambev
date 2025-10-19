namespace Ambev.DeveloperEvaluation.Domain.Events;


/// <summary>
/// Domain Event that is dispatched when a new Sale is successfully created and persisted.
/// </summary>
/// <param name="SaleId">The unique identifier of the created Sale.</param>
/// <param name="CustomerId">The external identity of the Customer who made the purchase.</param>
/// <param name="BranchId">The external identity of the Branch where the sale occurred.</param>
/// <param name="TotalAmount">The final calculated amount of the sale.</param>
public record SaleCreatedEvent(
    Guid SaleId, Guid CustomerId,
    Guid BranchId, decimal TotalAmount) : IDomainEvent
{
    public DateTime DateOccurred { get; } = DateTime.UtcNow;
}