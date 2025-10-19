using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Common.Cache;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Shared;
using Ambev.DeveloperEvaluation.Unit.Application.Handlers.TestData;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using AutoMapper;
using FluentValidation;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Handlers;

/// <summary>
/// Contains unit tests for the GetSalesHandler, focusing on pagination, filtering, 
/// and cache management for lists.
/// </summary>
public class GetSalesHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ICacheManager _cacheManager;
    private readonly GetSalesHandler _handler;

    public GetSalesHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _cacheManager = Substitute.For<ICacheManager>();

        _handler = new GetSalesHandler(_saleRepository, _mapper, _cacheManager);
    }

    /// <summary>
    /// Tests that a query with cacheable criteria returns the cached result.
    /// </summary>
    [Fact(DisplayName = "Handle success (Cache Hit): Should return cached PaginatedList and skip repository")]
    public async Task Handle_GivenCachedList_When_Handle_Then_ShouldReturnCachedResult()
    {
        // ARRANGE
        var query = GetSalesHandlerTestData.GenerateValidQuery(pageNumber: 2, pageSize: 10, withCriteria: true);
        var expectedResult = GetSalesHandlerTestData.GenerateCachedPaginatedList(totalCount: 50, pageNumber: 2, pageSize: 10);

        _cacheManager.GetItemAsync<PaginatedList<GetSaleResult>>(Arg.Any<string>())
            .Returns(expectedResult);

        // ACT
        var result = await _handler.Handle(query, CancellationToken.None);

        // ASSERT
        Assert.Equal(expectedResult.TotalCount, result.TotalCount);
        Assert.True(result.Data.Count != 0);

        // Verify dependencies
        await _saleRepository.DidNotReceiveWithAnyArgs()
            .GetPaginatedAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<SaleSearchCriteria>(), CancellationToken.None);

        await _cacheManager.DidNotReceiveWithAnyArgs()
            .SetItemAsync(Arg.Any<string>(), Arg.Any<PaginatedList<GetSaleResult>>(), Arg.Any<TimeSpan>());
    }

    /// <summary>
    /// Tests the successful flow: repository retrieval, mapping, and caching.
    /// </summary>
    [Fact(DisplayName = "Handle success (Repository Hit): Should retrieve, map, cache, and return list")]
    public async Task Handle_GivenValidQuery_When_Handle_Then_ShouldRetrieveMapCacheAndReturnList()
    {
        // ARRANGE
        const int pageSize = 2;
        const int totalCount = 15;
        var query = GetSalesHandlerTestData.GenerateValidQuery(pageNumber: 1, pageSize: pageSize, withCriteria: true);

        var mockedSales = SaleTestData.GenerateValidSales(pageSize);

        var expectedResults = GetSalesHandlerTestData.GenerateSaleResults(pageSize);

        _cacheManager.GetItemAsync<PaginatedList<GetSaleResult>>(Arg.Any<string>()).Returns((PaginatedList<GetSaleResult>)null!);

        _saleRepository.GetPaginatedAsync(
            Arg.Any<int>(), Arg.Any<int>(), Arg.Any<SaleSearchCriteria>(), Arg.Any<CancellationToken>())
            .Returns((mockedSales, totalCount));

        _mapper.Map<List<GetSaleResult>>(mockedSales).Returns(expectedResults);

        // ACT
        var result = await _handler.Handle(query, CancellationToken.None);

        // ASSERT
        Assert.Equal(totalCount, result.TotalCount);
        Assert.Equal(pageSize, result.Data.Count);

        // Verify repository call (should be called with mapped criteria type)
        await _saleRepository.Received(1).GetPaginatedAsync(
            query.PageNumber,
            query.PageSize,
            Arg.Any<SaleSearchCriteria>(),
            Arg.Any<CancellationToken>());

        // Verify mapping and caching
        _mapper.Received(1).Map<List<GetSaleResult>>(mockedSales);

        await _cacheManager.Received(1).SetItemAsync(Arg.Any<string>(),
            Arg.Is<PaginatedList<GetSaleResult>>(p => p.TotalCount == totalCount), Arg.Any<TimeSpan>());

        await _cacheManager.Received(1).TagKeyAsync(Arg.Any<string>(), CacheConstants.SalesListTag);
    }


    /// <summary>
    /// Tests the flow when the query returns no data (empty list). Should not write to cache.
    /// </summary>
    [Fact(DisplayName = "Handle success (No Data): Should return empty list and NOT write to cache")]
    public async Task Handle_GivenNoSalesFound_When_Handle_Then_ShouldReturnEmptyListAndNotCache()
    {
        // ARRANGE
        var query = GetSalesHandlerTestData.GenerateValidQuery(pageNumber: 5, pageSize: 10);
        var mockedSales = new List<Sale>();
        const int mockedCount = 0;
        var expectedResults = new List<GetSaleResult>();

        _cacheManager.GetItemAsync<PaginatedList<GetSaleResult>>(Arg.Any<string>())
            .Returns((PaginatedList<GetSaleResult>)null!);

        _saleRepository.GetPaginatedAsync(Arg.Any<int>(), Arg.Any<int>(),
            Arg.Any<SaleSearchCriteria>(), Arg.Any<CancellationToken>())
            .Returns((mockedSales, mockedCount));

        _mapper.Map<List<GetSaleResult>>(mockedSales).Returns(expectedResults);

        // ACT
        var result = await _handler.Handle(query, CancellationToken.None);

        // ASSERT
        Assert.Equal(0, result.TotalCount);
        Assert.Empty(result.Data);

        await _cacheManager.DidNotReceiveWithAnyArgs()
            .SetItemAsync(Arg.Any<string>(), Arg.Any<PaginatedList<GetSaleResult>>(), Arg.Any<TimeSpan>());
    }


    /// <summary>
    /// Tests that the handler throws a ValidationException for invalid pagination parameters.
    /// </summary>
    [Fact(DisplayName = "Handle validation failure: Should throw ValidationException for invalid page size")]
    public async Task Handle_GivenInvalidPageSize_When_Handle_Then_ShouldThrowValidationException()
    {
        // ARRANGE
        var invalidQuery = GetSalesHandlerTestData.GenerateInvalidQuery_InvalidPageSize();

        // ACT & ASSERT
        await Assert.ThrowsAsync<ValidationException>(() =>
            _handler.Handle(invalidQuery, CancellationToken.None));

        // ASSERT: No repository or cache interaction should occur
        await _saleRepository.DidNotReceiveWithAnyArgs()
           .GetPaginatedAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<SaleSearchCriteria>(), CancellationToken.None);

        await _cacheManager.DidNotReceiveWithAnyArgs()
         .SetItemAsync(Arg.Any<string>(), Arg.Any<PaginatedList<GetSaleResult>>(), Arg.Any<TimeSpan>());
    }
}