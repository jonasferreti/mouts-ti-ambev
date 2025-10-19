using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

namespace Ambev.DeveloperEvaluation.Unit.Application.Handlers.TestData;

/// <summary>
/// Provides test objects for the CancelSaleItemCommand.
/// </summary>
public static class CancelSaleItemHandlerTestData
{
    private static readonly Guid ValidSaleId = Guid.NewGuid();
    private static readonly Guid ValidItemId = Guid.NewGuid();
    private static readonly Guid NonExistentItemId = Guid.NewGuid();



    /// <summary>
    /// Generates a valid CancelSaleItemCommand with SaleId and ItemId
    /// </summary>
    public static CancelSaleItemCommand GenerateValidCommand(Guid SaleId, Guid ItemId)
    {
        return new CancelSaleItemCommand(SaleId, ItemId);
    }

    /// <summary>
    /// Generates a valid CancelSaleItemCommand with pre-defined IDs.
    /// </summary>
    public static CancelSaleItemCommand GenerateValidCommand()
    {
        return new CancelSaleItemCommand(ValidSaleId, ValidItemId);
    }

    /// <summary>
    /// Generates a command for a non-existent item (ItemId is a new GUID).
    /// </summary>
    public static CancelSaleItemCommand GenerateNonExistentItemCommand()
    {
        return new CancelSaleItemCommand(ValidSaleId, NonExistentItemId);
    }

    /// <summary>
    /// Generates an invalid command with an empty SaleId (violates NotEmpty rule).
    /// </summary>
    public static CancelSaleItemCommand GenerateInvalidCommand_EmptySaleId()
    {
        return new CancelSaleItemCommand(Guid.Empty, ValidItemId);
    }

    /// <summary>
    /// Generates an invalid command with an empty ItemId (violates NotEmpty rule).
    /// </summary>
    public static CancelSaleItemCommand GenerateInvalidCommand_EmptyItemId()
    {
        return new CancelSaleItemCommand(ValidSaleId, Guid.Empty);
    }

    /// <summary>
    /// Provides the Sale ID used for the valid command (for use in the repository arrangement).
    /// </summary>
    public static Guid ValidIdForArrangement() => ValidSaleId;
}
