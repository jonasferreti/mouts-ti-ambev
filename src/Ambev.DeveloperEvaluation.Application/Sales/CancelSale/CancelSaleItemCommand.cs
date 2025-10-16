using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

/// <summary>
/// Command to cancel a specific SaleItem within a Sale Aggregate Root.
/// </summary>
public record CancelSaleItemCommand(Guid SaleId, Guid ItemId) : IRequest;
