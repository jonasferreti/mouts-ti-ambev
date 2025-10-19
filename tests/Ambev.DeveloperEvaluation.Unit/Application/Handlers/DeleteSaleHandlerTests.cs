using Ambev.DeveloperEvaluation.Application.Exceptions;
using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.Handlers.TestData;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using FluentValidation;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Rebus.Bus;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Handlers;

/// <summary>
/// Contains unit tests for the DeleteSaleHandler, focusing on the orchestration
/// of the complete deletion of the Sale Aggregate Root and subsequent event publication.
/// </summary>
public class DeleteSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IBus _bus;
    private readonly DeleteSaleHandler _handler;
    private readonly Guid _validSaleId;

    public DeleteSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _bus = Substitute.For<IBus>();

        // Get the ID that the TestData will use for consistency
        _validSaleId = DeleteSaleHandlerTestData.ValidIdForArrangement();

        // Handler instantiation with mocked dependencies
        _handler = new DeleteSaleHandler(_saleRepository, _bus);
    }

    /// <summary>
    /// Tests the complete successful deletion flow: fetch, persistence, and event publishing (Sale + all Items).
    /// </summary>
    [Fact(DisplayName = "Handle success: Should delete sale, persist, and publish SaleDeletedEvent and all SaleItemDeletedEvents")]
    public async Task Handle_GivenValidCommand_ShouldDeleteSalePersistAndPublishEvents()
    {
        // ARRANGE
        var command = DeleteSaleHandlerTestData.GenerateValidCommand();
        var mockedSale = SaleTestData.GenerateValidSaleWithItems(2);

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(mockedSale);

        // ACT
        await _handler.Handle(command, CancellationToken.None);

        // ASSERT
        // 1. Repository Interaction Verification
        await _saleRepository.Received(1).GetByIdAsync(command.Id, Arg.Any<CancellationToken>());
        await _saleRepository.Received(1).DeleteAsync(mockedSale, Arg.Any<CancellationToken>());

        // 2. Event Publishing Verification (Sale Deleted)
        await _bus.Received(1).Send(Arg.Is<SaleDeletedEvent>(e => e.SaleId == mockedSale.Id));

        // 3. Event Publishing Verification (All Items Deleted) - Checks that 2 Item deleted events were sent
        await _bus.Received(2).Send(Arg.Is<SaleItemDeletedEvent>(e => e.SaleId == mockedSale.Id));

        // Optional: Verify specific item IDs were used (more robust, but relies on knowing the internal IDs)
        // Note: The previous version checked element 0 and 1 explicitly.
        // We can verify that an event was sent for the first item's ID:
        await _bus.Received(1).Send(Arg.Is<SaleItemDeletedEvent>(e => e.ItemId == mockedSale.Items.First().Id));
    }

    /// <summary>
    /// Tests if a ValidationException is thrown for an invalid command (e.g., empty Id).
    /// </summary>
    [Fact(DisplayName = "Handle validation failure: Should throw ValidationException for invalid command")]
    public async Task Handle_GivenInvalidCommand_ShouldThrowValidationException()
    {
        // ARRANGE
        var invalidCommand = DeleteSaleHandlerTestData.GenerateInvalidCommand_EmptySaleId();

        // ACT & ASSERT
        await Assert.ThrowsAsync<ValidationException>(() =>
            _handler.Handle(invalidCommand, CancellationToken.None));

        // ASSERT: No repository or bus calls should happen
        await _saleRepository.DidNotReceiveWithAnyArgs().GetByIdAsync(default, default);
        await _saleRepository.DidNotReceiveWithAnyArgs().DeleteAsync(default!, default);
        await _bus.DidNotReceiveWithAnyArgs().Send(default!);
    }

    /// <summary>
    /// Tests if NotFoundException is thrown when the Sale aggregate does not exist.
    /// </summary>
    [Fact(DisplayName = "Handle not found: Should throw NotFoundException when Sale does not exist")]
    public async Task Handle_GivenNonExistentSaleId_ShouldThrowNotFoundException()
    {
        // ARRANGE
        var command = DeleteSaleHandlerTestData.GenerateNotFoundCommand(); // Use a different ID

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .ReturnsNull(); // NSubstitute helper for returning null

        // ACT & ASSERT
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));

        // ASSERT
        await _saleRepository.Received(1).GetByIdAsync(command.Id, Arg.Any<CancellationToken>());

        // Assert: Deletion and events should not occur
        await _saleRepository.DidNotReceiveWithAnyArgs().DeleteAsync(default!, default);
        await _bus.DidNotReceiveWithAnyArgs().Send(Arg.Any<SaleDeletedEvent>());
    }
}