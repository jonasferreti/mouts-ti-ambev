using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Shared;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.Handlers.TestData;

/// <summary>
/// Provides test objects for the GetSalesQuery and PaginatedList results using Bogus/Faker.
/// </summary>
public static class GetSalesHandlerTestData
{

    // Faker for the individual result DTO
    private static readonly Faker<GetSaleResult> saleResultFaker = new Faker<GetSaleResult>()
        .RuleFor(r => r.Id, f => f.Random.Guid())
        .RuleFor(r => r.Number, f => f.Random.Int(1000, 9999))
        .RuleFor(r => r.CustomerName, f => f.Person.FullName)
        .RuleFor(r => r.TotalAmount, f => f.Finance.Amount(100, 5000));


    // Faker for GetSalesCriteria structure
    private static readonly Faker<GetSalesCriteria> getSalesCriteriaFaker = new Faker<GetSalesCriteria>()
        .RuleFor(c => c.SortField, f => f.Random.Enum<SaleSortField>())
        .RuleFor(c => c.SortDirection, f => f.Random.Enum<SortDirection>())
        .RuleFor(c => c.CustomerName, f => f.Person.FullName.Split(' ').First())
        .RuleFor(c => c.BranchName, f => f.Address.City())
        .RuleFor(c => c.ProductName, f => f.Commerce.ProductName());


    /// <summary>
    /// Generates a valid GetSalesQuery with optional criteria.
    /// </summary>
    public static GetSalesQuery GenerateValidQuery(int pageNumber = 1, int pageSize = 10, bool withCriteria = false)
    {
        return new GetSalesQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            Criteria = withCriteria ? getSalesCriteriaFaker.Generate() : new()
        };
    }

    /// <summary>
    /// Generates a query that would violate the PageSize validation rules (e.g., PageSize > 100).
    /// </summary>
    public static GetSalesQuery GenerateInvalidQuery_InvalidPageSize()
    {
        return new GetSalesQuery { PageNumber = 1, PageSize = 101 };
    }


    /// <summary>
    /// Generates a list of GetSaleResult DTOs.
    /// </summary>
    public static List<GetSaleResult> GenerateSaleResults(int count)
    {
        return saleResultFaker.Generate(count);
    }

    /// <summary>
    /// Generates a PaginatedList suitable for a Cache Hit scenario.
    /// </summary>
    public static PaginatedList<GetSaleResult> GenerateCachedPaginatedList(int totalCount = 50, int pageNumber = 2, int pageSize = 10)
    {
        var data = GenerateSaleResults(pageSize);
        return new PaginatedList<GetSaleResult>(data, totalCount, pageNumber, pageSize);
    }
}
