namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Application.Exceptions;


/// <summary>
/// Handles the CancelSaleCommand, executing the domain logic for total sale cancellation.
/// </summary>
public class CancelSaleHandler : IRequestHandler<CancelSaleCommand>
{
    private readonly ISaleRepository _saleRepository;

    public CancelSaleHandler(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
    }

    public async Task Handle(CancelSaleCommand command, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new NotFoundException($"Sale with ID {command.Id} not found.");

        sale.Cancel();

        await _saleRepository.UpdateAsync(sale);
    }
}