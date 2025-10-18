namespace Ambev.DeveloperEvaluation.Domain.Events;

public interface IDomainEvent
{
    public Guid SaleId { get; }
}
