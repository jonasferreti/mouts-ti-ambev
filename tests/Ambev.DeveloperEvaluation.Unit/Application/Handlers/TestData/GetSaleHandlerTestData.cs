using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.Handlers.TestData;

/// <summary>
/// Provides test objects for the GetSaleQuery and GetSaleResult, ensuring deterministic IDs for setup.
/// </summary>
public static class GetSaleHandlerTestData
{
    private static readonly Guid TestSaleId = Guid.NewGuid();
    private static readonly Guid NonExistentSaleId = Guid.NewGuid();

    // Define Faker for the DTO (Result)
    private static readonly Faker<GetSaleResult> saleResultFaker = new Faker<GetSaleResult>()
        .RuleFor(r => r.Id, f => f.Random.Guid())
        .RuleFor(r => r.Number, f => f.Random.Int(1000, 9999))
        .RuleFor(r => r.CustomerName, f => f.Person.FullName)
        .RuleFor(r => r.TotalAmount, f => f.Finance.Amount(100, 5000));


    /// <summary>
    /// Generates a valid GetSaleQuery using the pre-defined Test ID.
    /// </summary>
    public static GetSaleQuery GenerateValidQuery()
    {
        return new GetSaleQuery(TestSaleId);
    }

    /// <summary>
    /// Generates a query for a non-existent sale ID.
    /// </summary>
    public static GetSaleQuery GenerateNotFoundQuery()
    {
        return new GetSaleQuery(NonExistentSaleId);
    }

    /// <summary>
    /// Generates an invalid query with an empty SaleId (violates NotEmpty rule).
    /// </summary>
    public static GetSaleQuery GenerateInvalidQuery_EmptySaleId()
    {
        return new GetSaleQuery(Guid.Empty);
    }

    /// <summary>
    /// Generates the expected DTO result, ensuring the ID matches the TestSaleId for consistency.
    /// </summary>
    public static GetSaleResult GenerateExpectedResult()
    {
        // Generates the DTO and overrides the ID to match the static TestSaleId
        return saleResultFaker
            .RuleFor(r => r.Id, TestSaleId)
            .Generate();
    }

    /// <summary>
    /// Generates a random DTO result with a specific, random ID. Useful for cache hit scenarios 
    /// where the Sale ID is generated dynamically during the test.
    /// </summary>
    public static GetSaleResult GenerateRandomResult(Guid id)
    {
        // Generates the DTO and overrides the ID to match the provided ID
        return saleResultFaker
            .RuleFor(r => r.Id, id)
            .Generate();
    }

    /// <summary>
    /// Provides the Sale ID used for the happy path (for repository arrangement and cache key).
    /// </summary>
    public static Guid ValidSaleIdForArrangement() => TestSaleId;

    /// <summary>
    /// Provides the expected cache key string.
    /// </summary>
    public static string ValidCacheKey() => TestSaleId.ToString();
}