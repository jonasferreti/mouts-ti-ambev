using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;


/// <summary>
/// Contains unit tests for the SaleItem entity, focusing on discount calculation,
/// item cancellation, and related invariants.
/// </summary>
public class SaleItemTests
{
    /// <summary>
    /// Tests that 0% discount is applied when the quantity is less than 4.
    /// </summary>
    [Theory(DisplayName = "0% discount should be applied for quantity less than 4")]
    [InlineData(1)]
    [InlineData(3)]
    public void Given_QuantityLessThanFour_When_Constructed_Then_DiscountShouldBeZeroPercent(int quantity)
    {
        // Arrange
        var unitPrice = new Money(100.00M);
        var productRef = SaleTestData.GenerateExternalReference();
        var expectedTotal = quantity * unitPrice.Value;

        // Act
        var item = new SaleItem(productRef, new Quantity(quantity), unitPrice);

        // Assert
        Assert.Equal(0M, item.DiscountPercentage);
        Assert.Equal(expectedTotal, item.TotalAmount.Value);
    }

    /// <summary>
    /// Tests that 10% discount is applied when the quantity is between 4 and 9.
    /// </summary>
    [Theory(DisplayName = "10% discount should be applied for quantity between 4 and 9")]
    [InlineData(4)]
    [InlineData(9)]
    public void Given_QuantityBetweenFourAndNine_When_Constructed_Then_DiscountShouldBeTenPercent(int quantity)
    {
        // Arrange
        var unitPrice = new Money(100.00M);
        var productRef = SaleTestData.GenerateExternalReference();
        var subTotal = quantity * unitPrice.Value;
        var expectedTotal = subTotal * 0.90M;

        // Act
        var item = new SaleItem(productRef, new Quantity(quantity), unitPrice);

        // Assert
        Assert.Equal(0.10M, item.DiscountPercentage);
        Assert.Equal(expectedTotal, item.TotalAmount.Value);
    }

    /// <summary>
    /// Tests that 20% discount is applied when the quantity is 10 or more.
    /// </summary>
    [Theory(DisplayName = "20% discount should be applied for quantity 10 or more")]
    [InlineData(10)]
    [InlineData(15)]
    public void Given_QuantityTenOrMore_When_Constructed_Then_DiscountShouldBeTwentyPercent(int quantity)
    {
        // Arrange
        var unitPrice = new Money(100.00M);
        var productRef = SaleTestData.GenerateExternalReference();
        var subTotal = quantity * unitPrice.Value;
        var expectedTotal = subTotal * 0.80M;

        // Act
        var item = new SaleItem(productRef, new Quantity(quantity), unitPrice);

        // Assert
        Assert.Equal(0.20M, item.DiscountPercentage);
        Assert.Equal(expectedTotal, item.TotalAmount.Value);
    }


    /// <summary>
    /// Tests that the Cancel method sets the IsCancelled flag to true.
    /// </summary>
    [Fact(DisplayName = "Cancel on uncancelled item should set IsCancelled to true")]
    public void Given_UncancelledItem_When_Cancel_Then_IsCancelledShouldBeTrue()
    {
        // Arrange
        var item = SaleTestData.GenerateValidSaleItem();

        // Act
        item.Cancel();

        // Assert
        Assert.True(item.IsCancelled);
    }

    /// <summary>
    /// Tests that attempting to call Cancel on an already cancelled item throws a DomainException.
    /// </summary>
    [Fact(DisplayName = "Cancel on cancelled item should throw DomainException")]
    public void Given_CancelledItem_When_Cancel_Then_ShouldThrowDomainException()
    {
        // Arrange
        var item = SaleTestData.GenerateValidSaleItem();
        item.Cancel();

        // Act & Assert
        Assert.Throws<DomainException>(() => item.Cancel());
    }

    /// <summary>
    /// Tests that calling CalculateTotal explicitly recalculates the discount and total
    /// based on the item's current quantity. We test the 20% discount case.
    /// </summary>
    [Fact(DisplayName = "CalculateTotal should re-apply 20% discount based on current quantity")]
    public void Given_ExistingItem_When_CalculateTotalIsCalled_Then_DiscountAndTotalShouldBeRecalculated()
    {
        const int highQuantity = 12;
        const decimal unitPrice = 50.00M;
        var productRef = SaleTestData.GenerateExternalReference();

        var item = new SaleItem(productRef, new Quantity(highQuantity), new Money(unitPrice));

        const decimal expectedTotal = 480.00M;
        const decimal expectedDiscount = 0.20M;

        item.CalculateTotal();

        Assert.Equal(expectedDiscount, item.DiscountPercentage);
        Assert.Equal(expectedTotal, item.TotalAmount.Value);
    }
}