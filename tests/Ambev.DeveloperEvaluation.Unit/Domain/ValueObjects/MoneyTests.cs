using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.ValueObjects;

/// <summary>
/// Contains unit tests for the Money Value Object, ensuring immutability,
/// value greater than zero, and precision rounding invariants.
/// </summary>
public class MoneyTests
{
    /// <summary>
    /// Tests that Money is successfully created with a valid positive value.
    /// </summary>
    [Fact(DisplayName = "Construction should succeed with valid positive value")]
    public void Given_PositiveValue_When_Constructed_Then_ValueShouldBeSet()
    {
        // Arrange
        const decimal validValue = 150.55M;

        // Act
        var money = new Money(validValue);

        // Assert
        Assert.Equal(validValue, money.Value);
    }

    /// <summary>
    /// Tests that Money rounds the value to two decimal places upon construction.
    /// </summary>
    [Fact(DisplayName = "Construction should round value to two decimal places")]
    public void Given_ValueWithMoreThanTwoDecimals_When_Constructed_Then_ValueShouldBeRounded()
    {
        // Arrange
        const decimal unroundedValue = 100.1234M;
        const decimal expectedValue = 100.12M;

        // Act
        var money = new Money(unroundedValue);

        // Assert
        Assert.Equal(expectedValue, money.Value);
    }

    /// <summary>
    /// Tests that construction fails with a DomainException when the value is zero or less.
    /// </summary>
    [Theory(DisplayName = "Construction should throw DomainException for zero or negative value")]
    [InlineData(0)]
    [InlineData(-10.50)]
    public void Given_ZeroOrNegativeValue_When_Constructed_Then_ShouldThrowDomainException(decimal invalidValue)
    {
        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new Money(invalidValue));
        Assert.Contains("Money must be greater than zero.", exception.Message);
    }
}
