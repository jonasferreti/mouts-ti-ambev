using Ambev.DeveloperEvaluation.Application.Exceptions;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Ambev.DeveloperEvaluation.Unit.Application.Handlers.TestData;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using AutoMapper;
using FluentValidation;
using NSubstitute;
using Rebus.Bus;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Handlers;

/// <summary>
/// Contains unit tests for the UpdateSaleHandler, focusing on the orchestration 
/// of the update flow, mapping, persistence, and event publication.
/// </summary>
public class UpdateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IBus _bus;
    private readonly UpdateSaleHandler _handler;

    public UpdateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _bus = Substitute.For<IBus>();

        // Handler instantiation with mocked dependencies
        _handler = new UpdateSaleHandler(_saleRepository, _mapper, _bus);
    }

    /// <summary>
    /// Tests the complete successful update flow: fetch, domain update, persistence, and event publishing.
    /// </summary>
    [Fact(DisplayName = "Handle success: Should update sale, persist, and publish SaleModifiedEvent")]
    public async Task Given_ValidCommand_When_Handle_Then_ShouldUpdateSalePersistAndPublishEvent()
    {
        // ARRANGE
        var mockedSale = SaleTestData.GenerateValidSale();
        var command = UpdateSaleHandlerTestData.GenerateValidCommand(mockedSale.Id);

        var expectedResult = UpdateSaleHandlerTestData.GenerateUpdateSaleResult(command);

        _saleRepository.GetByIdForUpdateAsync(mockedSale.Id, Arg.Any<CancellationToken>())
            .Returns(mockedSale);

        // Mock mapper for SaleItem conversion (used inside the Sale's Update method)
        // We ensure that when mapping an UpdateSaleItemCommand, it returns a valid SaleItem object.
        _mapper.Map<SaleItem>(Arg.Any<UpdateSaleItemCommand>())
            .Returns(
                new SaleItem(
                    new ExternalReference(Guid.NewGuid(), "Mock Product"),
                    new Quantity(1),
                    new Money(10.00m)
                ));

        // Mock mapper for the final result (mapped from the updated Sale entity)
        // We assert that the mapper is called with the Sale entity that has the command's CustomerId
        _mapper.Map<UpdateSaleResult>(Arg.Is<Sale>(s => s.Customer.Value == command.CustomerId))
            .Returns(expectedResult);

        // ACT
        var result = await _handler.Handle(command, CancellationToken.None);

        // ASSERT
        Assert.Equal(expectedResult.Id, result.Id);
        Assert.Equal(expectedResult.CustomerId, result.CustomerId);
        Assert.Equal(expectedResult.TotalSaleAmount, result.TotalSaleAmount);

        // Verify interactions
        await _saleRepository.Received(1).GetByIdForUpdateAsync(mockedSale.Id, Arg.Any<CancellationToken>());
        await _saleRepository.Received(1).UpdateAsync(mockedSale, Arg.Any<CancellationToken>());

        // Verify event publishing
        await _bus.Received(1).Send(Arg.Is<SaleModifiedEvent>(e => e.SaleId == mockedSale.Id));
    }

    /// <summary>
    /// Tests if a ValidationException is thrown for an invalid command (e.g., empty ID).
    /// </summary>
    [Fact(DisplayName = "Handle validation failure: Should throw ValidationException for invalid command")]
    public async Task Given_InvalidCommand_When_Handle_Then_ShouldThrowValidationException()
    {
        // ARRANGE
        var invalidCommand = UpdateSaleHandlerTestData.GenerateInvalidCommand_EmptyId();

        // ACT & ASSERT
        await Assert.ThrowsAsync<ValidationException>(() =>
            _handler.Handle(invalidCommand, CancellationToken.None));

        // ASSERT: No repository or bus interaction should occur
        await _saleRepository.DidNotReceiveWithAnyArgs()
            .GetByIdForUpdateAsync(Arg.Any<Guid>(), CancellationToken.None);

        await _saleRepository.DidNotReceiveWithAnyArgs()
            .UpdateAsync(Arg.Any<Sale>(), CancellationToken.None);

        await _bus.DidNotReceiveWithAnyArgs().Send(Arg.Any<SaleModifiedEvent>());
    }

    /// <summary>
    /// Tests if NotFoundException is thrown when the Sale aggregate does not exist.
    /// </summary>
    [Fact(DisplayName = "Handle not found: Should throw NotFoundException when Sale does not exist")]
    public async Task Given_NonExistentSaleId_When_Handle_Then_ShouldThrowNotFoundException()
    {
        // ARRANGE
        var saleId = Guid.NewGuid();
        var command = UpdateSaleHandlerTestData.GenerateValidCommand(saleId);

        _saleRepository.GetByIdForUpdateAsync(saleId, Arg.Any<CancellationToken>()).Returns((Sale)null!);

        // ACT & ASSERT
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));

        // ASSERT: NO update in the repository or event published
        await _saleRepository.DidNotReceiveWithAnyArgs()
            .UpdateAsync(Arg.Any<Sale>(), CancellationToken.None);

        await _bus.DidNotReceiveWithAnyArgs().Send(Arg.Any<SaleModifiedEvent>());
    }
}