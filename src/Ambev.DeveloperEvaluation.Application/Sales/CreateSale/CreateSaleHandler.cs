using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Handler responsible for receiving the CreateSaleCommand and creating the Sale Aggregate Root.
/// It orchestrates domain validation, persistence (Unit of Work), and event publication.
/// </summary>
public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public CreateSaleHandler(ISaleRepository repository, IMapper mapper)
    {
        _saleRepository = repository;
        _mapper = mapper;
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
        return _mapper.Map<CreateSaleResult>(sale);
    }
}
