using Ambev.DeveloperEvaluation.Application.Exceptions;
using Ambev.DeveloperEvaluation.Common.Cache;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

/// <summary>
/// Handler for the GetSaleQuery. Responsible for fetching a single Sale Aggregate Root by ID.
/// </summary>
public class GetSaleHandler : IRequestHandler<GetSaleQuery, GetSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ICacheManager _cacheManager;
    private readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

    public GetSaleHandler(ISaleRepository repository, IMapper mapper,
        ICacheManager cacheManager)
    {
        _saleRepository = repository;
        _mapper = mapper;
        _cacheManager = cacheManager;
    }

    public async Task<GetSaleResult> Handle(GetSaleQuery query, CancellationToken cancellationToken)
    {
        var validator = new GetSaleValidator();
        var validationResult = await validator.ValidateAsync(query, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var cacheKey = query.Id.ToString();
        var cachedResult = await _cacheManager.GetItemAsync<GetSaleResult>(cacheKey);
        if (cachedResult is not null)
            return cachedResult;

        var sale = await _saleRepository.GetByIdAsync(query.Id, cancellationToken)
           ?? throw new NotFoundException($"Sale with ID {query.Id} not found.");

        var result = _mapper.Map<GetSaleResult>(sale);

        await _cacheManager.SetItemAsync(cacheKey, result, CacheDuration);

        return result;
    }
}
