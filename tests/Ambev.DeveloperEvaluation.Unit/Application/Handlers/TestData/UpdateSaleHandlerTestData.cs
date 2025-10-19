using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.Handlers.TestData;

/// <summary>
/// Provides test objects for the UpdateSaleCommand and related DTOs using Bogus/Faker.
/// </summary>
public static class UpdateSaleHandlerTestData
{
    private static readonly Faker _faker = new Faker();

    // Faker for generating sale items
    private static readonly Faker<UpdateSaleItemCommand> saleItemCommandFaker = new Faker<UpdateSaleItemCommand>()
        .RuleFor(i => i.ProductId, f => f.Random.Guid())
        .RuleFor(i => i.ProductName, f => f.Commerce.ProductName())
        .RuleFor(i => i.Quantity, f => f.Random.Int(1, 10))
        .RuleFor(i => i.UnitPrice, f => f.Finance.Amount(5.00m, 500.00m));

    // Faker for generating the main command
    private static readonly Faker<UpdateSaleCommand> updateSaleCommandFaker = new Faker<UpdateSaleCommand>()
        // ID must be set by the test method, as it depends on the mocked repository fetch
        .RuleFor(c => c.Id, f => f.Random.Guid())
        .RuleFor(c => c.CustomerId, f => f.Random.Guid())
        .RuleFor(c => c.CustomerName, f => f.Person.FullName)
        .RuleFor(c => c.BranchId, f => f.Random.Guid())
        .RuleFor(c => c.BranchName, f => f.Company.CompanyName())
        .RuleFor(c => c.Items, f => saleItemCommandFaker.Generate(f.Random.Int(1, 3)));

    /// <summary>
    /// Generates a valid UpdateSaleCommand with realistic data.
    /// </summary>
    /// <param name="saleId">The ID of the sale to be updated.</param>
    public static UpdateSaleCommand GenerateValidCommand(Guid saleId)
    {
        var command = updateSaleCommandFaker.Generate();
        command.Id = saleId;

        return command;
    }

    /// <summary>
    /// Generates an invalid UpdateSaleCommand (e.g., with an empty ID) to test validation failures.
    /// </summary>
    public static UpdateSaleCommand GenerateInvalidCommand_EmptyId()
    {
        var command = updateSaleCommandFaker.Generate();
        command.Id = Guid.Empty;

        return command;
    }

    /// <summary>
    /// Calculates the total amount for the command to be used in the expected result DTO.
    /// </summary>
    /// <param name="command">The command containing the items.</param>
    public static decimal CalculateTotalSaleAmount(UpdateSaleCommand command)
    {
        return command.Items.Sum(item => item.Quantity * item.UnitPrice);
    }

    /// <summary>
    /// Generates the expected UpdateSaleResult DTO based on the command data.
    /// </summary>
    /// <param name="command">The command used for the update.</param>
    public static UpdateSaleResult GenerateUpdateSaleResult(UpdateSaleCommand command)
    {
        return new UpdateSaleResult
        {
            Id = command.Id,
            CustomerId = command.CustomerId,
            TotalSaleAmount = CalculateTotalSaleAmount(command)
        };
    }
}