using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;
using Rebus.Bus;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Handler responsible for receiving the CreateSaleCommand and creating the Sale Aggregate Root.
/// It orchestrates domain validation, persistence (Unit of Work), and event publication.
/// </summary>
public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IBus _bus;

    public CreateSaleHandler(ISaleRepository repository, IMapper mapper, IBus bus)
    {
        _saleRepository = repository;
        _mapper = mapper;
        _bus = bus;
    }

    public async Task<CreateSaleResult> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new CreateSaleValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = _mapper.Map<Sale>(command);

        foreach (var item in command.Items)
        {
            var saleItem = _mapper.Map<SaleItem>(item);
            sale.AddItem(saleItem);
        }

        await _saleRepository.CreateAsync(sale, cancellationToken);

        await EmitSaleCreateEvent(sale);

        return _mapper.Map<CreateSaleResult>(sale);
    }

    private async Task EmitSaleCreateEvent(Sale sale)
    {
        var saleCreatedEvent = new SaleCreatedEvent(
            sale.Id, 
            sale.Customer.Value,
            sale.Branch.Value, 
            sale.TotalAmount.Value
        );

        await _bus.Send(saleCreatedEvent);

    }
}
