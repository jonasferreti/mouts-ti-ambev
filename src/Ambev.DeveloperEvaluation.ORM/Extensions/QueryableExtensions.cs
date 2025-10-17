using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Shared;

namespace Ambev.DeveloperEvaluation.ORM.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<Sale> ApplyCustomerFilter(this IQueryable<Sale> query, string? customerName)
    {
        if (!string.IsNullOrWhiteSpace(customerName))
            return query.Where(s => s.Customer.Description.ToLower().Contains(customerName.ToLower()));

        return query;
    }

    public static IQueryable<Sale> ApplyBranchFilter(this IQueryable<Sale> query, string? branchName)
    {
        if (!string.IsNullOrWhiteSpace(branchName))
            return query.Where(s => s.Branch.Description.ToLower().Contains(branchName.ToLower()));

        return query;

    }

    public static IQueryable<Sale> ApplyProductFilter(this IQueryable<Sale> query, string? productName)
    {
        if (!string.IsNullOrWhiteSpace(productName))
            return query = query.Where(s => s.Items.Any(item =>
                item.Product.Description.ToLower().Contains(productName.ToLower())));

        return query;
    }

    public static IQueryable<Sale> ApplyOrderBy(this IQueryable<Sale> query, 
        SaleSortField? saleSortField, SortDirection? sortDirection)
    {
        if (saleSortField is null)
            return query;

        return (sortDirection == SortDirection.Descending)
            ? query.ApplyOrderByDescending(saleSortField.Value)
            : query.ApplyOrderBy(saleSortField.Value);
    }

    private static IQueryable<Sale> ApplyOrderBy(this IQueryable<Sale> query,
        SaleSortField saleSortField)
    {
        return saleSortField switch
        {
            SaleSortField.CreatedDate => query.OrderBy(s => s.CreatedDate),
            SaleSortField.CustomerName => query.OrderBy(s => s.Customer.Description),
            SaleSortField.BranchName => query.OrderBy(s => s.Branch.Description),
            _ => query
        };
    }

    private static IQueryable<Sale> ApplyOrderByDescending(this IQueryable<Sale> query,
       SaleSortField saleSortField)
    {
        return saleSortField switch
        {
            SaleSortField.CreatedDate => query.OrderByDescending(s => s.CreatedDate),
            SaleSortField.CustomerName => query.OrderByDescending(s => s.Customer.Description),
            SaleSortField.BranchName => query.OrderByDescending(s => s.Branch.Description),
            _ => query
        };
    }
}
