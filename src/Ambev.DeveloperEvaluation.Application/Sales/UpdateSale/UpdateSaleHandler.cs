using Ambev.DeveloperEvaluation.Application.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

/// <summary>
/// Handler responsible for receiving the UpdateSaleCommand and updating the Sale Aggregate Root.
/// It orchestrates domain validation, persistence (Unit of Work), and event publication.
/// </summary>
public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public UpdateSaleHandler(ISaleRepository repository, IMapper mapper)
    {
        _saleRepository = repository;
        _mapper = mapper;
    }

    public async Task<UpdateSaleResult> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new UpdateSaleValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken)
             ?? throw new NotFoundException($"Sale with ID {command.Id} not found.");

        var customer = new ExternalReference(command.CustomerId, command.CustomerName);
        var branch = new ExternalReference(command.BranchId, command.BranchName);

        var newItems = command.Items
            .Select(_mapper.Map<SaleItem>)
            .ToList();

        sale.Update(customer, branch, newItems);

        await _saleRepository.UpdateAsync(sale, cancellationToken);
        return _mapper.Map<UpdateSaleResult>(sale);
    }
}
