using Ambev.DeveloperEvaluation.Application.Exceptions;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Common.Cache;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.Handlers.TestData;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using AutoMapper;
using FluentValidation;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Handlers;

/// <summary>
/// Contains unit tests for the GetSaleHandler, focusing on retrieval, 
/// NotFound handling, validation, and cache management.
/// </summary>
public class GetSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ICacheManager _cacheManager;
    private readonly GetSaleHandler _handler;

    private readonly Guid _validSaleId;
    private readonly string _cacheKey;

    public GetSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _cacheManager = Substitute.For<ICacheManager>();

        // Retrieve static IDs/Keys from TestData for consistent arrangement
        _validSaleId = GetSaleHandlerTestData.ValidSaleIdForArrangement();
        _cacheKey = GetSaleHandlerTestData.ValidCacheKey();

        _handler = new GetSaleHandler(_saleRepository, _mapper, _cacheManager);
    }

    /// <summary>
    /// Tests that if the item is in the cache, it is returned immediately, skipping the repository call.
    /// </summary>
    [Fact(DisplayName = "Handle success (Cache Hit): Should return cached result and skip repository")]
    public async Task Handle_GivenCachedSale_ShouldReturnCachedResultAndSkipRepository()
    {
        // ARRANGE
        var query = GetSaleHandlerTestData.GenerateValidQuery();
        var expectedResult = GetSaleHandlerTestData.GenerateExpectedResult();

        _cacheManager.GetItemAsync<GetSaleResult>(_cacheKey).Returns(expectedResult);

        // ACT
        var result = await _handler.Handle(query, CancellationToken.None);

        // ASSERT
        // 1. Result Verification
        Assert.Equal(expectedResult.Id, result.Id);

        // 2. Dependency Verification
        await _saleRepository.DidNotReceiveWithAnyArgs()
            .GetByIdAsync(Arg.Any<Guid>(), CancellationToken.None);
        // Should not try to set the item if it was a hit
        await _cacheManager.DidNotReceiveWithAnyArgs()
            .SetItemAsync(Arg.Any<string>(), Arg.Any<GetSaleResult>(), Arg.Any<TimeSpan>());
    }

    /// <summary>
    /// Tests the successful flow: cache miss, repository retrieval, mapping, and caching.
    /// </summary>
    [Fact(DisplayName = "Handle success (Repository Hit): Should retrieve, map, cache, and return")]
    public async Task Handle_GivenValidQuery_ShouldRetrieveMapCacheAndReturn()
    {
        // ARRANGE
        var query = GetSaleHandlerTestData.GenerateValidQuery();

        // 1. Setup Domain & DTOs
        var mockedSale = SaleTestData.GenerateValidSale();
        var expectedResult = GetSaleHandlerTestData.GenerateExpectedResult();

        // 2. Setup Dependencies
        _cacheManager.GetItemAsync<GetSaleResult>(_cacheKey).ReturnsNull(); // Cache Miss
        _saleRepository.GetByIdAsync(_validSaleId, Arg.Any<CancellationToken>()).Returns(mockedSale);
        _mapper.Map<GetSaleResult>(mockedSale).Returns(expectedResult);

        // ACT
        var result = await _handler.Handle(query, CancellationToken.None);

        // ASSERT
        // 1. Result Verification
        Assert.Equal(expectedResult.Id, result.Id);

        // 2. Dependency Verification
        await _saleRepository.Received(1).GetByIdAsync(_validSaleId, Arg.Any<CancellationToken>());
        _mapper.Received(1).Map<GetSaleResult>(mockedSale);

        // 3. Cache Verification (should SET the item after retrieval)
        await _cacheManager.Received(1).SetItemAsync(_cacheKey, expectedResult, Arg.Any<TimeSpan>());
    }


    /// <summary>
    /// Tests that the handler throws a ValidationException when the query ID is empty.
    /// </summary>
    [Fact(DisplayName = "Handle validation failure: Should throw ValidationException and skip logic")]
    public async Task Handle_GivenInvalidQuery_ShouldThrowValidationException()
    {
        // ARRANGE
        var invalidQuery = GetSaleHandlerTestData.GenerateInvalidQuery_EmptySaleId();

        // ACT & ASSERT
        await Assert.ThrowsAsync<ValidationException>(() =>
            _handler.Handle(invalidQuery, CancellationToken.None));

        // ASSERT: No dependencies should be called
        await _cacheManager.DidNotReceiveWithAnyArgs().GetItemAsync<object>(default!);
        await _saleRepository.DidNotReceiveWithAnyArgs().GetByIdAsync(default, default);
    }


    /// <summary>
    /// Tests that the handler throws NotFoundException when the Sale is not found in the repository after a cache miss.
    /// </summary>
    [Fact(DisplayName = "Handle not found: Should throw NotFoundException when Sale is null")]
    public async Task Handle_GivenNonExistentSale_ShouldThrowNotFoundException()
    {
        // ARRANGE
        var query = GetSaleHandlerTestData.GenerateValidQuery(); // We test with the valid ID, but mock the repo to return null

        _cacheManager.GetItemAsync<GetSaleResult>(_cacheKey).ReturnsNull(); // Cache Miss

        _saleRepository.GetByIdAsync(_validSaleId, Arg.Any<CancellationToken>())
            .ReturnsNull(); // Repository Miss

        // ACT & ASSERT
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(query, CancellationToken.None));


        // ASSERT: Cache Set should not be called
        await _cacheManager.DidNotReceiveWithAnyArgs()
            .SetItemAsync(Arg.Any<string>(), Arg.Any<GetSaleResult>(), Arg.Any<TimeSpan>());
    }
}