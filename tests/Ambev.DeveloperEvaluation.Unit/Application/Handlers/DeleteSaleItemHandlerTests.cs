using Ambev.DeveloperEvaluation.Application.Exceptions;
using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
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
/// Contains unit tests for the DeleteSaleItemHandler, focusing on item removal, 
/// conditional cascading deletion of the parent Sale Aggregate Root, and event publication.
/// </summary>
public class DeleteSaleItemHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IBus _bus;
    private readonly DeleteSaleItemHandler _handler;


    public DeleteSaleItemHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _bus = Substitute.For<IBus>();

        _handler = new DeleteSaleItemHandler(_saleRepository, _bus);
    }

    // Helper for invalid commands Theory
    public static IEnumerable<object[]> GetInvalidCommands =>
        new List<object[]>
        {
            new object[] { DeleteSaleItemHandlerTestData.GenerateInvalidCommand_EmptySaleId() },
            new object[] { DeleteSaleItemHandlerTestData.GenerateInvalidCommand_EmptyItemId() }
        };

    /// <summary>
    /// Tests the scenario where a SaleItem is deleted but the Sale aggregate remains.
    /// This should result in an Update operation and a SaleItemDeletedEvent publication.
    /// </summary>
    [Fact(DisplayName = "Handle success (Update): Should remove item and update sale when other items remain")]
    public async Task Handle_GivenItemDeleted_WhenSaleHasOtherItems_ShouldUpdateSaleAndPublishItemDeletedEvent()
    {
        // ARRANGE
        // 1. Build Sale with 2 items, ensuring the first item matches the command's ItemId
        var mockedSale = SaleTestData.GenerateValidSaleWithItems(2);
        var command = DeleteSaleItemHandlerTestData.GenerateValidCommand(mockedSale.Id, mockedSale.Items.First().Id);

        _saleRepository.GetByIdForUpdateAsync(command.SaleId, Arg.Any<CancellationToken>())
            .Returns(mockedSale);

        // ACT
        await _handler.Handle(command, CancellationToken.None);

        // ASSERT
        // 1. Persistence Verification: Should UPDATE, not DELETE
        await _saleRepository.Received(1).UpdateAsync(mockedSale, Arg.Any<CancellationToken>());
        await _saleRepository.DidNotReceive().DeleteAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());

        // 2. Domain State Verification (optional but good): Item count should decrease
        Assert.Single(mockedSale.Items);

        // 3. Event Verification
        await _bus.Received(1).Send(Arg.Is<SaleItemDeletedEvent>(e =>
           e.SaleId == command.SaleId && e.ItemId == command.ItemId));

        await _bus.DidNotReceive().Send(Arg.Any<SaleDeletedEvent>());
    }

    /// <summary>
    /// Tests the scenario where the last SaleItem is deleted, causing a cascading deletion of the Sale aggregate.
    /// This should result in a Delete operation and the publication of both events.
    /// </summary>
    [Fact(DisplayName = "Handle success (Cascade Delete): Should remove item and cascade delete sale when no other items remain")]
    public async Task Handle_GivenItemDeleted_WhenSaleHasNoOtherItems_ShouldDeleteSaleAndPublishBothEvents()
    {
        // ARRANGE
        // 1. Build Sale with 1 item, ensuring it matches the command's ItemId
        var mockedSale = SaleTestData.GenerateValidSaleWithItems(1);
        var command = DeleteSaleItemHandlerTestData.GenerateValidCommand(mockedSale.Id, mockedSale.Items.First().Id);

        _saleRepository.GetByIdForUpdateAsync(command.SaleId, Arg.Any<CancellationToken>())
            .Returns(mockedSale);

        // ACT
        await _handler.Handle(command, CancellationToken.None);

        // ASSERT
        // 1. Persistence Verification: Should DELETE, not UPDATE
        await _saleRepository.Received(1).GetByIdForUpdateAsync(command.SaleId, Arg.Any<CancellationToken>());
        await _saleRepository.DidNotReceive().UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
        await _saleRepository.Received(1).DeleteAsync(mockedSale, Arg.Any<CancellationToken>());

        // 2. Event Verification
        await _bus.Received(1).Send(Arg.Is<SaleDeletedEvent>(e => e.SaleId == command.SaleId));

        await _bus.Received(1).Send(Arg.Is<SaleItemDeletedEvent>(e =>
            e.SaleId == command.SaleId && e.ItemId == command.ItemId));
    }


    /// <summary>
    /// Tests if a ValidationException is thrown for an invalid command (e.g., empty ID).
    /// </summary>
    [Theory(DisplayName = "Handle validation failure: Should throw ValidationException for invalid IDs and skip repository calls")]
    [MemberData(nameof(GetInvalidCommands))]
    public async Task Handle_GivenInvalidCommand_ShouldThrowValidationException(DeleteSaleItemCommand invalidCommand)
    {
        // ACT & ASSERT
        await Assert.ThrowsAsync<ValidationException>(() =>
            _handler.Handle(invalidCommand, CancellationToken.None));

        // ASSERT: No repository or bus calls should happen
        await _saleRepository.DidNotReceiveWithAnyArgs().GetByIdAsync(default, default);
        await _saleRepository.DidNotReceiveWithAnyArgs().UpdateAsync(default!, default);
        await _saleRepository.DidNotReceiveWithAnyArgs().DeleteAsync(default!, default);
        await _bus.DidNotReceiveWithAnyArgs().Send(default!);
    }

    /// <summary>
    /// Tests if NotFoundException is thrown when the Sale aggregate does not exist.
    /// </summary>
    [Fact(DisplayName = "Handle sale not found: Should throw NotFoundException when Sale does not exist")]
    public async Task Handle_GivenNonExistentSaleId_ShouldThrowNotFoundException()
    {
        // ARRANGE
        var command = DeleteSaleItemHandlerTestData.GenerateCommandWithNonExistentSale();

        // ACT
        _saleRepository.GetByIdForUpdateAsync(command.SaleId, Arg.Any<CancellationToken>()).ReturnsNull();

        // ASSERT
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));

        await _saleRepository.Received(1).GetByIdForUpdateAsync(command.SaleId, Arg.Any<CancellationToken>());

        // Assert: Update, Delete, and Events should not occur
        await _saleRepository.DidNotReceiveWithAnyArgs().UpdateAsync(default!, default);
        await _saleRepository.DidNotReceiveWithAnyArgs().DeleteAsync(default!, default);
        await _bus.DidNotReceiveWithAnyArgs().Send(default!);
    }

    /// <summary>
    /// Tests if persistence is aborted if the domain call to RemoveItem fails (e.g., item not found in the sale).
    /// </summary>
    [Fact(DisplayName = "Handle item not found/domain rule failure: Should throw DomainException and not persist")]
    public async Task Handle_GivenItemRemovalFails_ShouldThrowDomainExceptionAndNotPersist()
    {
        // ARRANGE
        var command = DeleteSaleItemHandlerTestData.GenerateCommandWithNonExistentItem();
        var saleId = DeleteSaleItemHandlerTestData.ValidSaleIdForArrangement();

        // Create a Sale that DOES NOT contain the ItemId from the command
        var mockedSale = SaleTestData.GenerateValidSaleWithItems(1);

        _saleRepository.GetByIdForUpdateAsync(saleId).Returns(mockedSale);

        // ACT & ASSERT: Assumes the Sale.RemoveItem() method throws a DomainException (or similar) if the item is not found.
        await Assert.ThrowsAsync<DomainException>(() =>
            _handler.Handle(command, CancellationToken.None));

        // ASSERT: Persistence and events should not occur
        await _saleRepository.DidNotReceiveWithAnyArgs().UpdateAsync(default!, default);
        await _saleRepository.DidNotReceiveWithAnyArgs().DeleteAsync(default!, default);
        await _bus.DidNotReceiveWithAnyArgs().Send(Arg.Any<SaleDeletedEvent>());
        await _bus.DidNotReceiveWithAnyArgs().Send(Arg.Any<SaleItemDeletedEvent>());
    }
}