using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;

namespace Ambev.DeveloperEvaluation.Domain.Extensions;

public static class SaleExtensions
{
    public static SaleCreatedEvent SaleCreatedEvent(this Sale sale)
    {
        return new SaleCreatedEvent(
            sale.Id,
            sale.Customer.Value,
            sale.Branch.Value,
            sale.TotalAmount.Value
        );
    }

    public static SaleCancelledEvent SaleCancelledEvent(this Sale sale)
    {
        return new SaleCancelledEvent(sale.Id);
    }

    public static SaleItemCancelledEvent SaleItemCancelledEvent(this Sale sale, Guid itemId)
    {
        return new SaleItemCancelledEvent(sale.Id, itemId);
    }

    public static SaleModifiedEvent SaleModifiedEvent(this Sale sale)
    {
        return new SaleModifiedEvent(
            sale.Id,
            sale.Customer.Value,
            sale.Branch.Value,
            sale.TotalAmount.Value
        );
    }

    public static SaleDeletedEvent SaleDeletedEvent(this Sale sale)
    {
        return new SaleDeletedEvent(sale.Id);
    }

    public static SaleItemDeletedEvent SaleItemDeletedEvent(this Sale sale, Guid itemId)
    {
        return new SaleItemDeletedEvent(sale.Id, itemId);
    }
}
