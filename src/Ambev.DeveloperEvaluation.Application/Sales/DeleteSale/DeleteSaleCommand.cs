using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

/// <summary>
/// Command to delete a Sale Aggregate Root.
/// </summary>
public record DeleteSaleCommand(Guid Id) : IRequest;
