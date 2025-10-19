using Ambev.DeveloperEvaluation.Application.Exceptions;
using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.Handlers.TestData;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using FluentValidation;
using NSubstitute;
using Rebus.Bus;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Handlers;

/// <summary>
/// Contains unit tests for the CancelSaleHandler, focusing on domain orchestration,
/// persistence, event publication, and exception handling.
/// </summary>
public class CancelSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IBus _bus;
    private readonly CancelSaleHandler _handler;
    private readonly Guid _validSaleId;

    public CancelSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _bus = Substitute.For<IBus>();

        _validSaleId = CancelSaleHandlerTestData.ValidIdForArrangement();
        _handler = new CancelSaleHandler(_saleRepository, _bus);
    }

    public static IEnumerable<object[]> SaleCounts => [[1],[3],[5]];


    /// <summary>
    /// Tests the successful flow: validation, retrieval, domain logic, persistence, and event sending.
    /// Uses [Theory] to test with different numbers of items, ensuring N SaleItemCancelledEvent events are sent.
    /// </summary>
    [Theory(DisplayName = "Handle success: Should cancel the Sale, persist, and send ALL cancellation events")]
    [MemberData(nameof(SaleCounts))]
    public async Task Handle_GivenValidCommand_ShouldCancelAndSendEvents(int itemCount)
    {
        // ARRANGE
        var validCommand = CancelSaleHandlerTestData.GenerateValidCommand();
        var mockedSale = SaleTestData.GenerateValidSaleWithItems(itemCount);

        _saleRepository.GetByIdForUpdateAsync(_validSaleId, Arg.Any<CancellationToken>())
            .Returns(mockedSale);

        // Capture the updated sale for post-persistence assertions
        Sale? updatedSale = null;
        _saleRepository.UpdateAsync(Arg.Do<Sale>(s => updatedSale = s), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // ACT
        await _handler.Handle(validCommand, CancellationToken.None);

        // ASSERT
        // 1. Persistence Verification
        await _saleRepository.Received(1).UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());

        // 2. Domain State Verification
        Assert.True(updatedSale?.IsCancelled, "The Sale should be cancelled after Handle.");
        Assert.True(updatedSale?.Items.All(i => i.IsCancelled), "All Items should be cancelled.");

        // 3. Event Verification
        await _bus.Received(1).Send(Arg.Is<SaleCancelledEvent>(e => e.SaleId == mockedSale.Id));
        await _bus.Received(itemCount).Send(Arg.Is<SaleItemCancelledEvent>(e => e.SaleId == mockedSale.Id));
    }


    /// <summary>
    /// Tests that the handler throws a ValidationException when the command ID is empty (Guid.Empty).
    /// </summary>
    [Fact(DisplayName = "Handle validation failure: Should throw ValidationException and skip repository calls")]
    public async Task Handle_GivenInvalidCommand_ShouldThrowValidationException()
    {
        // ARRANGE
        var invalidCommand = CancelSaleHandlerTestData.GenerateInvalidCommand_EmptySaleId();

        // ACT & ASSERT
        await Assert.ThrowsAsync<ValidationException>(() =>
            _handler.Handle(invalidCommand, CancellationToken.None));

        // ASSERT: No dependencies should be called if validation fails
        await _saleRepository.DidNotReceiveWithAnyArgs().GetByIdForUpdateAsync(default, default);
        await _saleRepository.DidNotReceiveWithAnyArgs().UpdateAsync(default!, default);
        await _bus.DidNotReceiveWithAnyArgs().Send(default!);
    }

    /// <summary>
    /// Tests that the handler throws NotFoundException when the Sale is not found.
    /// </summary>
    [Fact(DisplayName = "Handle not found: Should throw NotFoundException when Sale is null")]
    public async Task Handle_GivenNonExistentSale_ShouldThrowNotFoundException()
    {
        // ARRANGE
        var notFoundCommand = CancelSaleHandlerTestData.GenerateNotFoundCommand(); // Uses a different ID than setup

        _saleRepository.GetByIdForUpdateAsync(notFoundCommand.Id, Arg.Any<CancellationToken>())
            .Returns((Sale)null!); // Returns null

        // ACT & ASSERT
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(notFoundCommand, CancellationToken.None));

        // ASSERT: NO update or event should occur
        await _saleRepository.DidNotReceiveWithAnyArgs().UpdateAsync(default!, default);
        await _bus.DidNotReceiveWithAnyArgs().Send(default!);
    }

    /// <summary>
    /// Tests that the handler throws a DomainException if the domain logic rejects the operation (e.g., sale already cancelled).
    /// </summary>
    [Fact(DisplayName = "Handle domain failure: Should throw DomainException when sale is already cancelled")]
    public async Task Handle_GivenAlreadyCancelledSale_ShouldThrowDomainException()
    {
        // ARRANGE
        var validCommand = CancelSaleHandlerTestData.GenerateValidCommand();
        var mockedSale = SaleTestData.GenerateValidSale();
        mockedSale.Cancel(); // Puts the sale into a "cancelled" state to force the exception

        _saleRepository.GetByIdForUpdateAsync(_validSaleId, Arg.Any<CancellationToken>())
            .Returns(mockedSale);

        // ACT & ASSERT
        // The handler must propagate the DomainException thrown by the domain method
        await Assert.ThrowsAsync<DomainException>(() => _handler.Handle(validCommand, CancellationToken.None));

        // ASSERT: NO persistence or event should occur
        await _saleRepository.DidNotReceiveWithAnyArgs().UpdateAsync(default!, default);
        await _bus.DidNotReceiveWithAnyArgs().Send(default!);
    }


    /// <summary>
    /// Tests that Rebus events are NOT sent if the repository update operation fails.
    /// This ensures events aren't published in case of a transaction failure.
    /// </summary>
    [Fact(DisplayName = "Handle persistence failure: Should throw exception and NOT send Rebus events")]
    public async Task Handle_GivenPersistenceFails_ShouldNotSendEvent()
    {
        // ARRANGE
        var validCommand = CancelSaleHandlerTestData.GenerateValidCommand();
        var mockedSale = SaleTestData.GenerateValidSaleWithItems(1);

        _saleRepository.GetByIdForUpdateAsync(_validSaleId, Arg.Any<CancellationToken>())
            .Returns(mockedSale);

        // Configure the repository to throw an exception upon saving
        _saleRepository
            .UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException<Sale>(new Exception("Database connection failed")));

        // ACT & ASSERT
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(validCommand, CancellationToken.None));

        // ASSERT: The event should not be sent
        await _bus.DidNotReceiveWithAnyArgs().Send(Arg.Any<SaleCancelledEvent>());
        await _bus.DidNotReceiveWithAnyArgs().Send(Arg.Any<SaleItemCancelledEvent>());
    }
}