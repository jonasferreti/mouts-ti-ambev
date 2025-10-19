using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.Handlers.TestData;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using AutoMapper;
using FluentValidation;
using NSubstitute;
using Rebus.Bus;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Handlers;


/// <summary>
/// Contains unit tests for the CreateSaleHandler, focusing on the correct orchestration of 
/// application flow: validation, mapping, persistence, and event publication via Rebus.
/// </summary>
public class CreateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IBus _bus;
    private readonly CreateSaleHandler _handler;

    public CreateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _bus = Substitute.For<IBus>();
        _mapper = Substitute.For<IMapper>();

        _handler = new CreateSaleHandler(_saleRepository, _mapper, _bus);
    }

    /// <summary>
    /// Tests the complete success flow: validation, mapping, persistence, and event sending.
    /// </summary>
    [Fact(DisplayName = "Handle success: Should persist the Sale and send a SaleCreatedEvent")]
    public async Task Given_ValidCommand_When_Handle_Then_ShouldPersistAndSendEvent()
    {
        // arrange
        var validCommand = CreateSaleHandlerTestData.GenerateValidCommand();
        var saleId = Guid.NewGuid();

        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        var expectedResult = new CreateSaleResult { Id = saleId, TotalAmount = validCommand.Items[0].Quantity * validCommand.Items[0].UnitPrice };
        _mapper.Map<CreateSaleResult>(Arg.Any<Sale>()).Returns(expectedResult);
        _mapper.Map<Sale>(validCommand).Returns(SaleTestData.GenerateValidSaleWithItems(1));
        _mapper.Map<SaleItem>(Arg.Any<CreateSaleItemCommand>())
            .Returns(SaleTestData.GenerateValidSaleItem());

        // act
        var result = await _handler.Handle(validCommand, CancellationToken.None);

        // assert
        await _saleRepository.Received(1).CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
        await _bus.Received(1).Send(Arg.Any<SaleCreatedEvent>());

        Assert.NotNull(result);
        Assert.Equal(expectedResult.Id, result.Id);
    }

    /// <summary>
    /// Tests that the handler throws a ValidationException when the AutoMapper attempts 
    /// to create a Value Object that breaks a business invariant (e.g., Money <= 0).
    /// </summary>
    [Fact(DisplayName = "Handle validation failure: Should throw ValidationException for invalid VO construction")]
    public async Task Given_CommandBreakingVOInvariant_When_Handle_Then_ShouldThrowDomainException()
    {
        // arrange
        var commandWithInvalidVO = CreateSaleHandlerTestData.GenerateCommand_BreakingMoneyInvariant();

        // assert
        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(commandWithInvalidVO, CancellationToken.None));

        await _saleRepository.DidNotReceiveWithAnyArgs().CreateAsync(Arg.Any<Sale>(), CancellationToken.None);
        await _bus.DidNotReceiveWithAnyArgs().Send(Arg.Any<SaleCreatedEvent>());
    }

    /// <summary>
    /// Tests that the Rebus event is NOT sent if the repository operation fails, 
    /// ensuring transactional integrity.
    /// </summary>
    [Fact(DisplayName = "Handle persistence failure: Should throw exception and NOT send Rebus event")]
    public async Task Given_PersistenceFails_When_Handle_Then_ShouldNotSendEvent()
    {
        // arrange
        var validCommand = CreateSaleHandlerTestData.GenerateValidCommand();

        // act
        _mapper.Map<Sale>(validCommand).Returns(SaleTestData.GenerateValidSaleWithItems(1));
        _mapper.Map<SaleItem>(Arg.Any<CreateSaleItemCommand>())
            .Returns(SaleTestData.GenerateValidSaleItem());

        _saleRepository
            .CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException<Sale>(new Exception("Database connection failed")));

        // assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(validCommand, CancellationToken.None));
        await _bus.DidNotReceiveWithAnyArgs().Send(Arg.Any<SaleCreatedEvent>());
    }
}