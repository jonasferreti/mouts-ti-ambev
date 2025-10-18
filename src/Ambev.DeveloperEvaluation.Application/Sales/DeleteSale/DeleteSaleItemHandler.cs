using Ambev.DeveloperEvaluation.Application.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Extensions;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentValidation;
using MediatR;
using Rebus.Bus;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

/// <summary>
/// Handles item deletion. If the last item is deleted, it performs a cascading deletion 
/// of the parent Sale Aggregate Root.
/// </summary>
public class DeleteSaleItemHandler : IRequestHandler<DeleteSaleItemCommand>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IBus _bus;

    public DeleteSaleItemHandler(ISaleRepository saleRepository, IBus bus)
    {
        _saleRepository = saleRepository;
        _bus = bus;
    }

    public async Task Handle(DeleteSaleItemCommand command, CancellationToken cancellationToken)
    {
        var validator = new DeleteSaleItemValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdForUpdateAsync(command.SaleId, cancellationToken)
            ?? throw new NotFoundException($"Sale with ID {command.SaleId} not found.");

        sale.RemoveItem(command.ItemId);

        if (sale.HasItems())
        {
            await _saleRepository.UpdateAsync(sale, cancellationToken);
        }
        else
        {
            await _saleRepository.DeleteAsync(sale, cancellationToken);

            var saleDeletedEvent = sale.SaleDeletedEvent();
            await _bus.Send(saleDeletedEvent);
        }

        var saleItemDeletedEvent = sale.SaleItemDeletedEvent(command.ItemId);
        await _bus.Send(saleItemDeletedEvent);
    }
}
