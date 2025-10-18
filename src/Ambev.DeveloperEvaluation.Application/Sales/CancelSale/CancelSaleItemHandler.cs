using Ambev.DeveloperEvaluation.Application.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Extensions;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentValidation;
using MediatR;
using Rebus.Bus;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

/// <summary>
/// Handles the CancelSaleItemCommand, executing the domain logic for item cancellation 
/// and checking for aggregate cascade cancellation.
/// </summary>
public class CancelSaleItemHandler : IRequestHandler<CancelSaleItemCommand>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IBus _bus;

    public CancelSaleItemHandler(ISaleRepository saleRepository, IBus bus)
    {
        _saleRepository = saleRepository;
        _bus = bus;
    }

    public async Task Handle(CancelSaleItemCommand command, CancellationToken cancellationToken)
    {
        var validator = new CancelSaleItemValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdForUpdateAsync(command.SaleId, cancellationToken)
            ?? throw new NotFoundException($"Sale with ID {command.SaleId} not found.");

        var item = sale.Items.FirstOrDefault(i => i.Id == command.ItemId)
            ?? throw new NotFoundException($"Item with ID {command.ItemId} not found for Sale ID {command.SaleId}");

        sale.CancelItem(command.ItemId);

        await _saleRepository.UpdateAsync(sale, cancellationToken);

        var saleItemCancelledEvent = sale.SaleItemCancelledEvent(item.Id);
        await _bus.Send(saleItemCancelledEvent);

        if (sale.IsCancelled)
        {
            var saleCancelledEvent = sale.SaleCancelledEvent();
            await _bus.Send(saleCancelledEvent);
        }
    }
}
