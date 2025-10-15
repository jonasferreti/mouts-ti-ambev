namespace Ambev.DeveloperEvaluation.Domain.ValueObjects;

/// <summary>
/// Represents a monetary value, ensuring precision and invariants like greater than zero.
/// </summary>
public record Money
{
    public decimal Value { get; private set; }

    protected Money() { }

    public Money(decimal value)
    {
        if (value <= 0)
            throw new DomainException("Money must be greater than zero.");

        Value = Math.Round(value, 2);
    }
   
}
