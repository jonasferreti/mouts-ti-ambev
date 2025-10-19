using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.ValueObjects;

/// <summary>
/// Contains unit tests for the ExternalReference Value Object, ensuring that both
/// the ID and the description invariants are enforced.
/// </summary>
public class ExternalReferenceTests
{
    /// <summary>
    /// Tests that ExternalReference is successfully created with valid GUID and description.
    /// </summary>
    [Fact(DisplayName = "Construction should succeed with valid GUID and description")]
    public void Given_ValidData_When_Constructed_Then_PropertiesShouldBeSet()
    {
        // Arrange
        var validGuid = Guid.NewGuid();
        const string validDescription = "Product ABC";

        // Act
        var externalRef = new ExternalReference(validGuid, validDescription);

        // Assert
        Assert.Equal(validGuid, externalRef.Value);
        Assert.Equal(validDescription, externalRef.Description);
    }

    /// <summary>
    /// Tests that construction throws a DomainException when the GUID is Guid.Empty.
    /// </summary>
    [Fact(DisplayName = "Construction should throw DomainException for Empty GUID")]
    public void Given_EmptyGuid_When_Constructed_Then_ShouldThrowDomainException()
    {
        // Arrange
        var invalidGuid = Guid.Empty;
        const string validDescription = "Test Description";

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new ExternalReference(invalidGuid, validDescription));
        Assert.Contains("External ID is required.", exception.Message); //
    }

    /// <summary>
    /// Tests that construction throws a DomainException when the description is null or whitespace.
    /// </summary>
    [Theory(DisplayName = "Construction should throw DomainException for missing description")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Given_MissingDescription_When_Constructed_Then_ShouldThrowDomainException(string? invalidDescription)
    {
        // Arrange
        var validGuid = Guid.NewGuid();

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new ExternalReference(validGuid, invalidDescription!));
        Assert.Contains("The external identity description is required.", exception.Message);
    }
}