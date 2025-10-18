using Ambev.DeveloperEvaluation.Application.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Extensions;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using AutoMapper;
using FluentValidation;
using MediatR;
using Rebus.Bus;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

/// <summary>
/// Handler responsible for receiving the UpdateSaleCommand and updating the Sale Aggregate Root.
/// It orchestrates domain validation, persistence (Unit of Work), and event publication.
/// </summary>
public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IBus _bus;

    public UpdateSaleHandler(ISaleRepository repository, IMapper mapper, IBus bus)
    {
        _saleRepository = repository;
        _mapper = mapper;
        _bus = bus;
    }

    public async Task<UpdateSaleResult> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new UpdateSaleValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdForUpdateAsync(command.Id, cancellationToken)
             ?? throw new NotFoundException($"Sale with ID {command.Id} not found.");

        var customer = new ExternalReference(command.CustomerId, command.CustomerName);
        var branch = new ExternalReference(command.BranchId, command.BranchName);

        var newItems = command.Items
            .Select(_mapper.Map<SaleItem>)
            .ToList();

        sale.Update(customer, branch, newItems);

        await _saleRepository.UpdateAsync(sale, cancellationToken);

        var saleModifiedEvent = sale.SaleModifiedEvent();
        await _bus.Send(saleModifiedEvent);

        return _mapper.Map<UpdateSaleResult>(sale);
    }
}
