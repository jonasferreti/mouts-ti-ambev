using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Shared;
using Ambev.DeveloperEvaluation.ORM.Extensions;
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
    /// <returns>The Sale aggregate (no tracking), or null if not found.</returns>
    public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .AsNoTracking()
            .Include(s => s.Items)
            .SingleOrDefaultAsync(s => s.Id == id, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Retrieves a Sale aggregate by its id, including all its items for updating purposes
    /// </summary>
    /// <param name="id">The  Sale aggregate identifier.</param>
    /// <returns>The Sale aggregate, or null if not found.</returns>
    public async Task<Sale?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .SingleOrDefaultAsync(s => s.Id == id, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Retrieves a paginated list of Sales.
    /// </summary>
    /// <param name="page">Page number.</param>
    /// <param name="size">Items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A tuple containing the list of Sales and the total count before pagination.</returns>
    public async Task<(List<Sale> sales, int count)> GetPaginatedAsync(int page, int size,
        SaleSearchCriteria criteria,
        CancellationToken cancellationToken)
    {
        var query = _context.Sales
            .Include(s => s.Items)
            .AsQueryable();

        query = query
            .ApplyCustomerFilter(criteria.CustomerName)
            .ApplyBranchFilter(criteria.BranchName)
            .ApplyProductFilter(criteria.ProductName)
            .ApplyOrderBy(criteria.SortField, criteria.SortDirection);

        var count = await query.CountAsync(cancellationToken);
        var sales = await query
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);

        return (sales, count);
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
    /// Updates an existing Sale aggregate in the database.
    /// </summary>
    /// <param name="sale">The Sale aggregate root.</param>
    public async Task UpdateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        _context.Sales.Update(sale);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Delete Sale aggregate from the persistence context asynchronously.
    /// </summary>
    /// <param name="id">The Sale aggregate identifier to delete.</param>
    public async Task DeleteAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        _context.Sales.Remove(sale);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
