using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

/// <summary>
/// Repository contract for the Sale Aggregate Root.
/// Defines persistence operations without depending on the Infrastructure layer (EF Core).
/// </summary>
public interface ISaleRepository
{
    /// <summary>
    /// Retrieves a Sale aggregate by its id, including all its items.
    /// </summary>
    /// <param name="id">The  Sale aggregate identifier.</param>
    /// <returns>The Sale aggregate, or null if not found.</returns>
    Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new Sale aggregate to the persistence context asynchronously.
    /// </summary>
    /// <param name="sale">The Sale aggregate to add.</param>
    Task CreateAsync(Sale sale, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing Sale aggregate, persisting all changes tracked by the context.
    /// </summary>
    /// <param name="sale">The Sale aggregate root with modified state.</param>
    Task UpdateAsync(Sale sale);

    /// <summary>
    /// Delete Sale aggregate from the persistence context asynchronously.
    /// </summary>
    /// <param name="id">The Sale aggregate identifier to delete.</param>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
