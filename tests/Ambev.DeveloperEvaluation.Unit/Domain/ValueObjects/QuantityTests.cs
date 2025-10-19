using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.ValueObjects;

/// <summary>
/// Contains unit tests for the Quantity Value Object, ensuring greater than zero
/// and maximum value invariants.
/// </summary>
public class QuantityTests
{
    private const int MaxAllowed = Quantity.MaxValue;

    /// <summary>
    /// Tests that Quantity is successfully created with a valid positive value within the max limit.
    /// </summary>
    [Theory(DisplayName = "Construction should succeed with positive value within max limit")]
    [InlineData(1)]
    [InlineData(MaxAllowed)] // 20
    public void Given_ValidValue_When_Constructed_Then_ValueShouldBeSet(int validValue)
    {
        // Act
        var quantity = new Quantity(validValue);

        // Assert
        Assert.Equal(validValue, quantity.Value);
    }

    /// <summary>
    /// Tests that construction fails with a DomainException when the value is zero or less.
    /// </summary>
    [Theory(DisplayName = "Construction should throw DomainException for zero or negative value")]
    [InlineData(0)]
    [InlineData(-5)]
    public void Given_ZeroOrNegativeValue_When_Constructed_Then_ShouldThrowDomainException(int invalidValue)
    {
        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new Quantity(invalidValue));
        Assert.Contains("Quantity must be greater than zero", exception.Message); //
    }

    /// <summary>
    /// Tests that construction fails with a DomainException when the value exceeds the max allowed quantity.
    /// </summary>
    [Fact(DisplayName = "Construction should throw DomainException for value exceeding max limit")]
    public void Given_ValueExceedingMax_When_Constructed_Then_ShouldThrowDomainException()
    {
        // Arrange
        var invalidValue = MaxAllowed + 1; // 21

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new Quantity(invalidValue));
        Assert.Contains($"The maximum quantity allowed per item is {MaxAllowed}.", exception.Message); //
    }
}