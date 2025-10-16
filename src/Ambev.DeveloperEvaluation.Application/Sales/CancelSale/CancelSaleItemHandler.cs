using Ambev.DeveloperEvaluation.Application.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

/// <summary>
/// Handles the CancelSaleItemCommand, executing the domain logic for item cancellation 
/// and checking for aggregate cascade cancellation.
/// </summary>
public class CancelSaleItemHandler : IRequestHandler<CancelSaleItemCommand>
{
    private readonly ISaleRepository _saleRepository;

    public CancelSaleItemHandler(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
    }

    public async Task Handle(CancelSaleItemCommand command, CancellationToken cancellationToken)
    {
        // 1. Load the Aggregate Root using the SaleId
        var sale = await _saleRepository.GetByIdAsync(command.SaleId, cancellationToken)
            ?? throw new NotFoundException($"Sale with ID {command.SaleId} not found.");

        var item = sale.Items.FirstOrDefault(i => i.Id == command.ItemId)
            ?? throw new NotFoundException($"Item with ID {command.ItemId} not found for Sale ID {command.SaleId}");

        sale.CancelItem(command.ItemId);

        await _saleRepository.UpdateAsync(sale);
    }
}
