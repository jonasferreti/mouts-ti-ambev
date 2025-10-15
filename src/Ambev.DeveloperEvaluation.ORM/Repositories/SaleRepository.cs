using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

/// <summary>
/// Implementation of the ISaleRepository using Entity Framework Core.
/// </summary>
public class SaleRepository : ISaleRepository
{
    private readonly DefaultContext _context;

    public SaleRepository(DefaultContext context, CancellationToken cancellationToken = default)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves a Sale aggregate by its id, including all its items.
    /// </summary>
    /// <param name="id">The  Sale aggregate identifier.</param>
    /// <returns>The Sale aggregate, or null if not found.</returns>
    public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .SingleOrDefaultAsync(s => s.Id == id);
    }

    /// <summary>
    /// Create a new Sale aggregate to the persistence context asynchronously.
    /// </summary>
    /// <param name="sale">The Sale aggregate to add.</param>
    public async Task CreateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        await _context.Sales.AddAsync(sale, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Delete Sale aggregate from the persistence context asynchronously.
    /// </summary>
    /// <param name="id">The Sale aggregate identifier to delete.</param>
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sale = await GetByIdAsync(id, cancellationToken);
        if (sale is null)
            return false;

        _context.Sales.Remove(sale);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
