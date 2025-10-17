using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

/// <summary>
/// Command to remove a specific SaleItem from a Sale Aggregate Root.
/// </summary>
public record DeleteSaleItemCommand(Guid SaleId, Guid ItemId) : IRequest;