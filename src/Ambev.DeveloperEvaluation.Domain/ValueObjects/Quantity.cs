namespace Ambev.DeveloperEvaluation.Domain.ValueObjects;

/// <summary>
/// Represents the quantity of a sales item.
/// Contains the business rule that restricts the maximum quantity per item (<= 20).
/// </summary>
public record Quantity
{
    public const int MaxValue = 20;

    /// <summary>The quantity value.</summary>
    public int Value { get; private set; }

    protected Quantity() { }

    public Quantity(int value)
    {
        if (value <= 0)
            throw new DomainException("Quantity must be greater than zero");

        if (value > MaxValue)
            throw new DomainException($"The maximum quantity allowed per item is {MaxValue}.");

        Value = value;
    }
}
