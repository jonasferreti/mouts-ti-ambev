using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Extensions;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Extensions;

/// <summary>
/// Contains unit tests for the SaleExtensions class, ensuring correct mapping
/// of Sale Aggregate data to the corresponding Domain Event objects.
/// </summary>
public class SaleExtensionsTests
{
    private readonly Sale _sale;
    private readonly Guid _testItemId;
    private readonly decimal _testCalculatedTotalAmount;
    private readonly Guid _testCustomerId = Guid.NewGuid();
    private readonly Guid _testBranchId = Guid.NewGuid();

    public SaleExtensionsTests()
    {
        var customerRef = new ExternalReference(_testCustomerId, "Test Customer");
        var branchRef = new ExternalReference(_testBranchId, "Test Branch");

        _sale = SaleTestData.GenerateValidSaleWithItems(
            initialItemCount: 2,
            number: 1001,
            quantity: 1,
            unitPrice: 100.00M,
            customer: customerRef,
            branch: branchRef
        );

        _testCalculatedTotalAmount = _sale.TotalAmount.Value;
        _testItemId = _sale.Items.First().Id;
    }


    /// <summary>
    /// Tests that the SaleCreatedEvent extension correctly maps the Sale ID, external references, 
    /// and the final calculated TotalAmount.
    /// </summary>
    [Fact(DisplayName = "SaleCreatedEvent extension should correctly map core Sale data")]
    public void Given_Sale_When_CallingSaleCreatedEvent_Then_EventShouldContainCorrectData()
    {
        // Act
        var createdEvent = _sale.SaleCreatedEvent();

        // Assert
        Assert.Equal(_sale.Id, createdEvent.SaleId);
        Assert.Equal(_testCustomerId, createdEvent.CustomerId);
        Assert.Equal(_testBranchId, createdEvent.BranchId);
        Assert.Equal(_testCalculatedTotalAmount, createdEvent.TotalAmount);
    }


    /// <summary>
    /// Tests that the SaleCancelledEvent extension correctly maps the Sale ID.
    /// </summary>
    [Fact(DisplayName = "SaleCancelledEvent extension should map the Sale ID")]
    public void Given_Sale_When_CallingSaleCancelledEvent_Then_EventShouldContainSaleId()
    {
        // Act
        var cancelledEvent = _sale.SaleCancelledEvent();

        // Assert
        Assert.Equal(_sale.Id, cancelledEvent.SaleId);
    }

    /// <summary>
    /// Tests that the SaleModifiedEvent extension correctly maps the latest mutable state of the Sale, 
    /// including updated external references and TotalAmount.
    /// </summary>
    [Fact(DisplayName = "SaleModifiedEvent extension should correctly map mutable Sale data")]
    public void Given_Sale_When_CallingSaleModifiedEvent_Then_EventShouldContainCorrectData()
    {
        // Act
        var modifiedEvent = _sale.SaleModifiedEvent();

        // Assert
        Assert.Equal(_sale.Id, modifiedEvent.SaleId);
        Assert.Equal(_testCustomerId, modifiedEvent.CustomerId);
        Assert.Equal(_testBranchId, modifiedEvent.BranchId);
        Assert.Equal(_testCalculatedTotalAmount, modifiedEvent.TotalAmount);
    }

    /// <summary>
    /// Tests that the SaleItemCancelledEvent extension correctly maps both the parent Sale ID and the specific Item ID.
    /// </summary>
    [Fact(DisplayName = "SaleItemCancelledEvent extension should map both Sale ID and Item ID")]
    public void Given_SaleAndItemId_When_CallingSaleItemCancelledEvent_Then_EventShouldContainBothIDs()
    {
        // Act
        var itemCancelledEvent = _sale.SaleItemCancelledEvent(_testItemId);

        // Assert
        Assert.Equal(_sale.Id, itemCancelledEvent.SaleId);
        Assert.Equal(_testItemId, itemCancelledEvent.ItemId);
    }

    /// <summary>
    /// Tests that the SaleDeletedEvent extension correctly maps the Sale ID.
    /// </summary>
    [Fact(DisplayName = "SaleDeletedEvent extension should map the Sale ID")]
    public void Given_Sale_When_CallingSaleDeletedEvent_Then_EventShouldContainSaleId()
    {
        // Act
        var deletedEvent = _sale.SaleDeletedEvent();

        // Assert
        Assert.Equal(_sale.Id, deletedEvent.SaleId);
    }

    /// <summary>
    /// Tests that the SaleItemDeletedEvent extension correctly maps both the parent Sale ID and the specific Item ID.
    /// </summary>
    [Fact(DisplayName = "SaleItemDeletedEvent extension should map both Sale ID and Item ID")]
    public void Given_SaleAndItemId_When_CallingSaleItemDeletedEvent_Then_EventShouldContainBothIDs()
    {
        // Act
        var itemDeletedEvent = _sale.SaleItemDeletedEvent(_testItemId);

        // Assert
        Assert.Equal(_sale.Id, itemDeletedEvent.SaleId);
        Assert.Equal(_testItemId, itemDeletedEvent.ItemId);
    }
}
