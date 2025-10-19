using Ambev.DeveloperEvaluation.Application.Exceptions;
using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.Handlers.TestData; // NEW: Import TestData
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData; 
using FluentValidation;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Rebus.Bus;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Handlers;

/// <summary>
/// Contains unit tests for the CancelSaleItemHandler, focusing on item cancellation,
/// persistence, event publication, cascade cancellation, and exception handling.
/// </summary>
public class CancelSaleItemHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IBus _bus;
    private readonly CancelSaleItemHandler _handler;
    private readonly Guid _validSaleId;

    public CancelSaleItemHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _bus = Substitute.For<IBus>();

        // Get the ID that the TestData will use for consistency
        _validSaleId = CancelSaleItemHandlerTestData.ValidIdForArrangement();

        _handler = new CancelSaleItemHandler(_saleRepository, _bus);
    }

    /// <summary>
    /// Tests the successful flow where a single item is cancelled, but the entire sale remains active (more than one active item left).
    /// </summary>
    [Fact(DisplayName = "Handle success (Item only): Should cancel ONE item, persist, and send ONE SaleItemCancelledEvent")]
    public async Task Handle_GivenValidCommandWithActiveSale_ShouldCancelItemAndSendOnlyItemEvent()
    {
        // ARRANGE
        var mockedSale = SaleTestData.GenerateValidSaleWithItems(2);
        var validCommand = CancelSaleItemHandlerTestData
            .GenerateValidCommand(mockedSale.Id, mockedSale.Items.First().Id);


        Assert.False(mockedSale.IsCancelled);

        _saleRepository.GetByIdForUpdateAsync(validCommand.SaleId, Arg.Any<CancellationToken>())
            .Returns(mockedSale);

        Sale? updatedSale = null;
        _saleRepository.UpdateAsync(Arg.Do<Sale>(s => updatedSale = s), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // ACT
        await _handler.Handle(validCommand, CancellationToken.None);

        // ASSERT
        await _saleRepository.Received(1).UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
        Assert.False(updatedSale?.IsCancelled, "The Sale should NOT be fully cancelled.");
        Assert.True(updatedSale?.Items.First(i => i.Id == validCommand.ItemId).IsCancelled, "The specific Item should be cancelled.");

        await _bus.Received(1).Send(Arg.Is<SaleItemCancelledEvent>(e => e.ItemId == validCommand.ItemId));
        await _bus.DidNotReceive().Send(Arg.Is<SaleCancelledEvent>(e => e.SaleId == validCommand.SaleId));
    }

    /// <summary>
    /// Tests the successful flow where cancelling the last active item triggers the full sale cancellation.
    /// </summary>
    [Fact(DisplayName = "Handle success (Cascade): Should cancel last active item, triggering full SaleCancelledEvent")]
    public async Task Handle_GivenLastActiveItemCommand_ShouldCancelSaleAndSendBothEvents()
    {
        // ARRANGE
        var mockedSale = SaleTestData.GenerateValidSaleWithItems(1);
        var validCommand = CancelSaleItemHandlerTestData
            .GenerateValidCommand(mockedSale.Id, mockedSale.Items.First().Id);


        _saleRepository.GetByIdForUpdateAsync(validCommand.SaleId, Arg.Any<CancellationToken>())
            .Returns(mockedSale);

        Sale? updatedSale = null;
        _saleRepository.UpdateAsync(Arg.Do<Sale>(s => updatedSale = s), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // ACT
        await _handler.Handle(validCommand, CancellationToken.None);

        // ASSERT
        await _saleRepository.Received(1).UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
        Assert.True(updatedSale?.IsCancelled, "The Sale should be fully cancelled (cascade).");
        Assert.True(updatedSale?.Items.All(i => i.IsCancelled), "All Items should be cancelled.");

        await _bus.Received(1).Send(Arg.Is<SaleItemCancelledEvent>(e => e.ItemId == validCommand.ItemId));
        await _bus.Received(1).Send(Arg.Is<SaleCancelledEvent>(e => e.SaleId == validCommand.SaleId));
    }

    /// <summary>
    /// Tests that the handler throws a ValidationException when either ID is empty.
    /// </summary>
    [Theory(DisplayName = "Handle validation failure: Should throw ValidationException for invalid IDs and skip repository calls")]
    [MemberData(nameof(GetInvalidCommands))]
    public async Task Handle_GivenInvalidCommand_ShouldThrowValidationException(CancelSaleItemCommand invalidCommand)
    {
        // ACT & ASSERT
        await Assert.ThrowsAsync<ValidationException>(() =>
            _handler.Handle(invalidCommand, CancellationToken.None));

        // ASSERT: No dependencies should be called
        await _saleRepository.DidNotReceiveWithAnyArgs().GetByIdForUpdateAsync(default, default);
        await _bus.DidNotReceiveWithAnyArgs().Send(default!);
    }
    
    // Helper for invalid commands Theory
    public static IEnumerable<object[]> GetInvalidCommands =>
        new List<object[]>
        {
            new object[] { CancelSaleItemHandlerTestData.GenerateInvalidCommand_EmptySaleId() },
            new object[] { CancelSaleItemHandlerTestData.GenerateInvalidCommand_EmptyItemId() }
        };


    /// <summary>
    /// Tests that the handler throws NotFoundException when the Sale is not found.
    /// </summary>
    [Fact(DisplayName = "Handle sale not found: Should throw NotFoundException")]
    public async Task Handle_GivenNonExistentSale_ShouldThrowNotFoundException()
    {
        // ARRANGE
        var validCommand = CancelSaleItemHandlerTestData.GenerateValidCommand();
        
        _saleRepository.GetByIdForUpdateAsync(validCommand.SaleId, Arg.Any<CancellationToken>())
            .ReturnsNull(); // NSubstitute helper

        // ACT & ASSERT
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(validCommand, CancellationToken.None));

        // ASSERT: NO update or event should occur
        await _saleRepository.DidNotReceiveWithAnyArgs().UpdateAsync(default!, default);
        await _bus.DidNotReceiveWithAnyArgs().Send(default!);
    }

    /// <summary>
    /// Tests that the handler throws NotFoundException when the Item is not part of the Sale.
    /// </summary>
    [Fact(DisplayName = "Handle item not found: Should throw NotFoundException")]
    public async Task Handle_GivenNonExistentItem_ShouldThrowNotFoundException()
    {
        // ARRANGE
        var nonExistentCommand = CancelSaleItemHandlerTestData.GenerateNonExistentItemCommand();
        // Create a sale that DOES NOT contain the ItemId from the command
        var mockedSale = SaleTestData.GenerateValidSaleWithItems(1); 

        _saleRepository.GetByIdForUpdateAsync(nonExistentCommand.SaleId, Arg.Any<CancellationToken>())
            .Returns(mockedSale);

        // ACT & ASSERT
        await Assert.ThrowsAsync<NotFoundException>(() =>
        _handler.Handle(nonExistentCommand, CancellationToken.None));

        // ASSERT: NO update or event should occur
        await _saleRepository.DidNotReceiveWithAnyArgs().UpdateAsync(default!, default);
        await _bus.DidNotReceiveWithAnyArgs().Send(default!);
    }

    /// <summary>
    /// Tests that the handler throws a DomainException if the item domain logic rejects the Cancel operation (e.g., item already cancelled).
    /// </summary>
    [Fact(DisplayName = "Handle domain failure: Should throw DomainException when item is already cancelled")]
    public async Task Handle_GivenAlreadyCancelledItem_ShouldThrowDomainException()
    {
        // ARRANGE
        var mockedSale = SaleTestData.GenerateValidSaleWithItems(1);
        var validCommand = CancelSaleItemHandlerTestData
            .GenerateValidCommand(mockedSale.Id, mockedSale.Items.First().Id);


        // Explicitly cancel the item to force the domain exception
        mockedSale.Items.First(i => i.Id == validCommand.ItemId).Cancel(); 

        _saleRepository.GetByIdForUpdateAsync(validCommand.SaleId, Arg.Any<CancellationToken>())
            .Returns(mockedSale);

        // ACT & ASSERT
        await Assert.ThrowsAsync<DomainException>(() =>
            _handler.Handle(validCommand, CancellationToken.None));

        // ASSERT: NO persistence or event should occur
        await _saleRepository.DidNotReceiveWithAnyArgs().UpdateAsync(default!, default);
        await _bus.DidNotReceiveWithAnyArgs().Send(default!);
    }


    /// <summary>
    /// Tests that the Rebus events are NOT sent if the repository update operation fails.
    /// </summary>
    [Fact(DisplayName = "Handle persistence failure: Should throw exception and NOT send Rebus events")]
    public async Task Handle_GivenPersistenceFails_ShouldNotSendEvent()
    {
        // ARRANGE
        var mockedSale = SaleTestData.GenerateValidSaleWithItems(1);
        var validCommand = CancelSaleItemHandlerTestData
            .GenerateValidCommand(mockedSale.Id, mockedSale.Items.First().Id);


        _saleRepository.GetByIdForUpdateAsync(validCommand.SaleId, Arg.Any<CancellationToken>())
            .Returns(mockedSale);

        // Simulate a persistence failure
        _saleRepository
            .UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException<Sale>(new Exception("Database connection failed")));

        // ACT & ASSERT
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(validCommand, CancellationToken.None));

        // ASSERT: The events should not be sent
        await _bus.DidNotReceiveWithAnyArgs().Send(Arg.Any<SaleItemCancelledEvent>());
        await _bus.DidNotReceiveWithAnyArgs().Send(Arg.Any<SaleCancelledEvent>());
    }
}