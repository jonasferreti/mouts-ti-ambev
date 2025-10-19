using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.Handlers.TestData;

/// <summary>
/// Provides methods for generating test data for the CreateSaleCommand and its inner CreateSaleItemCommand,
/// using the Bogus library. This class centralizes all valid and specific invalid data scenarios.
/// </summary>
public static class CreateSaleHandlerTestData
{
    // Constant for Quantity MaxValue (from Quantity.cs: public const int MaxValue = 20;)
    private const int MaxQuantityValue = 20;

    /// <summary>
    /// Static instance of Faker for generic methods (used for generating random product names/words).
    /// </summary>
    private static readonly Faker Faker = new(); // CORREÇÃO: Inicialização estática do Faker

    /// <summary>
    /// Configures the Faker to generate valid Sale Item commands.
    /// The generated items will have valid:
    /// - ProductId (Guid)
    /// - ProductName (Commerce Name)
    /// - Quantity (Between 1 and 20)
    /// - UnitPrice (Greater than 0)
    /// </summary>
    private static readonly Faker<CreateSaleItemCommand> createSaleItemCommandFaker = new Faker<CreateSaleItemCommand>()
        .RuleFor(i => i.ProductId, f => f.Random.Guid())
        .RuleFor(i => i.ProductName, f => f.Commerce.ProductName())
        // Quantity must be between 1 and 20 (Quantity.MaxValue)
        .RuleFor(i => i.Quantity, f => f.Random.Int(1, MaxQuantityValue))
        // Price must be greater than 0 (Money invariant)
        .RuleFor(i => i.UnitPrice, f => f.Finance.Amount(0.01M, 500.00M, 2));

    /// <summary>
    /// Configures the Faker to generate a valid Sale Command.
    /// The generated command will have valid:
    /// - CustomerId (Guid)
    /// - CustomerName (Full Name)
    /// - BranchId (Guid)
    /// - BranchName (Company Name)
    /// - Items (List of 1 to 3 valid Sale Items)
    /// </summary>
    private static readonly Faker<CreateSaleCommand> createSaleHandlerFaker = new Faker<CreateSaleCommand>()
        .RuleFor(c => c.CustomerId, f => f.Random.Guid())
        .RuleFor(c => c.CustomerName, f => f.Person.FullName)
        .RuleFor(c => c.BranchId, f => f.Random.Guid())
        .RuleFor(c => c.BranchName, f => f.Company.CompanyName())
        // Generate a list of 1 to 3 valid Sale Item commands
        .RuleFor(c => c.Items, f => createSaleItemCommandFaker.Generate(f.Random.Int(1, 3)));


    /// <summary>
    /// Generates a valid CreateSaleCommand with randomized data.
    /// The generated command will have all properties populated with valid values
    /// that meet the system's validation requirements.
    /// </summary>
    /// <returns>A valid CreateSaleCommand with randomly generated data.</returns>
    public static CreateSaleCommand GenerateValidCommand()
    {
        return createSaleHandlerFaker.Generate();
    }


    /// <summary>
    /// Generates a command that contains an item breaking a Domain Value Object invariant (UnitPrice = 0M),
    /// which will lead to a DomainException during mapping/creation.
    /// </summary>
    /// <returns>A CreateSaleCommand that breaks the Money Value Object invariant.</returns>
    public static CreateSaleCommand GenerateCommand_BreakingMoneyInvariant()
    {
        // Generates a valid sale and overrides the items with one invalid item.
        var command = createSaleHandlerFaker.Generate();

        // Overrides the items list with a manually constructed item that fails the Money invariant (value <= 0)
        command.Items =
        [
            new CreateSaleItemCommand
            {
                ProductId = command.BranchId,
                ProductName = Faker.Lorem.Word(),
                Quantity = 1,
                UnitPrice = 0M 
            }
        ];
        return command;
    }
}