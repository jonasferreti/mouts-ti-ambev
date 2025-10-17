using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Shared;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

/// <summary>
/// Handler for the GetSalesQuery. Responsible for fetching a paginated list of Sale Aggregate Roots.
/// </summary>
public class GetSalesHandler : IRequestHandler<GetSalesQuery, PaginatedList<GetSaleResult>>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public GetSalesHandler(ISaleRepository repository, IMapper mapper)
    {
        _saleRepository = repository;
        _mapper = mapper;
    }

    public async Task<PaginatedList<GetSaleResult>> Handle(GetSalesQuery query, CancellationToken cancellationToken)
    {
        var validator = new GetSalesValidator();
        var validationResult = await validator.ValidateAsync(query, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var (sales, count) = await _saleRepository.GetPaginatedAsync(
            query.PageNumber,
            query.PageSize,
            _mapper.Map<SaleSearchCriteria>(query.Criteria),
            cancellationToken);

        var result = _mapper.Map<List<GetSaleResult>>(sales);

        return new PaginatedList<GetSaleResult>(
            result,
            count,
            query.PageNumber,
            query.PageSize
        );
    }
}
