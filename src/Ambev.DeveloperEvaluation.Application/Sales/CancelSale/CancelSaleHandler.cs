namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Application.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Extensions;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentValidation;
using MediatR;
using Rebus.Bus;
using System.Threading;
using System.Threading.Tasks;


/// <summary>
/// Handles the CancelSaleCommand, executing the domain logic for total sale cancellation.
/// </summary>
public class CancelSaleHandler : IRequestHandler<CancelSaleCommand>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IBus _bus;

    public CancelSaleHandler(ISaleRepository saleRepository, IBus bus)
    {
        _saleRepository = saleRepository;
        _bus = bus;
    }

    public async Task Handle(CancelSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new CancelSaleValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdForUpdateAsync(command.Id, cancellationToken)
            ?? throw new NotFoundException($"Sale with ID {command.Id} not found.");

        sale.Cancel();

        await _saleRepository.UpdateAsync(sale, cancellationToken);

        var saleCancelledEvent = sale.SaleCancelledEvent();
        await _bus.Send(saleCancelledEvent);

        foreach(var item in sale.Items)
        {
            var saleCancelledItemEvent = sale.SaleItemCancelledEvent(item.Id);
            await _bus.Send(saleCancelledItemEvent);
        }
    }
}