namespace Ambev.DeveloperEvaluation.Domain.Events;

/// <summary>
/// Domain Event that is dispatched when a UpdateSale is successfully executed
/// </summary>
/// <param name="SaleId">The unique identifier of the updated Sale.</param>
/// <param name="CustomerId">The external identity of the Customer who made the purchase.</param>
/// <param name="BranchId">The external identity of the Branch where the sale occurred.</param>
/// <param name="TotalAmount">The final calculated amount of the sale.</param>
public record SaleModifiedEvent(
    Guid SaleId, Guid CustomerId,
    Guid BranchId, decimal TotalAmount) : IDomainEvent
{
    public DateTime DateOccurred { get; } = DateTime.UtcNow;
}