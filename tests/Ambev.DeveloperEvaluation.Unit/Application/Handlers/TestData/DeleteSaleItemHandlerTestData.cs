using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

namespace Ambev.DeveloperEvaluation.Unit.Application.Handlers.TestData;

/// <summary>
/// Provides test objects for the DeleteSaleItemCommand, ensuring deterministic IDs for setup.
/// </summary>
public static class DeleteSaleItemHandlerTestData
{
    // The specific IDs used for the "happy path" tests
    private static readonly Guid TestSaleId = Guid.NewGuid();
    private static readonly Guid TestItemId = Guid.NewGuid();

    // IDs for specific failure scenarios
    private static readonly Guid NonExistentSaleId = Guid.NewGuid();
    private static readonly Guid NonExistentItemId = Guid.NewGuid();

    /// <summary>
    /// Generates a valid DeleteSaleItemCommand using SaleId and ItemId parameters
    /// </summary>
    public static DeleteSaleItemCommand GenerateValidCommand(Guid SaleId, Guid itemId)
    {
        return new DeleteSaleItemCommand(SaleId, itemId);
    }

    /// <summary>
    /// Generates a valid DeleteSaleItemCommand using pre-defined Test IDs.
    /// </summary>
    public static DeleteSaleItemCommand GenerateValidCommand()
    {
        return new DeleteSaleItemCommand(TestSaleId, TestItemId);
    }

    /// <summary>
    /// Generates a command for a non-existent item (ItemId is a new GUID).
    /// </summary>
    public static DeleteSaleItemCommand GenerateCommandWithNonExistentItem()
    {
        return new DeleteSaleItemCommand(TestSaleId, NonExistentItemId);
    }

    /// <summary>
    /// Generates a command for a non-existent sale (SaleId is a random GUID).
    /// </summary>
    public static DeleteSaleItemCommand GenerateCommandWithNonExistentSale()
    {
        return new DeleteSaleItemCommand(NonExistentSaleId, TestItemId);
    }

    /// <summary>
    /// Generates an invalid command with an empty SaleId (violates NotEmpty rule).
    /// </summary>
    public static DeleteSaleItemCommand GenerateInvalidCommand_EmptySaleId()
    {
        return new DeleteSaleItemCommand(Guid.Empty, TestItemId);
    }

    /// <summary>
    /// Generates an invalid command with an empty ItemId (violates NotEmpty rule).
    /// </summary>
    public static DeleteSaleItemCommand GenerateInvalidCommand_EmptyItemId()
    {
        return new DeleteSaleItemCommand(TestSaleId, Guid.Empty);
    }

    /// <summary>
    /// Provides the Sale ID used for the happy path command (for repository arrangement).
    /// </summary>
    public static Guid ValidSaleIdForArrangement() => TestSaleId;

}