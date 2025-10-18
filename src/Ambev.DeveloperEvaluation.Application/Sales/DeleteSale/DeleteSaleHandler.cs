using Ambev.DeveloperEvaluation.Application.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Extensions;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentValidation;
using MediatR;
using Rebus.Bus;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

/// <summary>
/// Handles the DeleteSaleCommand, performing the final deletion of the Aggregate Root.
/// </summary>
public class DeleteSaleHandler : IRequestHandler<DeleteSaleCommand>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IBus _bus;

    public DeleteSaleHandler(ISaleRepository saleRepository, IBus bus)
    {
        _saleRepository = saleRepository;
        this._bus = bus;
    }

    public async Task Handle(DeleteSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new DeleteSaleValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new NotFoundException($"Sale with ID {command.Id} not found.");

        await _saleRepository.DeleteAsync(sale, cancellationToken);

        var saleDeletedEvent = sale.SaleDeletedEvent();
        await _bus.Send(saleDeletedEvent);

        foreach(var item in sale.Items)
        {
            var saleItemDeletedEvent = sale.SaleItemDeletedEvent(item.Id);
            await _bus.Send(saleItemDeletedEvent);
        }
    }
}
