using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Common.Cache;
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
    private readonly ICacheManager _cacheManager;
    private readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

    public GetSalesHandler(ISaleRepository repository, IMapper mapper, ICacheManager cacheManager)
    {
        _saleRepository = repository;
        _mapper = mapper;
        _cacheManager = cacheManager;
    }

    public async Task<PaginatedList<GetSaleResult>> Handle(GetSalesQuery query, CancellationToken cancellationToken)
    {
        var validator = new GetSalesValidator();
        var validationResult = await validator.ValidateAsync(query, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var cacheKey = GetCacheKey(query);

        var cachedResult = await _cacheManager.GetItemAsync<PaginatedList<GetSaleResult>>(cacheKey);
        if (cachedResult is not null)
            return cachedResult;

        var (sales, count) = await _saleRepository.GetPaginatedAsync(
            query.PageNumber,
            query.PageSize,
            _mapper.Map<SaleSearchCriteria>(query.Criteria),
            cancellationToken);

        var result = _mapper.Map<List<GetSaleResult>>(sales);

        var paginatedResult = new PaginatedList<GetSaleResult>(
            result, count,
            query.PageNumber, query.PageSize
        );

        if (result.Count > 0)
        {
            await _cacheManager.SetItemAsync(cacheKey, paginatedResult, CacheDuration);
            await _cacheManager.TagKeyAsync(cacheKey, CacheConstants.SalesListTag);
        }
        
        return paginatedResult;
    }

    private static string GetCacheKey(GetSalesQuery query)
    {
        var cacheKey = $"_PageNumber:{query.PageNumber}_PageSize:{query.PageSize}";

        if (!string.IsNullOrWhiteSpace(query.Criteria.CustomerName))
            cacheKey += $"_CustomerName:{query.Criteria.CustomerName}";

        if (!string.IsNullOrWhiteSpace(query.Criteria.CustomerName))
            cacheKey += $"_BranchName:{query.Criteria.BranchName}";

        if (!string.IsNullOrWhiteSpace(query.Criteria.CustomerName))
            cacheKey += $"_ProductName:{query.Criteria.ProductName}";

        if (query.Criteria.SortField is not null)
            cacheKey += $"_SortField:{query.Criteria.SortField}";

        if (query.Criteria.SortDirection is not null)
            cacheKey += $"_SortDirection:{query.Criteria.SortDirection}";


        return cacheKey;
    }
}
