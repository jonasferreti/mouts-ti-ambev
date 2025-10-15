namespace Ambev.DeveloperEvaluation.Domain.ValueObjects;

/// <summary>
/// Represents an external identity from another domain
/// using the Guid type (standard in microservices)
/// </summary>
public record ExternalReference
{
    /// <summary>
    /// The actual GUID of the entity in the external domain
    /// </summary>
    public Guid Value { get; private set; }

    /// <summary>
    /// Denormalized description of the entity
    /// </summary>
    public string Description { get; private set; } = string.Empty;

    protected ExternalReference()
    {
    }

    public ExternalReference(Guid value, string description)
    {
        if (value == Guid.Empty)
            throw new DomainException("External ID is required.");

        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("The external identity description is required.");

        Value = value;
        Description = description;
    }
}